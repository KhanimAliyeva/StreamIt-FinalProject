using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("[controller]")]
public class CommunityController : Controller
{
    private readonly IHttpClientFactory _http;

    public CommunityController(IHttpClientFactory http) => _http = http;

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet("Token")]
    public IActionResult Token()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrWhiteSpace(token)) return Unauthorized();
        Response.Headers["Cache-Control"] = "no-store";
        return Content(token);
    }


}