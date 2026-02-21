using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.TvShowDtos;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Business.Services.Implementations; // <-- use interface here

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TvShowController(ITvShowService _service) : ControllerBase
    {
       

        // GET: api/TvShow
        [HttpGet]
        public async Task<IActionResult> GetAllTvShows()
        {
            var tvShows = await _service.GetAllAsync();
            return Ok(tvShows);
        }

        // GET: api/TvShow/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTvShowById(int id)
        {
            var tvShow = await _service.GetByIdAsync(id);
            if (tvShow == null)
                return NotFound();
            return Ok(tvShow);
        }

        // POST: api/TvShow
        [HttpPost]
        public async Task<IActionResult> CreateTvShow([FromForm] CreateTvShowDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.IsSucceed)
                return BadRequest(result.Message);
            return StatusCode(result.StatusCode, result.Message);
        }

        // PUT: api/TvShow
        [HttpPut]
        public async Task<IActionResult> UpdateTvShow([FromForm] UpdateTvShowDto dto)
        {
            var result = await _service.UpdateAsync(dto);
            if (!result.IsSucceed)
                return BadRequest(result.Message);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
