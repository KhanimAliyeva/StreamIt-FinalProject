using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STREAMIT.Web.Controllers
{
    public class CastController : Controller
    {
        private readonly HttpClient _httpClient;

        public CastController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<PersonDto>>("api/person");

            return View(result);
        }
    }
}