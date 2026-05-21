using STREAMIT.Business.Dtos.AiDtos;
using STREAMIT.Business.Dtos.ResultDtos;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface IAiChatService
    {
        /// <summary>
        /// Sends the full conversation history to Groq and returns the assistant reply.
        /// </summary>
        Task<ResultDto<ChatResponseDto>> SendMessageAsync(ChatRequestDto request);
    }
}
