using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using STREAMIT.MVC.Models;
using System.Net;
using System.Net.Mail;

namespace STREAMIT.MVC.Controllers
{
    public class ContactController : Controller
    {
        private readonly EmailSettings _emailSettings;

        public ContactController(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactMessageVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactMessageVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var mail = new MailMessage();

                mail.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                mail.To.Add("aliyevakhanim386@gmail.com");

                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    mail.ReplyToList.Add(new MailAddress(model.Email));
                }

                mail.Subject = $"Contact Form - {model.FirstName} {model.LastName}";
                mail.IsBodyHtml = true;

                mail.Body = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
</head>

<body style='margin:0;padding:0;background:#ffffff;font-family:Arial,Helvetica,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='background:#0f0f0f;padding:40px 0;'>

<tr>
<td align='center'>

<table width='600' cellpadding='0' cellspacing='0' style='background:#141414;border-radius:8px;overflow:hidden;'>

<tr>
<td style='background:#e50914;padding:20px;text-align:center;color:white;font-size:24px;font-weight:bold;'>
STREAMIT
</td>
</tr>

<tr>
<td style='padding:30px;color:#ffffff;'>

<h2 style='margin-top:0;color:#ffffff;'>New Contact Message</h2>

<p style='color:#bbbbbb;margin-bottom:25px;'>
A new message has been sent from the contact form.
</p>

<table width='100%' cellpadding='10' cellspacing='0' style='border-collapse:collapse;'>

<tr style='border-bottom:1px solid #333'>
<td style='color:#999;width:150px;'>Name</td>
<td style='color:white;font-weight:bold;'>{model.FirstName} {model.LastName}</td>
</tr>

<tr style='border-bottom:1px solid #333'>
<td style='color:#999;'>Email</td>
<td style='color:white;'>{model.Email}</td>
</tr>

<tr style='border-bottom:1px solid #333'>
<td style='color:#999;'>Phone</td>
<td style='color:white;'>{model.PhoneNumber}</td>
</tr>

</table>

<div style='margin-top:25px;'>

<p style='color:#999;margin-bottom:10px;'>Message</p>

<div style='background:#1f1f1f;padding:15px;border-radius:6px;color:white;line-height:1.6;'>
{model.Message}
</div>

</div>

</td>
</tr>

<tr>
<td style='background:#0b0b0b;padding:15px;text-align:center;color:#777;font-size:12px;'>

This message was sent from the StreamIT contact form.

</td>
</tr>

</table>

</td>
</tr>

</table>

</body>
</html>
";

                using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
                smtp.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;

                await smtp.SendMailAsync(mail);

                TempData["SuccessMessage"] = "Your message has been sent successfully.";
                return RedirectToAction("Index");
            }
            catch (SmtpException ex)
            {
                ModelState.AddModelError("", $"SMTP error: {ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while sending the message: {ex.Message}");
                return View(model);
            }
        }
    }
}