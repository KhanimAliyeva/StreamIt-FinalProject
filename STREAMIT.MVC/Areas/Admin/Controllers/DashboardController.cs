using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.DashboardDtos;
using System.Text.Json;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/dashboard");

            if (!response.IsSuccessStatusCode)
            {
                return View(new GetDashboardDto());
            }

            var json = await response.Content.ReadAsStringAsync();

            var model = JsonSerializer.Deserialize<GetDashboardDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(model ?? new GetDashboardDto());
        }
    }
}