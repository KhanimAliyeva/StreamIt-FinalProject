using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.MVC.Areas.Admin.ViewModels;
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
        public async Task<IActionResult> Index(int page = 1)
        {
            var genres = await SafeGetListFromApi<GetGenreDto>("api/Genre");

            int pageSize = 5;
            int totalCount = genres.Count;

            var pagedGenres = genres
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedGenreViewModel
            {
                Genres = pagedGenres,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }
        #endregion

        #region CREATE
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGenreDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PostAsJsonAsync("api/Genre", dto);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.RawResponse = content;
                ViewBag.DebugStatusCode = (int)response.StatusCode;
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {content}");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var genre = await SafeGetFromApi<GetGenreDto>($"api/Genre/{id}");
            if (genre == null) return NotFound();

            var updateDto = new UpdateGenreDto
            {
                Id = genre.Id,
                Name = genre.Name
            };

            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateGenreDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PutAsJsonAsync("api/Genre", dto);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.RawResponse = content;
                ViewBag.DebugStatusCode = (int)response.StatusCode;
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {content}");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Genre/{id}");
            var result = await SafeReadJson<ResultDto>(response);

            if (result == null || !result.IsSucceed)
            {
                TempData["DeleteError"] = result?.Message ?? "Something went wrong...";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region HELPERS
        private async Task<List<T>> SafeGetListFromApi<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode) return new List<T>();

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return new List<T>();

                try
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<List<T>>>(content);
                    if (result != null && result.Data != null) return result.Data;
                }
                catch { }

                if (content.TrimStart().StartsWith("{"))
                {
                    var j = JObject.Parse(content);
                    JToken? dataToken = j["data"];
                    if (dataToken != null && dataToken.Type == JTokenType.Array)
                        return dataToken.ToObject<List<T>>() ?? new List<T>();
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(content) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        private async Task<T?> SafeGetFromApi<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode) return default;

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return default;

                try
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<T>>(content);
                    if (result != null && result.Data != null)
                        return result.Data;
                }
                catch { }

                if (content.TrimStart().StartsWith("{"))
                {
                    var j = JObject.Parse(content);
                    var dataToken = j["data"];
                    if (dataToken != null && dataToken.Type == JTokenType.Object)
                        return dataToken.ToObject<T>();
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
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