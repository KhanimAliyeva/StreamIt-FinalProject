using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using STREAMIT.Business.Dtos.WatchlistDtos;

namespace STREAMIT.MVC.Controllers
{
    public class WatchListController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WatchListController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var resp = await client.GetAsync("api/watchlist");

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Account");

            resp.EnsureSuccessStatusCode();

            var list = await resp.Content.ReadFromJsonAsync<List<GetWatchListDto>>();
            return View(list ?? new List<GetWatchListDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int movieId)
        {
            if (movieId <= 0) return BadRequest(new { message = "Invalid movieId" });

            var client = _httpClientFactory.CreateClient("ApiClient");
            var resp = await client.PostAsJsonAsync("api/watchlist/toggle", new { movieId });

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Unauthorized(new { message = "Please login." });

            var text = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, text);

            return Content(text, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var resp = await client.GetAsync("api/watchlist");
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode, body);
            return Content(body, "application/json");
        }
    }

}