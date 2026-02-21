using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using System.Net.Http.Json;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly HttpClient _httpClient;

        public GenreController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        #region INDEX
        public async Task<IActionResult> Index()
        {
            var result = await SafeGetFromJson<ResultDto<List<GetGenreDto>>>("Genre");
            var genres = result?.Data ?? new List<GetGenreDto>();
            return View(genres);
        }
        #endregion

        #region CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGenreDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            // post to API controller route (api/Genre)
            var response = await _httpClient.PostAsJsonAsync("api/Genre", dto);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // show API response for easier debugging
                ViewBag.RawResponse = content;
                ViewBag.DebugStatusCode = (int)response.StatusCode;
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {content}");
                return View(dto);
            }

            // Success — redirect. API may return a non-generic ResultDto or a generic one; we don't require the payload here.
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region UPDATE
        public async Task<IActionResult> Edit(int id)
        {
            // API returns GetGenreDto for GET by id; request that and map to UpdateGenreDto for the edit form
            var getResult = await SafeGetFromJson<ResultDto<GetGenreDto>>($"Genre/{id}");
            if (getResult == null || getResult.Data == null) return NotFound();

            var updateDto = new UpdateGenreDto
            {
                Id = getResult.Data.Id,
                Name = getResult.Data.Name
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateGenreDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            // API expects PUT to /api/Genre with the DTO in body
            var response = await _httpClient.PutAsJsonAsync($"api/Genre", dto);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.RawResponse = content;
                ViewBag.DebugStatusCode = (int)response.StatusCode;
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {content}");
                return View(dto);
            }

            // Success — redirect
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Genre/{id}");

            var result = await SafeReadJson<ResultDto>(response);

            if (result == null || !result.IsSucceed)
                return NotFound(result?.Message ?? "Something went wrong...");

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region HELPERS
        private async Task<T?> SafeGetFromJson<T>(string endpoint)
        {
            try
            {
                // use relative api path so client BaseAddress is respected
                var response = await _httpClient.GetAsync($"api/{endpoint}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                return default;
            }
        }

        private async Task<T?> SafeReadJson<T>(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                return default;
            }
        }
        #endregion
    }
}