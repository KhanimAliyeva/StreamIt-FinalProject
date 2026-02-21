using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Services.Abstractions;

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController(IGenreService _service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateGenreDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok(new STREAMIT.Business.Dtos.ResultDtos.ResultDto { IsSucceed = true, Message = "Created Successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]UpdateGenreDto dto)
        {
            await _service.UpdateAsync(dto);
            return Ok(new STREAMIT.Business.Dtos.ResultDtos.ResultDto { IsSucceed = true, Message = "Updated Successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new STREAMIT.Business.Dtos.ResultDtos.ResultDto { IsSucceed = true, Message = "Deleted Successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _service.GetAllAsync();
            return Ok(new STREAMIT.Business.Dtos.ResultDtos.ResultDto<List<GetGenreDto>>(genres));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var genre = await _service.GetByIdAsync(id);
            return Ok(new STREAMIT.Business.Dtos.ResultDtos.ResultDto<GetGenreDto>(genre));
        }
    }
}
