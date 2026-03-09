using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.DataAccess.Contexts;

namespace STREAMIT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipController : ControllerBase
{
    private readonly IMembershipService _service;
    private readonly AppDbContext _context;

    public MembershipController(IMembershipService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    #region GET ALL

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }

    #endregion

    #region GET BY ID

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.IsSucceed)
            return NotFound(result);

        return Ok(result);
    }

    #endregion

    #region USER MEMBERSHIP

    [HttpGet("user-membership")]
    public async Task<IActionResult> GetUserMembership([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user == null)
            return NotFound("User not found");

        var membership = await _context.UserMemberships
            .Include(x => x.Membership)
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.StartDate)
            .Select(x => new
            {
                membershipId = x.MembershipId,
                planName = x.Membership.Name,
                startDate = x.StartDate,
                endDate = x.EndDate,
                isActive = x.IsActive && x.EndDate > DateTime.UtcNow,
                paidAmount = x.PaidAmount,
                paymentMethod = x.PaymentMethod,
                isTrial = x.IsTrial,
                autoRenew = x.AutoRenew
            })
            .FirstOrDefaultAsync();

        if (membership == null)
        {
            return Ok(new
            {
                planName = "Free",
                isActive = true,
                startDate = (DateTime?)null,
                endDate = (DateTime?)null,
                paidAmount = 0,
                paymentMethod = "",
                isTrial = false,
                autoRenew = false
            });
        }

        return Ok(membership);
    }

    #endregion

    #region CREATE

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMembershipDto dto)
    {
        var result = await _service.CreateAsync(dto);

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

        var result = await _service.UpdateAsync(dto);

        if (!result.IsSucceed)
            return NotFound(result);

        return Ok(result);
    }

    #endregion

    #region DELETE (Soft Delete)

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.IsSucceed)
            return NotFound(result);

        return Ok(result);
    }

    #endregion
}