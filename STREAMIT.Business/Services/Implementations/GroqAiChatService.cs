using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using STREAMIT.Business.Dtos.AiDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Services.Abstractions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace STREAMIT.Business.Services.Implementations
{
    public class GroqAiChatService : IAiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GroqAiChatService> _logger;

        private const string GroqApiUrl = "https://api.groq.com/openai/v1/chat/completions";

        // System prompt: tells Groq it's a STREAMIT movie assistant
        private const string SystemPrompt =
            "You are StreamBot, a helpful AI assistant for STREAMIT — a movie and TV-show streaming platform. " +
            "You help users discover movies, explain genres, suggest shows based on their mood, " +
            "answer questions about cast, plot, ratings, and anything related to films or the platform. " +
            "Be friendly, concise, and enthusiastic about cinema. " +
            "If a question is completely unrelated to movies/entertainment/the platform, politely redirect the user.";

        public GroqAiChatService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GroqAiChatService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("GroqClient");
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ResultDto<ChatResponseDto>> SendMessageAsync(ChatRequestDto request)
        {
            try
            {
                var apiKey = _configuration["GroqOptions:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                    return new ResultDto<ChatResponseDto>("Groq API key is not configured.", 500, false, new ChatResponseDto());

                var model = _configuration["GroqOptions:Model"] ?? "llama-3.3-70b-versatile";

                // Build messages array: inject system prompt at the beginning
                var messages = new List<object>
                {
                    new { role = "system", content = SystemPrompt }
                };

                foreach (var msg in request.Messages)
                {
                    messages.Add(new { role = msg.Role.ToLower(), content = msg.Content });
                }

                var requestBody = new
                {
                    model = model,
                    messages = messages,
                    max_tokens = 1024,
                    temperature = 0.7
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.PostAsync(GroqApiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Groq API error {StatusCode}: {Body}", response.StatusCode, responseBody);
                    return new ResultDto<ChatResponseDto>(
                        $"Groq API returned {(int)response.StatusCode}: {responseBody}",
                        (int)response.StatusCode, false, new ChatResponseDto());
                }

                var jsonDoc = JsonNode.Parse(responseBody);
                var reply = jsonDoc?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? "";
                var promptTokens = jsonDoc?["usage"]?["prompt_tokens"]?.GetValue<int>() ?? 0;
                var completionTokens = jsonDoc?["usage"]?["completion_tokens"]?.GetValue<int>() ?? 0;
                var totalTokens = jsonDoc?["usage"]?["total_tokens"]?.GetValue<int>() ?? 0;

                var result = new ChatResponseDto
                {
                    Reply = reply,
                    PromptTokens = promptTokens,
                    CompletionTokens = completionTokens,
                    TotalTokens = totalTokens
                };

                return new ResultDto<ChatResponseDto>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GroqAiChatService");
                return new ResultDto<ChatResponseDto>("An unexpected error occurred.", 500, false, new ChatResponseDto());
            }
        }
    }
}
