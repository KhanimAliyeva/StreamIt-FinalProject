using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Services.Abstractions;

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController(IPersonService _personService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePersonDto dto)
        {
            var result = await _personService.CreateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdatePersonDto dto)
        {
            var result = await _personService.UpdateAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _personService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var people = await _personService.GetAllAsync();
            return Ok(people);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _personService.GetByIdAsync(id);
            if (person == null)
                return NotFound();
            return Ok(person);
        }
    }
}
