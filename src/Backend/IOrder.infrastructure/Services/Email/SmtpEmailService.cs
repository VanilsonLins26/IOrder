using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using IOrder.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOrder.infrastructure.Services.Email;

[ExcludeFromCodeCoverage]
public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly SmtpClient _smtpClient;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _logger = logger;
        _fromEmail = configuration["Smtp:FromEmail"] ?? "noreply@iorder.com";
        _fromName = configuration["Smtp:FromName"] ?? "IOrder";

        var host = configuration["Smtp:Host"] ?? "localhost";
        var port = int.Parse(configuration["Smtp:Port"] ?? "1025");
        var user = configuration["Smtp:User"] ?? "";
        var pass = configuration["Smtp:Password"] ?? "";

        _smtpClient = new SmtpClient(host, port);

        if (!string.IsNullOrEmpty(user))
        {
            _smtpClient.Credentials = new NetworkCredential(user, pass);
            _smtpClient.EnableSsl = true;
        }
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MailMessage(_fromEmail, to, subject, body)
            {
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(message);

            _logger.LogInformation("Email sent to {To} | Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
        }
    }

    public void Dispose()
    {
        _smtpClient?.Dispose();
    }
}
