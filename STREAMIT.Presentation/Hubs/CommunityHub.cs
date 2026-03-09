using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using STREAMIT.DataAccess.Contexts;
using STREAMIT.Core.Entities;
using System.Security.Claims;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + CookieAuthenticationDefaults.AuthenticationScheme)]
public class CommunityHub : Hub
{
    private readonly AppDbContext _db;
    private readonly ILogger<CommunityHub> _logger;

    public CommunityHub(AppDbContext db, ILogger<CommunityHub> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SendMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        string? userId =
            Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
            Context.User?.FindFirstValue("sub") ??
            Context.User?.FindFirstValue("nameid") ??
            Context.User?.FindFirstValue("id");

        string? username =
            Context.User?.FindFirstValue("username") ??
            Context.User?.FindFirstValue("unique_name") ??
            Context.User?.FindFirstValue(ClaimTypes.Name);

        if (!string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(username))
        {
            var u = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (u != null)
                username = u.UserName;
        }

        username ??= "User";

        CommunityMessage? msg = null;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            msg = new CommunityMessage
            {
                UserId = userId,
                Text = text.Trim(),
                SentAt = DateTime.UtcNow,
                GroupId = 1
            };

            try
            {
                _db.CommunityMessages.Add(msg);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save community message");
            }
        }

        await Clients.All.SendAsync("ReceiveMessage", new
        {
            userName = username,
            text = text.Trim(),
            sentAt = msg?.SentAt ?? DateTime.UtcNow
        });
    }
}