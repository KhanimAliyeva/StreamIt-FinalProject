using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.WatchlistDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using System.Security.Claims;

namespace STREAMIT.Presentation.Controllers;

[ApiController]
[Route("api/watchlist")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WatchListController : ControllerBase
{
    private readonly AppDbContext _context;
    public WatchListController(AppDbContext context) => _context = context;

    [HttpPost("toggle")]
    public async Task<IActionResult> Toggle([FromBody] WatchListDto dto)
    {
        if (dto == null || dto.MovieId <= 0) return BadRequest("Invalid movieId");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var movieExists = await _context.Movies.AnyAsync(m => m.Id == dto.MovieId);
        if (!movieExists) return NotFound("Movie not found");

        var exist = await _context.WatchLists
            .FirstOrDefaultAsync(x => x.MovieId == dto.MovieId && x.UserId == userId);

        if (exist != null)
        {
            _context.WatchLists.Remove(exist);
            await _context.SaveChangesAsync();
            return Ok(new { status = "removed" });
        }

        _context.WatchLists.Add(new WatchList { MovieId = dto.MovieId, UserId = userId });
        await _context.SaveChangesAsync();

        return Ok(new { status = "added" });
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWatchList()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var list = await _context.WatchLists
            .Where(x => x.UserId == userId)
            .Include(x => x.Movie)
            .Select(x => new GetWatchListDto
            {
                MovieId = x.MovieId,
                Title = x.Movie.Title,
                PosterUrl = x.Movie.PosterUrl
            })
            .ToListAsync();

        return Ok(list);
    }


}
