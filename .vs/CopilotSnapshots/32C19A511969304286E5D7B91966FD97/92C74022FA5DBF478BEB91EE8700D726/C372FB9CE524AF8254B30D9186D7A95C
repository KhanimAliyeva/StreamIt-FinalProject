using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Services.Abstractions;

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController(IMovieService _service,IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _service.GetAllWithDetailsAsync();
            var dtos = _mapper.Map<List<GetMovieDto>>(movies);
            return Ok(dtos);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateMovieDto dto)
        {
             var movie= await _service.CreateAsync(dto);
            return Ok( movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _service.DeleteAsync(id);
            return Ok(movie);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _service.GetByIdAsync(id);
            return Ok(movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm]UpdateMovieDto dto)
        {
            var movie = await _service.UpdateAsync(dto);
            return Ok(movie);
        }


    }
}
