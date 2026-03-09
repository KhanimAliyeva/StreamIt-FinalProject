using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

namespace STREAMIT.MVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _http;

        public ProfileController(IHttpClientFactory http)
        {
            _http = http;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> MyMembership()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "User email not found" });

            var client = _http.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/membership/user-membership?email={Uri.EscapeDataString(email)}");

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, json);

            return Content(json, "application/json");
        }
        // Səhifə
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        // View açılan kimi məlumatı gətiririk (JS bunu çağırır)
        [HttpGet]
        public async Task<IActionResult> Me()
        {
            var client = _http.CreateClient("ApiClient");

            var resp = await client.GetAsync("api/user/me");

            if (resp.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, body);

            return Content(body, "application/json");
        }

        // Save
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateProfileVm vm)
        {
            if (vm == null) return BadRequest("Invalid payload");

            var client = _http.CreateClient("ApiClient");

            var resp = await client.PutAsJsonAsync("api/user/me", new
            {
                fullName = vm.FullName,
                userName = vm.UserName,
                email = vm.Email
            });

            if (resp.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            var text = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, text);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "Member";

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId ?? ""),
        new Claim(ClaimTypes.Name, vm.UserName ?? "User"),
        new Claim("FullName", vm.FullName ?? ""),
        new Claim(ClaimTypes.Email, vm.Email ?? ""),
        new Claim(ClaimTypes.Role, role)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true
                });

            return Ok(new { ok = true });
        }
    }

    public class UpdateProfileVm
    {
        public string FullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
    }
}