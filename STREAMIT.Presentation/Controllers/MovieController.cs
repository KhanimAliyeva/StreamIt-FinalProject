using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Business.Services.Implementations;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController(IMovieService _service,IMapper _mapper, AppDbContext _context, ILogger<MovieController> _logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _service.GetAllWithDetailsAsync();
            var dtos = _mapper.Map<List<GetMovieDto>>(movies);
            return Ok(dtos);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateMovieDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result == null)
                return StatusCode(500, new ResultDto { IsSucceed = false, Message = "Unknown error", StatusCode = 500 });

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result == null)
                return StatusCode(500, new ResultDto { IsSucceed = false, Message = "Unknown error", StatusCode = 500 });

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _service.GetByIdAsync(id);
            return Ok(movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm]UpdateMovieDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            if (result == null)
                return StatusCode(500, new ResultDto { IsSucceed = false, Message = "Unknown error", StatusCode = 500 });

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming()
        {
            var result = await _service.GetUpcomingMoviesAsync();
            return Ok(result);
        }

        [HttpPost("increment-view/{id}")]
        public async Task<IActionResult> IncrementView(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieStatistics)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            if (movie.MovieStatistics == null)
            {
                movie.MovieStatistics = new MovieStatistics
                {
                    MovieId = id,
                    ViewCount = 0,
                    LikeCount = 0
                };
                _context.Add(movie.MovieStatistics);
            }

            movie.MovieStatistics.ViewCount += 1;
            await _context.SaveChangesAsync();

            return Ok(new { viewCount = movie.MovieStatistics.ViewCount });
        }
        [HttpGet("bestintv")]
        public async Task<IActionResult> GetBestInTv()
        {
            return Ok(await _service.GetBestInTvAsync());
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            return Ok(await _service.GetLatestMoviesAsync());
        }

        [HttpGet("suggested/{userId}")]
        public async Task<IActionResult> GetSuggestedMovies(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("UserId is required.");

            var watchedMovieIds = await _context.WatchHistories
                .Where(x => x.AppUserId == userId)
                .Select(x => x.MovieId)
                .Distinct()
                .ToListAsync();

            var favoriteGenreIds = await _context.WatchHistories
                .Where(x => x.AppUserId == userId)
                .SelectMany(x => x.Movie.MovieGenres.Select(mg => mg.GenreId))
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToListAsync();

            List<SuggestedMovieDto> movies;

            if (favoriteGenreIds.Any())
            {
                movies = await _context.Movies
                    .Where(m => !watchedMovieIds.Contains(m.Id) &&
                                m.MovieGenres.Any(mg => favoriteGenreIds.Contains(mg.GenreId)))
                    .OrderByDescending(m => m.MovieStatistics.ViewCount)
                    .Select(m => new SuggestedMovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        PosterUrl = m.PosterUrl,
                        CoverImageUrl = m.PosterUrl,
                        Imdb = m.AverageRating
                    })
                    .Take(12)
                    .ToListAsync();
            }
            else
            {
                movies = await _context.Movies
                    .OrderByDescending(m => m.MovieStatistics.ViewCount)
                    .Select(m => new SuggestedMovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        PosterUrl = m.PosterUrl,
                        CoverImageUrl = m.PosterUrl,
                        Imdb = m.AverageRating
                    })
                    .Take(12)
                    .ToListAsync();
            }

            return Ok(movies);
        }
    }
}
