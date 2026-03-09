using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.TagDtos;
using System.Net.Http.Json;

namespace STREAMIT.MVC.Controllers
{
    public class TagController : Controller
    {
        private readonly HttpClient _httpClient;

        public TagController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/tag");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                // debug üçün: status və body-ni gör
                return Content($"Status: {(int)response.StatusCode}\n\n{body}", "text/plain");
            }

            var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>(
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(tags ?? new List<TagDto>());
        }
    }
}