using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.SliderDtos;
using STREAMIT.MVC.Areas.Admin.ViewModels;
using System.Net.Http.Json;

[Area("Admin")]
public class SliderController : Controller
{
    private readonly HttpClient _httpClient;

    public SliderController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    #region INDEX
    public async Task<IActionResult> Index(int page = 1)
    {
        var sliders = await SafeGetListFromApi<SliderDto>("api/Slider");

        int pageSize = 5;
        int totalCount = sliders.Count;

        var pagedSliders = sliders
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new PagedSliderViewModel
        {
            Sliders = pagedSliders,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return View(model);
    }
    #endregion

    #region CREATE
    public async Task<IActionResult> Create()
    {
        ViewBag.Movies = await SafeGetListFromApi<GetMovieDto>("api/Movie");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSliderDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Movies = await SafeGetListFromApi<GetMovieDto>("api/Movie");
            return View(dto);
        }

        var resp = await _httpClient.PostAsJsonAsync("api/Slider", dto);
        var content = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.ApiErrors = content;
            ViewBag.Movies = await SafeGetListFromApi<GetMovieDto>("api/Movie");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }
    #endregion

    #region EDIT
    public async Task<IActionResult> Edit(int id)
    {
        var slider = await SafeGetFromApi<UpdateSliderDto>($"api/Slider/{id}");
        if (slider == null) return NotFound();

        ViewBag.Movies = await SafeGetListFromApi<GetMovieDto>("api/Movie");
        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSliderDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Movies = await SafeGetListFromApi<GetMovieDto>("api/Movie");
            return View(dto);
        }

        var resp = await _httpClient.PutAsJsonAsync("api/Slider", dto);
        var content = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.ApiErrors = content;
            ViewBag.Movies = await SafeGetListFromApi<GetMovieDto>("api/Movie");
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
        var resp = await _httpClient.DeleteAsync($"api/Slider/{id}");
        var result = await SafeReadJson<ResultDto>(resp);

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
                if (result?.Data != null) return result.Data;
            }
            catch { }

            if (content.TrimStart().StartsWith("{"))
            {
                var j = JObject.Parse(content);
                var dataToken = j["data"];

                if (dataToken != null && dataToken.Type == JTokenType.Array)
                    return dataToken.ToObject<List<T>>() ?? new List<T>();

                if (dataToken != null && dataToken.Type == JTokenType.Object && dataToken["$values"] != null)
                    return dataToken["$values"]!.ToObject<List<T>>() ?? new List<T>();
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