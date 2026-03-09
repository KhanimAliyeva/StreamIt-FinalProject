using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.MVC.Areas.Admin.ViewModels;
using System.Net.Http.Json;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly HttpClient _httpClient;

        public TagController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        #region INDEX
        public async Task<IActionResult> Index(int page = 1)
        {
            var tags = await SafeGetListFromApi<GetTagDto>("api/Tag");

            int pageSize = 5;
            int totalCount = tags.Count;

            var pagedTags = tags
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedTagViewModel
            {
                Tags = pagedTags,
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
        public async Task<IActionResult> Create(CreateTagDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PostAsJsonAsync("api/Tag", dto);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.RawResponse = content;
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {content}");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await SafeGetFromApi<GetTagDto>($"api/Tag/{id}");
            if (tag == null) return NotFound();

            var updateDto = new UpdateTagDto
            {
                Id = tag.Id,
                Name = tag.Name
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTagDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PutAsJsonAsync($"api/Tag/{dto.Id}", dto);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.RawResponse = content;
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
            var response = await _httpClient.DeleteAsync($"api/Tag/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                TempData["DeleteError"] = $"Delete failed: {content}";
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
        #endregion
    }
}