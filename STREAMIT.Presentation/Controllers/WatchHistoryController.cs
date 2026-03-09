using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.WatchHistoryDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;

[Route("api/[controller]")]
[ApiController]
public class WatchHistoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public WatchHistoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateWatchHistoryDto dto)
    {
        if (dto == null)
            return BadRequest("Dto is null.");

        if (dto.MovieId <= 0)
            return BadRequest("MovieId is required.");

        if (dto.Duration <= 0)
            return BadRequest("Duration must be greater than zero.");

        if (string.IsNullOrWhiteSpace(dto.UserId))
            return BadRequest("UserId is required.");

        var history = await _context.WatchHistories
            .FirstOrDefaultAsync(x => x.AppUserId == dto.UserId && x.MovieId == dto.MovieId);

        bool isCompleted = dto.WatchedMinutes >= dto.Duration * 0.95;

        if (history == null)
        {
            history = new WatchHistory
            {
                AppUserId = dto.UserId,
                MovieId = dto.MovieId,
                WatchedMinutes = dto.WatchedMinutes,
                Duration = dto.Duration,
                IsCompleted = isCompleted,
                LastWatchedAt = DateTime.UtcNow
            };

            _context.WatchHistories.Add(history);
        }
        else
        {
            if (dto.WatchedMinutes > history.WatchedMinutes)
            {
                history.WatchedMinutes = dto.WatchedMinutes;
            }

            history.Duration = dto.Duration;
            history.IsCompleted = isCompleted;
            history.LastWatchedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Watch history updated" });
    }

    [HttpGet("continue-watching/{userId}")]
    public async Task<IActionResult> ContinueWatching(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("UserId is required.");

        var data = await _context.WatchHistories
            .Where(x => x.AppUserId == userId &&
                        !x.IsCompleted &&
                        x.WatchedMinutes > 0)
            .OrderByDescending(x => x.LastWatchedAt)
            .Select(x => new ContinueWatchingDto
            {
                MovieId = x.MovieId,
                Title = x.Movie.Title,
                PosterUrl = x.Movie.PosterUrl,
                WatchedMinutes = x.WatchedMinutes,
                Duration = x.Duration,
                ProgressPercent = x.Duration == 0 ? 0 : x.WatchedMinutes * 100 / x.Duration
            })
            .Take(12)
            .ToListAsync();

        return Ok(data);
    }
}