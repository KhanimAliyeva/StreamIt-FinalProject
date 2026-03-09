using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.SliderDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;

[Route("api/[controller]")]
[ApiController]
public class SliderController : ControllerBase
{
    private readonly AppDbContext _context;

    public SliderController(AppDbContext context)
    {
        _context = context;
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sliders = await _context.Sliders
            .Include(x => x.Movie)
            .OrderBy(x => x.Order)
            .ToListAsync();

        return Ok(sliders);
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();

        return Ok(slider);
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(CreateSliderDto dto)
    {
        var slider = new Slider
        {
            MovieId = dto.MovieId,
            Order = dto.Order,
            IsActive = dto.IsActive
        };

        _context.Sliders.Add(slider);
        await _context.SaveChangesAsync();

        return Ok(slider);
    }

    // UPDATE
    [HttpPut]
    public async Task<IActionResult> Update(UpdateSliderDto dto)
    {
        var slider = await _context.Sliders.FindAsync(dto.Id);
        if (slider == null) return NotFound();

        slider.MovieId = dto.MovieId;
        slider.Order = dto.Order;
        slider.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();

        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}