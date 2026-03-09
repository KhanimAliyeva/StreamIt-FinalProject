using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public PaymentController(IPaymentService paymentService, AppDbContext context, IConfiguration configuration)
    {
        _paymentService = paymentService;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromQuery] int membershipId, [FromQuery] string email)
    {
        var membership = await _context.Memberships.FindAsync(membershipId);
        if (membership == null)
            return NotFound("Membership not found");

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user == null)
            return BadRequest("User not found");

        var mvcBase = _configuration["Mvc:BaseUrl"];
        var redirectUrl = !string.IsNullOrWhiteSpace(mvcBase)
            ? mvcBase.TrimEnd('/') + "/Membership/RedirectPayment"
            : "https://localhost:7107/Membership/RedirectPayment";

        var createVm = new STREAMIT.Business.Dtos.PaymentDtos.MembershipCreateVM
        {
            Amount = Convert.ToInt32(Math.Ceiling(membership.Price)),
            Currency = "AZN",
            Description = $"Membership purchase: {membership.Name}",
            RedirectUrl = redirectUrl
        };

        var purchase = await _paymentService.CreatePaymentAsync(createVm);
        if (purchase == null || purchase.Membership == null)
            return StatusCode(500, "Failed to create payment with provider");

        var payment = new Payment
        {
            UserId = user.Id,
            MembershipId = membershipId,
            Amount = membership.Price,
            OrderId = purchase.Membership.Id.ToString(),
            Status = "Pending",
            RedirectUrl = createVm.RedirectUrl,
            Currency = createVm.Currency,
            CreatedDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        var checkoutUrl = $"{purchase.Membership.HppUrl}?id={purchase.Membership.Id}&password={purchase.Membership.Password}";
        return Ok(new { checkoutUrl });
    }
    [HttpPost("callback")]
    public async Task<IActionResult> Callback(KapitalBankCallbackModel model)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.OrderId == model.OrderId);

        if (payment == null)
            return BadRequest("Payment not found");

        if (string.IsNullOrWhiteSpace(payment.UserId))
            return BadRequest("Payment has empty UserId");

        var userExists = await _context.Users.AnyAsync(x => x.Id == payment.UserId);
        if (!userExists)
            return BadRequest("Payment UserId not found in AspNetUsers");

        if (string.Equals(model.Status, "FullyPaid", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(model.Status, "success", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(model.Status, "paid", StringComparison.OrdinalIgnoreCase))
        {
            payment.Status = "Paid";

            var membership = await _context.Memberships.FindAsync(payment.MembershipId);

            var alreadyExists = await _context.UserMemberships
                .AnyAsync(x => x.UserId == payment.UserId &&
                               x.MembershipId == payment.MembershipId &&
                               x.IsActive);

            if (!alreadyExists)
            {
                var userMembership = new UserMembership
                {
                    UserId = payment.UserId,
                    MembershipId = payment.MembershipId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(membership?.DurationInDays ?? 30),
                    IsActive = true,
                    IsTrial = false,
                    PaidAmount = payment.Amount,
                    PaymentMethod = "KapitalBank",
                    AutoRenew = false,
                    CancelledAt = null
                };

                _context.UserMemberships.Add(userMembership);
            }
        }
        else
        {
            payment.Status = model.Status ?? "Failed";
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}