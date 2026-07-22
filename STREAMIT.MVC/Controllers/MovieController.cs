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
            var principalUserId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(principalUserId))
                dto.UserId = principalUserId;

            var response = await _httpClient.PostAsJsonAsync("api/reviews", dto);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var body = await response.Content.ReadAsStringAsync();
            return Content(body, "application/json");
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
            var response = await _httpClient.GetAsync($"api/movie/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var movie = JsonConvert.DeserializeObject<GetMovieDto>(json);

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
                    movie.IsInWatchList = false; 
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
            _logger.LogInformation("IncrementView called for movie {movieId}", id);
            var resp = await _httpClient.PostAsync($"api/movie/increment-view/{id}", null);
            var content = await resp.Content.ReadAsStringAsync();

            _logger.LogInformation("IncrementView: API response status {status}. Content: {content}", resp.StatusCode, content);

            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, content);

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

        [HttpGet]
        public async Task<IActionResult> GetReviews(int movieId)
        {
            var resp = await _httpClient.GetAsync($"api/reviews/movie/{movieId}");
            var body = await resp.Content.ReadAsStringAsync();
            return Content(body, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> PostReply([FromBody] AddReplyDto dto)
        {
            AttachToken();
            var resp = await _httpClient.PostAsJsonAsync("api/reviews/reply", dto);
            var body = await resp.Content.ReadAsStringAsync();
            return StatusCode((int)resp.StatusCode, body);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int reviewId)
        {
            AttachToken();
            var resp = await _httpClient.PostAsync($"api/reviews/{reviewId}/like", null);
            var body = await resp.Content.ReadAsStringAsync();
            return StatusCode((int)resp.StatusCode, body);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            AttachToken();
            var resp = await _httpClient.DeleteAsync($"api/reviews/{reviewId}");
            return StatusCode((int)resp.StatusCode);
        }

        [HttpGet]
        public IActionResult Token()
        {
            var token = Request.Cookies["AccessToken"];
            return Content(token ?? string.Empty);
        }

        private void AttachToken()
        {
            var token = Request.Cookies["AccessToken"] ?? string.Empty;
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token[7..].Trim();
            if (!string.IsNullOrWhiteSpace(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

    }
}