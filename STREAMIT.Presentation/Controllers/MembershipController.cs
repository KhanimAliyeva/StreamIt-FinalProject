using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Services.Abstractions;

namespace STREAMIT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipController(IMembershipService service) : ControllerBase
{
    #region GET ALL

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAllAsync();

        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }

    #endregion


    #region GET BY ID

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);

        if (!result.IsSucceed)
            return NotFound(result);

        return Ok(result);
    }

    #endregion


    #region CREATE

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMembershipDto dto)
    {
        var result = await service.CreateAsync(dto);

        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }

    #endregion


    #region UPDATE

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMembershipDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id mismatch");

        var result = await service.UpdateAsync(dto);

        if (!result.IsSucceed)
            return NotFound(result);

        return Ok(result);
    }

    #endregion


    #region DELETE (Soft Delete)

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await service.DeleteAsync(id);

        if (!result.IsSucceed)
            return NotFound(result);

        return Ok(result);
    }

    #endregion
}
