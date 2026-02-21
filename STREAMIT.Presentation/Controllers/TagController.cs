using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Business.Dtos.TagDtos.STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Business.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STREAMIT.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _service;
        private readonly IMapper _mapper;

        public TagController(ITagService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: api/Tag
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _service.GetAllAsync();
            return Ok(tags);
        }

        // GET: api/Tag/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tag = await _service.GetByIdAsync(id);
            return Ok(tag);
        }

        // POST: api/Tag
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.IsSucceed)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = dto.Name }, result);
        }

        // PUT: api/Tag/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTagDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var result = await _service.UpdateAsync(dto);
            return Ok(result);
        }

        // DELETE: api/Tag/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }
    }
}
