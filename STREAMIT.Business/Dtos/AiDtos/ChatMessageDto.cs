namespace STREAMIT.Business.Dtos.AiDtos
{
    public class ChatMessageDto
    {
        public string Role { get; set; } = "user";   // "user" | "assistant" | "system"
        public string Content { get; set; } = string.Empty;
    }

    public class ChatRequestDto
    {
        /// <summary>
        /// Full conversation history (system + previous turns + new user message)
        /// </summary>
        public List<ChatMessageDto> Messages { get; set; } = new();
    }

    public class ChatResponseDto
    {
        public string Reply { get; set; } = string.Empty;
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
