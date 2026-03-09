using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace STREAMIT.MVC.Controllers
{
    public class GenreController : Controller
    {
        private readonly HttpClient _httpClient;

        public GenreController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var result = await _httpClient
                .GetFromJsonAsync<ResultDto<List<GenreDto>>>("api/genre");

            if (result == null || !result.IsSucceed)
                return View(new List<GenreDto>());

            return View(result.Data);
        }
    }
}
