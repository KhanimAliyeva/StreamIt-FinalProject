using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TokenDtos;
using STREAMIT.Business.Dtos.UserDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

    }

    // GET: /Account/Login
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("api/Auth/Login", dto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ResultDto>() ?? new ResultDto { Message = "Login failed" };
            ModelState.AddModelError(string.Empty, error.Message);
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        var result = await response.Content.ReadFromJsonAsync<ResultDto<AccessTokenDto>>();
        if (result == null || !result.IsSucceed || result.Data == null)
        {
            ModelState.AddModelError(string.Empty, result?.Message ?? "Login failed");
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        var token = result.Data.Token;
        var refresh = result.Data.RefreshToken;

        Response.Cookies.Append("AccessToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,                 // ⚠️ VACIB
            SameSite = SameSiteMode.None,  // ⚠️ VACIB
            Expires = result.Data.ExpiredDate,
            Path = "/",
            IsEssential = true
        });

        var refreshOpts = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,              
            SameSite = SameSiteMode.None,
            Expires = result.Data.RefreshTokenExpiredDate,
            Path = "/",
            IsEssential = true
        };

        Response.Cookies.Append("RefreshToken", refresh, refreshOpts);

        var claims = ExtractClaimsFromJwt(token).ToList();
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = result.Data.ExpiredDate.ToUniversalTime()
        });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

    
        if(dto.EmailOrUsername=="admin" || dto.EmailOrUsername=="admin@gmail.com" && dto.Password == "Admin123!")
        {
            return RedirectToAction("Dashboard", "Admin");

        }

        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Register
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("api/Auth/Register", dto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ResultDto>() ?? new ResultDto { Message = "Registration failed" };
            ModelState.AddModelError(string.Empty, error.Message);
            return View(dto);
        }

        return View("EmailVerification");
    }

 
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            ViewBag.Message = "Invalid confirmation request.";
            return View();
        }
        // Use IHttpClientFactory and explicit port 7108 to call the API
        var client = _httpClientFactory.CreateClient();
        var url = $"https://localhost:7108/api/Auth/ConfirmEmail?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}";
        var response = await client.GetAsync(url);

        string content = await response.Content.ReadAsStringAsync();

        // Debug info: show everything we got
        ViewBag.DebugStatusCode = (int)response.StatusCode;
        ViewBag.DebugContent = content;

        ResultDto result;

        if (response.IsSuccessStatusCode)
        {
            try
            {
                result = JsonSerializer.Deserialize<ResultDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new ResultDto();
            }
            catch (JsonException)
            {
                // Treat common plain text responses as "success"
                if (!string.IsNullOrWhiteSpace(content) && content.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    result = new ResultDto { Message = "Email confirmed successfully", IsSucceed = true };
                }
                else
                {
                    // Fallback: show raw content
                    result = new ResultDto { Message = $"API returned: {content}" };
                }
            }
        }
        else
        {
            result = new ResultDto { Message = $"API returned error {(int)response.StatusCode}: {content}" };
        }

        ViewBag.Message = result.Message ?? "Email confirmation result unknown.";

        // Optional: pass raw API data to the view for debugging
        ViewBag.RawResponse = content;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    private IEnumerable<Claim> ExtractClaimsFromJwt(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var claims = new List<Claim>();

            var userId = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.NameIdentifier ||
                x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            var userName = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.Name ||
                x.Type == "Username")?.Value;

            var fullName = token.Claims.FirstOrDefault(x =>
                x.Type == "FullName" ||
                x.Type == "Fullname")?.Value;

            var email = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.Email ||
                x.Type == JwtRegisteredClaimNames.Email ||
                x.Type == "Email")?.Value;

            var role = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.Role ||
                x.Type == "Role" ||
                x.Type == "role")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

            if (!string.IsNullOrWhiteSpace(userName))
                claims.Add(new Claim(ClaimTypes.Name, userName));

            if (!string.IsNullOrWhiteSpace(fullName))
                claims.Add(new Claim("FullName", fullName));

            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(ClaimTypes.Email, email));

            if (!string.IsNullOrWhiteSpace(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }
        catch
        {
            return Array.Empty<Claim>();
        }
    }
}