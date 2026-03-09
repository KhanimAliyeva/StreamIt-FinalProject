using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.ReviewDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(AppDbContext context, ILogger<ReviewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Debug endpoint to help diagnose authentication issues. Returns whether Authorization header
    // was received and the validated claims. Call with a Bearer token to verify validation.
    [HttpGet("debug")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Debug()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        IEnumerable<object> claims = User?.Claims?.Select(c => (object)new { c.Type, c.Value }) ?? Enumerable.Empty<object>();
        return Ok(new { AuthorizationHeader = !string.IsNullOrEmpty(authHeader), Claims = claims });
    }
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddReview(AddReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        // 1 dəfə review qaydası
        var exists = await _context.ReviewMovies
            .AnyAsync(r => r.MovieId == dto.MovieId && r.UserId == userId);

        if (exists)
            return Conflict("You already reviewed this movie."); // 409

        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == dto.MovieId);
        if (movie == null) return BadRequest("Movie not found.");

        var review = new ReviewMovie
        {
            MovieId = dto.MovieId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment ?? ""
        };

        _context.ReviewMovies.Add(review);

        // AverageRating + RatingCount inkremental yeniləmə
        var oldCount = movie.RatingCount;
        var newCount = oldCount + 1;

        movie.AverageRating = ((movie.AverageRating * oldCount) + dto.Rating) / newCount;
        movie.RatingCount = newCount;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("movie/{movieId}")]
    public async Task<IActionResult> GetByMovie(int movieId)
    {
        var reviews = await _context.ReviewMovies
            .Where(r => r.MovieId == movieId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedDate) // BaseAuditableEntity-də hansı tarix sahən varsa onu yaz
            .Select(r => new ReviewDto
            {
                UserName = r.User != null ? r.User.UserName : "User",
                ProfilePictureUrl = null, // əgər AppUser-da şəkil field-in varsa burda map et
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedDate = r.CreatedDate
            })
            .ToListAsync();

        return Ok(reviews);
    }
}