using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using LentzCraftServices.Infrastructure.Configuration;

namespace LentzCraftServices.Infrastructure.Services;

/// <summary>
/// MailKit-based implementation of IEmailService.
/// Sends emails via SMTP using configuration from EmailSettings.
/// </summary>
public class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<MailKitEmailService> _logger;

    public MailKitEmailService(
        IOptions<EmailSettings> settings,
        ILogger<MailKitEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendContactEmailAsync(
        string senderName, string senderEmail, string subject, string message)
    {
        try
        {
            _logger.LogInformation(
                "Sending contact email from {SenderName} ({SenderEmail}) with subject: {Subject}",
                senderName, senderEmail, subject);

            var email = new MimeMessage();

            // From: the configured system sender address
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

            // To: the configured business recipient
            email.To.Add(MailboxAddress.Parse(_settings.RecipientEmail));

            // Reply-To: the visitor's email so the owner can reply directly
            email.ReplyTo.Add(new MailboxAddress(senderName, senderEmail));

            // Subject with prefix for easy filtering
            email.Subject = string.IsNullOrWhiteSpace(subject)
                ? $"[Contact Form] Message from {senderName}"
                : $"[Contact Form] {subject}";

            // Build both HTML and plain-text body
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildHtmlBody(senderName, senderEmail, subject, message),
                TextBody = BuildTextBody(senderName, senderEmail, subject, message)
            };
            email.Body = bodyBuilder.ToMessageBody();

            // Send via SMTP
            using var smtp = new SmtpClient();

            var secureSocketOptions = _settings.EnableSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, secureSocketOptions);

            if (!string.IsNullOrEmpty(_settings.SmtpUsername))
            {
                await smtp.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword);
            }

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation(
                "Contact email sent successfully to {Recipient} from {SenderEmail}",
                _settings.RecipientEmail, senderEmail);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send contact email from {SenderName} ({SenderEmail})",
                senderName, senderEmail);
            return false;
        }
    }

    private static string BuildHtmlBody(
        string senderName, string senderEmail, string subject, string message)
    {
        return $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
            <div style=""background-color: #2c1810; color: #ffffff; padding: 20px; text-align: center;"">
                <h1 style=""margin: 0; font-size: 22px;"">New Contact Form Submission</h1>
            </div>
            <div style=""padding: 20px; background-color: #f9f6f2; border: 1px solid #d4c5b5;"">
                <table style=""width: 100%; border-collapse: collapse;"">
                    <tr>
                        <td style=""padding: 10px; font-weight: bold; color: #5c3d2e; width: 120px; vertical-align: top;"">Name:</td>
                        <td style=""padding: 10px;"">{System.Net.WebUtility.HtmlEncode(senderName)}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 10px; font-weight: bold; color: #5c3d2e; vertical-align: top;"">Email:</td>
                        <td style=""padding: 10px;""><a href=""mailto:{System.Net.WebUtility.HtmlEncode(senderEmail)}"">{System.Net.WebUtility.HtmlEncode(senderEmail)}</a></td>
                    </tr>
                    <tr>
                        <td style=""padding: 10px; font-weight: bold; color: #5c3d2e; vertical-align: top;"">Subject:</td>
                        <td style=""padding: 10px;"">{System.Net.WebUtility.HtmlEncode(subject)}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 10px; font-weight: bold; color: #5c3d2e; vertical-align: top;"">Message:</td>
                        <td style=""padding: 10px; white-space: pre-wrap;"">{System.Net.WebUtility.HtmlEncode(message)}</td>
                    </tr>
                </table>
            </div>
            <div style=""padding: 10px; text-align: center; font-size: 12px; color: #999;"">
                This message was sent from the Lentz Crafts &amp; Services contact form.
            </div>
        </div>";
    }

    private static string BuildTextBody(
        string senderName, string senderEmail, string subject, string message)
    {
        return $@"New Contact Form Submission
=============================
Name: {senderName}
Email: {senderEmail}
Subject: {subject}

Message:
{message}

---
This message was sent from the Lentz Crafts & Services contact form.";
    }
}
