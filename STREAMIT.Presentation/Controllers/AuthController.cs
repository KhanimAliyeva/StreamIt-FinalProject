using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TokenDtos;
using STREAMIT.Business.Dtos.UserDtos;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

    [NonAction]
    public string CreateJwtToken(string userId, string fullName, string email, string role,string username,
    IConfiguration config)
    {
        var jwt = config.GetSection("JWTOptions");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!));

        var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, userId),
    new Claim(ClaimTypes.NameIdentifier, userId),
    new Claim(ClaimTypes.Name, username ?? "User"),
    new Claim("FullName", fullName ?? ""),
    new Claim(JwtRegisteredClaimNames.Email, email ?? ""),
    new Claim(ClaimTypes.Role, role ?? "User")
};

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

