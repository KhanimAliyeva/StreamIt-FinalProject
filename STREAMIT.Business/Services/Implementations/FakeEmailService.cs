using STREAMIT.Business.Services.Abstractions;

public class FakeEmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        Console.WriteLine("-------FAKE EMAIL-------");
        Console.WriteLine($"To: {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {htmlMessage}");
        Console.WriteLine("------------------------");
        return Task.CompletedTask;
    }
}