using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Core.Entities;
using STREAMIT.Business.Dtos.UserDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Business.Dtos.UserDtos;
using STREAMIT.Business.Dtos.TokenDtos;
using STREAMIT.Business.Dtos.ResultDtos;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ResultDto>> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ResultDto { IsSucceed = false, Message = ex.Message });
        }
    }

    [HttpPost("Login")]
    public async Task<ActionResult<ResultDto<AccessTokenDto>>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var tokenResult = await _authService.LoginAsync(dto);
            return Ok(tokenResult);
        }
        catch (Exception ex)
        {
            return BadRequest(new ResultDto<AccessTokenDto> { IsSucceed = false, Message = ex.Message });
        }
    }

    [HttpPost("Refresh")]
    public async Task<ActionResult<ResultDto<AccessTokenDto>>> Refresh([FromBody] string refreshToken)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ResultDto<AccessTokenDto> { IsSucceed = false, Message = ex.Message });
        }
    }

    [HttpGet("ConfirmEmail")]
    public async Task<ActionResult<ResultDto>> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        try
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ResultDto { IsSucceed = false, Message = ex.Message });
        }
    }

}

