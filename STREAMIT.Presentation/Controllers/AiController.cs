using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.AiDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Services.Abstractions;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IAiChatService _aiChatService;

    public AiController(IAiChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    /// <summary>
    /// POST api/ai/chat
    /// Body: { "messages": [ { "role": "user", "content": "Hello!" } ] }
    /// Returns the assistant's reply.
    /// Requires JWT Bearer auth.
    /// </summary>
    [HttpPost("chat")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<ResultDto<ChatResponseDto>>> Chat([FromBody] ChatRequestDto request)
    {
        if (request.Messages == null || request.Messages.Count == 0)
            return BadRequest(new ResultDto("Messages cannot be empty.", 400, false));

        var result = await _aiChatService.SendMessageAsync(request);

        if (!result.IsSucceed)
            return StatusCode(result.StatusCode, result);

        return Ok(result);
    }
}
