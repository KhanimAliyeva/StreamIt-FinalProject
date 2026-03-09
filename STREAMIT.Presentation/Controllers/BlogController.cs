using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.BlogDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using STREAMIT.Business.Services.Abstractions;

[Route("api/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public BlogController(AppDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var blogs = await _context.Blogs
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();

        return Ok(blogs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null) return NotFound();

        return Ok(blog);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateBlogDto dto)
    {
        string imageUrl = string.Empty;

        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            imageUrl = await _cloudinaryService.FileCreateAsync(dto.ImageFile, "blogs");
        }

        var blog = new Blog
        {
            Title = dto.Title,
            Description = dto.Description,
            AuthorName = dto.AuthorName,
            ImageUrl = imageUrl,
            CreatedDate = DateTime.Now.AddHours(4)
        };

        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();

        return Ok(blog);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateBlogDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id mismatch");

        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
            return NotFound();

        blog.Title = dto.Title;
        blog.Description = dto.Description;
        blog.AuthorName = dto.AuthorName;

        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            if (!string.IsNullOrWhiteSpace(blog.ImageUrl))
            {
                await _cloudinaryService.FileDeleteAsync(blog.ImageUrl);
            }

            blog.ImageUrl = await _cloudinaryService.FileCreateAsync(dto.ImageFile, "blogs");
        }

        blog.UpdatedDate = DateTime.Now.AddHours(4);

        await _context.SaveChangesAsync();
        return Ok(blog);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(blog.ImageUrl))
        {
            await _cloudinaryService.FileDeleteAsync(blog.ImageUrl);
        }

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Deleted successfully" });
    }
}