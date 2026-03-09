using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.MVC.Areas.Admin.ViewModels;
using System.Net.Http.Json;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LanguageController : Controller
    {
        private readonly HttpClient _httpClient;

        public LanguageController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        #region INDEX
        public async Task<IActionResult> Index(int page = 1)
        {
            var languages = await SafeGetListFromApi<GetLanguageDto>("api/Language");

            int pageSize = 5;
            int totalCount = languages.Count;

            var pagedLanguages = languages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedLanguageViewModel
            {
                Languages = pagedLanguages,
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
        public async Task<IActionResult> Create(CreateLanguageDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PostAsJsonAsync("api/Language", dto);
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
            var language = await SafeGetFromApi<GetLanguageDto>($"api/Language/{id}");
            if (language == null) return NotFound();

            var updateDto = new UpdateLanguageDto
            {
                Id = language.Id,
                Name = language.Name,
                Code = language.Code
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateLanguageDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PutAsJsonAsync("api/Language", dto);
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
            var response = await _httpClient.DeleteAsync($"api/Language/{id}");
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
                    var result = JsonConvert.DeserializeObject<ResultDto<List<T>>>(content);
                    if (result != null && result.Data != null)
                        return result.Data;
                }
                catch { }

                if (content.TrimStart().StartsWith("{"))
                {
                    var j = JObject.Parse(content);
                    var dataToken = j["data"];

                    if (dataToken != null && dataToken.Type == JTokenType.Array)
                        return dataToken.ToObject<List<T>>() ?? new List<T>();
                }

                return JsonConvert.DeserializeObject<List<T>>(content) ?? new List<T>();
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
                    var result = JsonConvert.DeserializeObject<ResultDto<T>>(content);
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

                return JsonConvert.DeserializeObject<T>(content);
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