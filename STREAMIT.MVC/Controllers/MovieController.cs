using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.ReviewDtos;
using STREAMIT.Business.Dtos.WatchHistoryDtos;
using STREAMIT.Business.Dtos.WatchlistDtos;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace STREAMIT.MVC.Controllers
{
    public class MovieController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IHttpClientFactory factory, ILogger<MovieController> logger)
        {
            _httpClient = factory.CreateClient("ApiClient");
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto)
        {
            // Server-side user id əlavə et
            var principalUserId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(principalUserId))
                dto.UserId = principalUserId;

            // Sadə PostAsJsonAsync istifadə et, token handler tərəfindən əlavə olunacaq
            var response = await _httpClient.PostAsJsonAsync("api/reviews", dto);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return Ok();
        }
        public async Task<IActionResult> Index()

        {
            var response = await _httpClient.GetAsync("api/movie");

            if (!response.IsSuccessStatusCode)
                return View(new List<GetMovieDto>());

            var json = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<GetMovieDto>>(json);

            return View(movies);
        }


        public async Task<IActionResult> Details(int id)
        {
            // movie detail
            var response = await _httpClient.GetAsync($"api/movie/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var movie = JsonConvert.DeserializeObject<GetMovieDto>(json);

            // reviews
            var reviewsResp = await _httpClient.GetAsync($"api/reviews/movie/{id}");
            if (reviewsResp.IsSuccessStatusCode)
            {
                var reviewsJson = await reviewsResp.Content.ReadAsStringAsync();
                movie.Reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(reviewsJson) ?? new();
            }
            else movie.Reviews = new();

            try
            {
                var wlResp = await _httpClient.GetAsync("api/watchlist");
                if (wlResp.IsSuccessStatusCode)
                {
                    var wlJson = await wlResp.Content.ReadAsStringAsync();
                    var wl = JsonConvert.DeserializeObject<List<GetWatchListDto>>(wlJson) ?? new();
                    movie.IsInWatchList = wl.Any(x => x.MovieId == id);
                }
                else
                {
                    movie.IsInWatchList = false; // 401 və s.
                }
            }
            catch
            {
                movie.IsInWatchList = false;
            }

            return View("Details", movie);
        }

        [HttpGet]
        [Route("Movie/Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            // Delegate to the Details action and ensure the same view name is used
            return await Details(id);
        }

        public async Task<IActionResult> Watch(int id)
        {
            var response = await _httpClient.GetAsync($"api/movie/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();
            var json = await response.Content.ReadAsStringAsync();
            var movie = JsonConvert.DeserializeObject<GetMovieDto>(json);
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWatchHistory([FromBody] UpdateWatchHistoryDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User is not logged in.");

            dto.UserId = userId;

            var response = await _httpClient.PostAsJsonAsync("api/WatchHistory/update", dto);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            return Ok(content);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> IncrementView(int id)
        {
            // Call Presentation API to increment view count for the movie
            _logger.LogInformation("IncrementView called for movie {movieId}", id);
            var resp = await _httpClient.PostAsync($"api/movie/increment-view/{id}", null);
            var content = await resp.Content.ReadAsStringAsync();

            _logger.LogInformation("IncrementView: API response status {status}. Content: {content}", resp.StatusCode, content);

            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, content);

            // Try to parse returned view count and forward it to the client
            try
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(content);
                int viewCount = obj?.viewCount ?? 0;
                return Ok(new { viewCount });
            }
            catch
            {
                return Ok();
            }
        }
    }
}