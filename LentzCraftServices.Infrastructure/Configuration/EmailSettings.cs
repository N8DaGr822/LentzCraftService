namespace LentzCraftServices.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for SMTP email sending.
/// Binds to the "EmailSettings" section in appsettings.json.
/// </summary>
public class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}
