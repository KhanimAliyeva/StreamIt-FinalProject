using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.DashboardDtos;
using STREAMIT.DataAccess.Contexts;

namespace STREAMIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var dto = new GetDashboardDto();

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            dto.TotalUsers = await _context.Users.CountAsync();

            // Səndə IsActive yoxdursa bunu TotalUsers elə
            dto.ActiveUsers = await _context.Users.CountAsync();

            // Membership/subscription sistemi necədirsə ona uyğun dəyiş
            dto.TotalSubscribers = await _context.Users.CountAsync(x => x.UserMemberships != null);

            dto.TotalMovies = await _context.Movies.CountAsync();

            // review entity adın fərqli ola bilər
            dto.TotalReviews = await _context.ReviewMovies.CountAsync();

            // favorite entity adın fərqli ola bilər
            dto.TotalFavorites = await _context.Favorites.CountAsync();

            // CreatedDate səndə fərqli ola bilər
            dto.NewUsersThisMonth = await _context.Users
                .CountAsync(x => x.CreatedAt >= startOfMonth);

            var genreData = await _context.MovieGenres
                .Include(x => x.Genre)
                .GroupBy(x => x.Genre.Name)
                .Select(g => new
                {
                    GenreName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            dto.GenreLabels = genreData.Select(x => x.GenreName).ToList();
            dto.GenreCounts = genreData.Select(x => x.Count).ToList();

            for (int i = 11; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                var monthStart = new DateTime(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var userCount = await _context.Users
                    .CountAsync(x => x.CreatedAt >= monthStart && x.CreatedAt < monthEnd);

                dto.UserLabels.Add(monthStart.ToString("MMM"));
                dto.UserCounts.Add(userCount);
            }

            return Ok(dto);
        }
    }
}