using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace STREAMIT.MVC.Controllers
{
    [Authorize]
    public class AiChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AiChatController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET /AiChat  — renders the chat page
        public IActionResult Index()
        {
            return View();
        }

        // POST /AiChat/Send — called by the chat UI via fetch
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ChatPayload payload)
        {
            if (payload?.Messages == null || payload.Messages.Count == 0)
                return BadRequest(new { error = "Empty messages" });

            var client = _httpClientFactory.CreateClient("ApiClient");

            // Forward the JWT cookie token to the API
            var token = Request.Cookies["AccessToken"];
            if (!string.IsNullOrWhiteSpace(token))
            {
                var bearerValue = token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? token[7..].Trim()
                    : token.Trim();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerValue);
            }

            var json = JsonSerializer.Serialize(new { messages = payload.Messages });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/ai/chat", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { error = body });

            // Parse and return only the reply text to keep it simple
            using var doc = JsonDocument.Parse(body);
            var reply = doc.RootElement
                .GetProperty("data")
                .GetProperty("reply")
                .GetString() ?? "";

            return Ok(new { reply });
        }
    }

    public class ChatPayload
    {
        public List<MessageItem> Messages { get; set; } = new();
    }

    public class MessageItem
    {
        public string Role { get; set; } = "user";
        public string Content { get; set; } = string.Empty;
    }
}
