using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using System.Security.Claims;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/community")]
public class CommunityController : ControllerBase
{
    private readonly AppDbContext _context;
    public CommunityController(AppDbContext context) => _context = context;

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? User.FindFirstValue("nameid")
            ?? User.FindFirstValue("id");
    }

    [HttpGet("messages")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMessages()
    {
        var msgs = await _context.CommunityMessages
            .Include(x => x.User)
            .OrderByDescending(x => x.SentAt)
            .Take(50)
            .OrderBy(x => x.SentAt)
            .Select(x => new
            {
                id = x.Id,
                userName = x.User != null ? x.User.UserName : "User",
                text = x.Text,
                sentAt = x.SentAt
            })
            .ToListAsync();

        return Ok(msgs);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount([FromQuery] int groupId = 1)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var unread = await _context.CommunityMessages
            .Where(m => m.GroupId == groupId)
            .Where(m => m.UserId != userId)
            .Where(m => !_context.CommunityMessageReads
                .Any(r => r.MessageId == m.Id && r.UserId == userId))
            .CountAsync();

        return Ok(new { unread });
    }

    
    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkRead([FromQuery] int groupId = 1)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var unreadIds = await _context.CommunityMessages
            .Where(m => m.GroupId == groupId)
            .Where(m => m.UserId != userId)
            .Where(m => !_context.CommunityMessageReads.Any(r => r.MessageId == m.Id && r.UserId == userId))
            .Select(m => m.Id)
            .ToListAsync();

        if (unreadIds.Count == 0) return Ok(new { marked = 0 });

        var reads = unreadIds.Select(id => new CommunityMessageRead
        {
            MessageId = id,
            UserId = userId,
            ReadAt = DateTime.UtcNow.AddHours(4)
        });

        _context.CommunityMessageReads.AddRange(reads);
        await _context.SaveChangesAsync();

        return Ok(new { marked = unreadIds.Count });
    }
}