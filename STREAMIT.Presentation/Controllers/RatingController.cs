using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.RatingDtos;
using STREAMIT.DataAccess.Contexts;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public RatingsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<RatingDto>>> GetAllRatings()
    {
        // Movie Ratings
        var movieRatings = await _dbContext.ReviewMovies
            .Include(r => r.Movie)
            .Select(r => new RatingDto
            {
                Id = r.Id,
                Category = "Movie",
                Name = r.Movie!.Title,
                ReleaseYear = r.Movie.ReleaseDate.Year,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToListAsync();

    

        // Combine
        var allRatings = movieRatings
                                     .OrderByDescending(r => r.Rating) // Optional: sort by rating
                                     .ToList();

        return Ok(allRatings);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var movieReview = await _dbContext.ReviewMovies.FindAsync(id);
        if (movieReview != null)
        {
            _dbContext.ReviewMovies.Remove(movieReview);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

       

        return NotFound();
    }
}