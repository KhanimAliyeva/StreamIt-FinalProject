using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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

    // GET api/reviews/movie/{movieId}
    [HttpGet("movie/{movieId}")]
    public async Task<IActionResult> GetByMovie(int movieId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var reviews = await _context.ReviewMovies
            .Where(r => r.MovieId == movieId && r.ParentReviewId == null && !r.IsDeleted)
            .Include(r => r.User)
            .Include(r => r.Likes)
            .Include(r => r.Replies.Where(rep => !rep.IsDeleted))
                .ThenInclude(rep => rep.User)
            .Include(r => r.Replies.Where(rep => !rep.IsDeleted))
                .ThenInclude(rep => rep.Likes)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();

        var result = reviews.Select(r => MapToDto(r, currentUserId)).ToList();
        return Ok(result);
    }

    // POST api/reviews  — add top-level review
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? dto.UserId; // MVC may pre-fill UserId
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        // One review per user per movie (top-level only)
        var exists = await _context.ReviewMovies
            .AnyAsync(r => r.MovieId == dto.MovieId && r.UserId == userId && r.ParentReviewId == null && !r.IsDeleted);
        if (exists) return Conflict("You already reviewed this movie.");

        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == dto.MovieId);
        if (movie == null) return BadRequest("Movie not found.");

        var review = new ReviewMovie
        {
            MovieId = dto.MovieId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment ?? string.Empty
        };
        _context.ReviewMovies.Add(review);

        var oldCount = movie.RatingCount;
        var newCount = oldCount + 1;
        movie.AverageRating = ((movie.AverageRating * oldCount) + dto.Rating) / newCount;
        movie.RatingCount = newCount;

        await _context.SaveChangesAsync();

        await _context.Entry(review).Reference(r => r.User).LoadAsync();
        return Ok(MapToDto(review, userId));
    }

    // POST api/reviews/reply
    [HttpPost("reply")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddReply([FromBody] AddReplyDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var parent = await _context.ReviewMovies
            .FirstOrDefaultAsync(r => r.Id == dto.ParentReviewId && !r.IsDeleted);
        if (parent == null) return BadRequest("Parent review not found.");

        var reply = new ReviewMovie
        {
            MovieId = dto.MovieId,
            UserId = userId,
            Rating = 0,
            Comment = dto.Comment,
            ParentReviewId = dto.ParentReviewId
        };
        _context.ReviewMovies.Add(reply);
        await _context.SaveChangesAsync();

        await _context.Entry(reply).Reference(r => r.User).LoadAsync();
        return Ok(MapToDto(reply, userId));
    }

    // POST api/reviews/{id}/like
    [HttpPost("{id}/like")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ToggleLike(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var review = await _context.ReviewMovies
            .Include(r => r.Likes)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (review == null) return NotFound();

        var existing = review.Likes.FirstOrDefault(l => l.UserId == userId);
        string status;
        if (existing != null)
        {
            _context.ReviewLikes.Remove(existing);
            review.LikeCount = Math.Max(0, review.LikeCount - 1);
            status = "unliked";
        }
        else
        {
            _context.ReviewLikes.Add(new ReviewLike { ReviewId = id, UserId = userId });
            review.LikeCount++;
            status = "liked";
        }

        await _context.SaveChangesAsync();
        return Ok(new { status, likeCount = review.LikeCount });
    }

    // DELETE api/reviews/{id}
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var review = await _context.ReviewMovies.FindAsync(id);
        if (review == null) return NotFound();
        if (review.UserId != userId) return Forbid();

        review.IsDeleted = true;
        review.DeletedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok();
    }

    private static ReviewDto MapToDto(ReviewMovie r, string? currentUserId) => new()
    {
        Id = r.Id,
        UserId = r.UserId,
        IsOwn = r.UserId == currentUserId,
        ParentReviewId = r.ParentReviewId,
        UserName = r.User?.UserName ?? "User",
        ProfilePictureUrl = r.User?.ProfilePictureUrl,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedDate = r.CreatedDate,
        LikeCount = r.LikeCount,
        LikedByCurrentUser = currentUserId != null && r.Likes.Any(l => l.UserId == currentUserId),
        Replies = r.Replies
            .Where(rep => !rep.IsDeleted)
            .OrderBy(rep => rep.CreatedDate)
            .Select(rep => MapToDto(rep, currentUserId))
            .ToList()
    };
}
