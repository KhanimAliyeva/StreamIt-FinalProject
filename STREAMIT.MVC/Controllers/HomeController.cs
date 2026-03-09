using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.SliderDtos;
using STREAMIT.Business.Dtos.WatchHistoryDtos;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IActionResult> Index()
    {
        var errors = new List<string>();

        var sliders = await GetListAsync<SliderDto>("api/slider", errors);
        ViewBag.UpcomingMovies = await GetListAsync<GetMovieDto>("api/movie/upcoming", errors);
        ViewBag.BestInTv = await GetListAsync<GetMovieDto>("api/movie/bestintv", errors);
        ViewBag.Latest = await GetListAsync<GetMovieDto>("api/movie/latest", errors);
        ViewBag.Slider2 = await GetListAsync<Slider2Dto>("api/slider2", errors);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            ViewBag.SuggestedMovies =
                await GetListAsync<SuggestedMovieDto>($"api/movie/suggested/{userId}", errors)
                ?? new List<SuggestedMovieDto>();
        }
        else
        {
            ViewBag.SuggestedMovies =
                await GetListAsync<SuggestedMovieDto>("api/movie/latest", errors)
                ?? new List<SuggestedMovieDto>();
        }
        if (!string.IsNullOrWhiteSpace(userId))
        {
            ViewBag.ContinueWatching =
                await GetListAsync<ContinueWatchingDto>($"api/watchhistory/continue-watching/{userId}", errors)
                ?? new List<ContinueWatchingDto>();
        }
        else
        {
            ViewBag.ContinueWatching = new List<ContinueWatchingDto>();
        }

        var featuredMovies = await GetListAsync<GetMovieDto>("api/movie/latest", errors);
        ViewBag.FeaturedMovie = featuredMovies?.FirstOrDefault();

        ViewBag.ApiErrors = errors;
        return View(sliders ?? new List<SliderDto>());
    }

    private async Task<List<T>?> GetListAsync<T>(string endpoint, List<string> errors, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _httpClient.GetAsync(endpoint, cancellationToken);
            var content = await resp.Content.ReadAsStringAsync(cancellationToken);

            if (!resp.IsSuccessStatusCode)
            {
                errors.Add($"{endpoint}: {(int)resp.StatusCode} - {content}");
                return null;
            }

            if (string.IsNullOrWhiteSpace(content))
                return null;

            var firstChar = content.TrimStart()[0];

            if (firstChar == '{')
            {
                try
                {
                    var j = JObject.Parse(content);
                    JToken? dataToken = null;

                    if (j.TryGetValue("data", StringComparison.OrdinalIgnoreCase, out var d))
                        dataToken = d;
                    else if (j.TryGetValue("Data", StringComparison.OrdinalIgnoreCase, out var d2))
                        dataToken = d2;
                    else if (j.TryGetValue("result", StringComparison.OrdinalIgnoreCase, out var r))
                        dataToken = r;

                    if (dataToken != null && dataToken.Type != JTokenType.Null)
                    {
                        if (dataToken.Type == JTokenType.Array)
                            return dataToken.ToObject<List<T>>();

                        if (dataToken.Type == JTokenType.Object)
                        {
                            var singleItem = dataToken.ToObject<T>();
                            return singleItem != null ? new List<T> { singleItem } : null;
                        }
                    }

                    foreach (var prop in j.Properties())
                    {
                        if (prop.Value.Type == JTokenType.Array)
                            return prop.Value.ToObject<List<T>>();
                    }

                    errors.Add($"{endpoint}: unexpected JSON object returned");
                    return null;
                }
                catch (JsonException ex)
                {
                    errors.Add($"{endpoint}: json parse error: {ex.Message}");
                    return null;
                }
            }

            return JsonConvert.DeserializeObject<List<T>>(content);
        }
        catch (Exception ex)
        {
            errors.Add($"{endpoint}: {ex.Message}");
            return null;
        }
    }
}