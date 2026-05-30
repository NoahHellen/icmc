namespace Services.Email.Configuration;

/// <summary>
/// Configuration for email services.
/// </summary>
public class EmailConfig
{
    /// <summary>
    /// The SMTP server address.
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// The SMTP server port.
    /// </summary>
    public int SmtpPort { get; set; }

    /// <summary>
    /// The username for SMTP authentication.
    /// </summary>
    public string SmtpUsername { get; set; } = string.Empty;

    /// <summary>
    /// The password for SMTP authentication.
    /// </summary>
    public string SmtpPassword { get; set; } = string.Empty;

    /// <summary>
    /// The email address to send from.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// The name to display as the sender.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;
}
