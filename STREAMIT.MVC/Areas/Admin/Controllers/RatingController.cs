using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.RatingDtos;
using STREAMIT.MVC.Areas.Admin.ViewModels;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RatingController : Controller
    {
        private readonly HttpClient _httpClient;

        public RatingController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index(string? filterCategory, int page = 1, int pageSize = 5)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Ratings");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return View(new PagedRatingViewModel());
                }

                List<RatingDto>? ratings = null;

                try
                {
                    if (!string.IsNullOrWhiteSpace(content) && content.TrimStart().StartsWith("{"))
                    {
                        var j = JObject.Parse(content);
                        JToken? dataToken = null;

                        if (j.TryGetValue("data", StringComparison.OrdinalIgnoreCase, out var d))
                            dataToken = d;
                        else if (j.Properties().Any(p => p.Value.Type == JTokenType.Array))
                            dataToken = j.Properties().First(p => p.Value.Type == JTokenType.Array).Value;

                        if (dataToken != null && dataToken.Type == JTokenType.Array)
                            ratings = dataToken.ToObject<List<RatingDto>>();
                    }
                    else
                    {
                        ratings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RatingDto>>(content);
                    }
                }
                catch
                {
                }

                ratings ??= new List<RatingDto>();

                if (!string.IsNullOrWhiteSpace(filterCategory))
                {
                    ratings = ratings
                        .Where(x => !string.IsNullOrWhiteSpace(x.Category) &&
                                    x.Category.Equals(filterCategory, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var totalCount = ratings.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                if (totalPages == 0) totalPages = 1;
                if (page < 1) page = 1;
                if (page > totalPages) page = totalPages;

                var pagedRatings = ratings
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var model = new PagedRatingViewModel
                {
                    Reviews = pagedRatings,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                return View(new PagedRatingViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Ratings/{id}");
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ViewBag.ApiErrors = await response.Content.ReadAsStringAsync();
                return BadRequest();
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ApiErrors = ex.Message;
                return BadRequest();
            }
        }
    }
}