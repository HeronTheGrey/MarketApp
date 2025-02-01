using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BlazorWebApp;

public class SendGridSettings
{
    public string ApiKey { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
}

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class SendGridEmailSender : IEmailSender
{
    private readonly SendGridSettings _settings;

    public SendGridEmailSender(IOptions<SendGridSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SendGridClient(_settings.ApiKey);
        var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
        var reply = await client.SendEmailAsync(msg);
    }
}
