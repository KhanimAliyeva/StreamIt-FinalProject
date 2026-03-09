using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Services.Abstractions;

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController(ILanguageService _service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLanguageDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok(new ResultDto
            {
                IsSucceed = true,
                Message = "Created Successfully"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateLanguageDto dto)
        {
            await _service.UpdateAsync(dto);
            return Ok(new ResultDto
            {
                IsSucceed = true,
                Message = "Updated Successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new ResultDto
            {
                IsSucceed = true,
                Message = "Deleted Successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var languages = await _service.GetAllAsync();
            return Ok(languages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var language = await _service.GetByIdAsync(id);
            return Ok(language);
        }
    }
}