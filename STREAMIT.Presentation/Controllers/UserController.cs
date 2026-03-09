using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.UserDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using System.Security.Claims;

namespace STREAMIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================================
        // GET ALL USERS
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Select(u => new GetUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    Fullname = u.Fullname,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    EmailConfirmed = u.EmailConfirmed,
                    CreatedAt = u.CreatedAt,

                    FavoriteMoviesCount = u.UserMovies.Count,
                    MovieReviewsCount = u.MovieReviews.Count,

                    MembershipName = u.UserMemberships
                        .OrderByDescending(m => m.StartDate)
                        .Select(m => m.Membership!.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(users);
        }

        // =========================================
        // GET USER BY ID
        // =========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new GetUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    Fullname = u.Fullname,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    EmailConfirmed = u.EmailConfirmed,
                    CreatedAt = u.CreatedAt,

                    FavoriteMoviesCount = u.UserMovies.Count,
                    MovieReviewsCount = u.MovieReviews.Count,

                    MembershipName = u.UserMemberships
                        .OrderByDescending(m => m.StartDate)
                        .Select(m => m.Membership!.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // ✅ GET api/user/me
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new EditProfileDto
                {
                    FullName = u.Fullname,
                    UserName = u.UserName,
                    Email = u.Email,
                    ImageUrl = u.ProfilePictureUrl
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        // ✅ PUT api/user/me
        [HttpPut("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateMe([FromBody] EditProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            user.Fullname = dto.FullName?.Trim() ?? user.Fullname;
            user.UserName = dto.UserName?.Trim() ?? user.UserName;

            // Email dəyişmək istəyirsənsə, diqqət: Identity-də xüsusi flow olur.
            // İndilik sadə saxlayırıq:
            user.Email = dto.Email?.Trim() ?? user.Email;

            user.ProfilePictureUrl = dto.ImageUrl?.Trim() ?? user.ProfilePictureUrl;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class EditProfileDto
    {
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ImageUrl { get; set; }
    }

}