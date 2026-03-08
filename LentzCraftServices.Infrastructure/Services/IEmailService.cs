namespace LentzCraftServices.Infrastructure.Services;

/// <summary>
/// Service interface for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a contact form email to the configured recipient.
    /// </summary>
    /// <param name="senderName">The name of the person submitting the form.</param>
    /// <param name="senderEmail">The email of the person submitting the form (set as Reply-To).</param>
    /// <param name="subject">The subject line.</param>
    /// <param name="message">The message body.</param>
    /// <returns>True if the email was sent successfully; false otherwise.</returns>
    Task<bool> SendContactEmailAsync(string senderName, string senderEmail, string subject, string message);
}
