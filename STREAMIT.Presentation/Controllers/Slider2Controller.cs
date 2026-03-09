using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.SliderDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Slider2Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public Slider2Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sliders = await _context.Sliders2
                .Include(x => x.Movie)
                .OrderBy(x => x.Order)
                .ToListAsync();

            return Ok(sliders);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var slider = await _context.Sliders2.FindAsync(id);
            if (slider == null) return NotFound();

            return Ok(slider);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateSliderDto dto)
        {
            var slider2 = new Slider2
            {
                MovieId = dto.MovieId,
                Order = dto.Order,
                IsActive = dto.IsActive
            };

            _context.Sliders2.Add(slider2);
            await _context.SaveChangesAsync();

            return Ok(slider2);
        }

        // UPDATE
        [HttpPut]
        public async Task<IActionResult> Update(UpdateSliderDto dto)
        {
            var slider = await _context.Sliders2.FindAsync(dto.Id);
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
            var slider = await _context.Sliders2.FindAsync(id);
            if (slider == null) return NotFound();

            _context.Sliders2.Remove(slider);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
