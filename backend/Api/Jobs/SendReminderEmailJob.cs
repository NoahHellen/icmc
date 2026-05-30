using EntityFramework;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Quartz;
using Services.Email.Configuration;

namespace Api.Jobs;

public class SendReminderEmailJob : IJob
{
    private readonly DatabaseContext _context;
    private readonly ILogger<SendReminderEmailJob> _logger;
    private readonly EmailConfig _emailConfig;

    public SendReminderEmailJob(
        DatabaseContext context,
        ILogger<SendReminderEmailJob> logger,
        EmailConfig emailConfig)
    {
        _context = context;
        _logger = logger;
        _emailConfig = emailConfig;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing {JobName}", nameof(SendReminderEmailJob));
        ArgumentNullException.ThrowIfNull(context);

        var today = DateTime.Today;
        var sevenDaysFromNow = today.AddDays(7);

        var overdueItems = await _context.GearItems
            .Where(gi => gi.LentToUser != null && gi.ExpectedReturnDate.HasValue && gi.ExpectedReturnDate < today)
            .Select(gi => new
            {
                fullName = gi.LentToUser!.FullName,
                email = gi.LentToUser!.Email,
                brand = gi.Brand,
                toughTag = gi.ToughTag,
                gearCategory = gi.GearCategory,
                expectedReturnDate = gi.ExpectedReturnDate!.Value
            })
            .ToListAsync();

        _logger.LogInformation("Found {Count} overdue gear items to send reminders for.", overdueItems.Count);
        foreach (var recipient in overdueItems)
        {
            await SendEmail(recipient.email, recipient.fullName, recipient.gearCategory.ToString(), recipient.toughTag, recipient.brand, recipient.expectedReturnDate, true);
        }

        var dueSoonItems = await _context.GearItems
            .Where(gi => gi.LentToUser != null && gi.ExpectedReturnDate.HasValue &&
                         gi.ExpectedReturnDate >= sevenDaysFromNow &&
                         gi.ExpectedReturnDate < sevenDaysFromNow.AddDays(1))
            .Select(gi => new
            {
                fullName = gi.LentToUser!.FullName,
                email = gi.LentToUser!.Email,
                brand = gi.Brand,
                toughTag = gi.ToughTag,
                gearCategory = gi.GearCategory,
                expectedReturnDate = gi.ExpectedReturnDate!.Value
            })
            .ToListAsync();

        _logger.LogInformation("Found {Count} gear items due in 7 days to send reminders for.", dueSoonItems.Count);
        foreach (var recipient in dueSoonItems)
        {
            await SendEmail(recipient.email, recipient.fullName, recipient.gearCategory.ToString(), recipient.toughTag, recipient.brand, recipient.expectedReturnDate, false);
        }

        _logger.LogInformation("{JobName} completed.", nameof(SendReminderEmailJob));
    }

    private async Task SendEmail(string? email, string? fullName, string? gearCategory, string? toughTag, string? brand, DateTimeOffset expectedReturnDate, bool isOverdue)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Skipping reminder for {FullName} because no email address is registered.", fullName);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.SenderEmail));
        message.To.Add(new MailboxAddress(fullName, email));

        if (isOverdue)
        {
            message.Subject = "Overdue ICMC gear item";
            message.Body = new TextPart("plain")
            {
                Text = $@"
                        Dear {fullName},

                        Please return the {gearCategory} with tough tag {toughTag} ({brand}) that you borrowed from ICMC as soon as possible. It was due back on {expectedReturnDate:dd/MM/yyyy}.

                        Kind regards,

                        ICMC
                        "
            };
        }
        else
        {
            message.Subject = "ICMC Gear Reminder: Due in 7 days";
            message.Body = new TextPart("plain")
            {
                Text = $@"
                        Dear {fullName},

                        This is a friendly reminder that the {gearCategory} with tough tag {toughTag} ({brand}) you borrowed from ICMC is due back in 7 days, on {expectedReturnDate:dd/MM/yyyy}.

                        Please ensure you return it on or before this date.

                        Kind regards,

                        ICMC
                        "
            };
        }

        using var client = new SmtpClient();
        try
        {
            _logger.LogDebug("Sending reminder email to {Email} for item {ToughTag}.", email, toughTag);
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            _logger.LogInformation("Successfully sent reminder email to {Email} for item {ToughTag}.", email, toughTag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending reminder email to {Email}.", email);
        }
    }
}