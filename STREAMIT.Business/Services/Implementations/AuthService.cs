using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TokenDtos;
using STREAMIT.Business.Dtos.UserDtos;
using STREAMIT.Business.Exceptions;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using System.Net;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IJWTService _jWTService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<AppUser> userManager, IMapper mapper, IJWTService jWTService, IEmailService emailService, IConfiguration configuration)
    {
        _userManager = userManager;
        _mapper = mapper;
        _jWTService = jWTService;
        _emailService = emailService;
        _configuration = configuration;
    }

    // Register
    public async Task<ResultDto> RegisterAsync(RegisterDto dto)
    {
        // 1. Check existing username/email
        if (await _userManager.Users.AnyAsync(x => x.UserName!.ToLower() == dto.UserName.ToLower()))
            return new ResultDto("Username already exists");

        if (await _userManager.Users.AnyAsync(x => x.Email!.ToLower() == dto.Email.ToLower()))
            return new ResultDto("Email already exists");

        // 2. Create user
        var user = _mapper.Map<AppUser>(dto);
        user.EmailConfirmed = false; // mütləq false olmalı
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ResultDto(errors);
        }

        await _userManager.AddToRoleAsync(user, "Member");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Base64Url encode
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var mvcBase = _configuration["MvcBaseUrl"]?.TrimEnd('/') ?? "https://localhost:7107";
        var confirmationLink = $"{mvcBase}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";
        var emailHtml = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Confirm Your Email</title>
</head>
<body style='margin:0; padding:0; font-family: Arial, sans-serif; background-color:#ffffff;'>
    <table width='100%' cellpadding='0' cellspacing='0'>
        <tr>
            <td align='center' style='padding: 60px 0;'>
                <table width='600' cellpadding='0' cellspacing='0' style='
                        background: linear-gradient(135deg, #1a1a1a 0%, #000000 100%);
                        border-radius:10px; 
                        overflow:hidden; 
                        box-shadow:0 4px 20px rgba(0,0,0,0.3);
                        text-align:center;
                    '>
                    <!-- Header Image -->
                    <tr>
                        <td style='padding: 40px; background-color:#000;'>
                            <img src='https://res.cloudinary.com/dgpf0kufs/image/upload/v1771527578/d23d4a1b-89e1-495f-b429-7e3cf2248c0a.png' 
                                 alt='Logo' width='120' style='display:block; border:none; margin:auto;'/>
                        </td>
                    </tr>

                    <!-- Greeting -->
                    <tr>
                        <td style='padding: 30px 20px; color:#fff;'>
                            <h2 style='color:#e53935; margin:0;'>Welcome, {user.Fullname}!</h2>
                            <p style='color:#fff; font-size:16px; margin-top:10px;'>Click the button below to confirm your email address and activate your account.</p>
                        </td>
                    </tr>

                    <!-- Confirm Button -->
                    <tr>
                        <td style='padding: 20px;'>
                            <a href='{confirmationLink}' 
                               style='
                                    background-color:#e53935; 
                                    color:#fff; 
                                    text-decoration:none; 
                                    padding:15px 30px; 
                                    border-radius:5px; 
                                    font-weight:bold; 
                                    display:inline-block; 
                                    font-size:16px;
                                    transition: all 0.3s ease;
                               '
                               onmouseover=""this.style.backgroundColor='#b71c1c'""
                               onmouseout=""this.style.backgroundColor='#e53935'"">
                               Confirm Email
                            </a>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='padding: 20px; color:#888; font-size:12px;'>
                            <p style='margin:0;'>If you did not create an account, you can safely ignore this email.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        await _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailHtml);
        return new ResultDto { Message = "Registered successfully. Check your email to confirm." };
    }

    public async Task<ResultDto> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new ResultDto("User not found");

        // Base64Url decode
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
            return new ResultDto(string.Join(",", result.Errors.Select(x => x.Description)));

        return new ResultDto("Email confirmed successfully");
    }
    // Login
    public async Task<ResultDto<AccessTokenDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.EmailOrUsername)
                    ?? await _userManager.FindByNameAsync(dto.EmailOrUsername);

        if (user == null) throw new Exception("Invalid credentials");

        if (!user.EmailConfirmed) throw new Exception("Email not confirmed");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new Exception("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.UserName ?? "User"),
    new Claim("FullName", user.Fullname ?? ""),
    new Claim(ClaimTypes.Email, user.Email ?? ""),
    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "Member")
};
        var tokenResult = _jWTService.CreateAccessToken(claims);

        user.RefreshToken = tokenResult.RefreshToken;
        user.RefreshTokenExpiredDate = tokenResult.RefreshTokenExpiredDate;
        await _userManager.UpdateAsync(user);

        return new ResultDto<AccessTokenDto>(tokenResult);
    }

    public async Task<ResultDto<AccessTokenDto>> RefreshTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new LoginFailException("Token is null or empty");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == token && x.RefreshTokenExpiredDate.HasValue && x.RefreshTokenExpiredDate > DateTime.UtcNow);

        if (user is null)
            throw new LoginFailException("Invalid or expired refresh token");

        // Yeni AccessToken + RefreshToken yarat
        var tokenResult = await _GetAccessTokenAsync(user);

        return new ResultDto<AccessTokenDto>(tokenResult);
    }

    private async Task<AccessTokenDto> _GetAccessTokenAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.UserName ?? "User"),
    new Claim("FullName", user.Fullname ?? ""),
    new Claim(ClaimTypes.Email, user.Email ?? ""),
    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "Member")
};

        var tokenResult = _jWTService.CreateAccessToken(claims);

        // DB-yə refresh token yaz
        user.RefreshToken = tokenResult.RefreshToken;
        user.RefreshTokenExpiredDate = tokenResult.RefreshTokenExpiredDate;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            throw new Exception("Failed to update refresh token");

        return tokenResult;
    }


}
