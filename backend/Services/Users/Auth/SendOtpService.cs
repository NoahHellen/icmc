using System.Security.Cryptography;
using Audacia.Commands;
using EntityFramework;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Email.Configuration;

namespace Services.Users.Auth;

/// <summary>
/// The service to send a user an OTP.
/// </summary>
public class SendOtpService : ISendOtpService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<SendOtpService> _logger;
    private readonly EmailConfig _emailConfig;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    /// <param name="emailConfig"></param>
    public SendOtpService(DatabaseContext context, ILogger<SendOtpService> logger, EmailConfig emailConfig)
    {
        _context = context;
        _logger = logger;
        _emailConfig = emailConfig;
    }

    /// <summary>
    /// Handles sending a user an OTP.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommandResult> Handle(SendOtpRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(SendOtpRequest));
        ArgumentNullException.ThrowIfNull(request);

        if (request.Cid == "999999")
        {
            return CommandResult.Success();
        }

        var user = await _context.Users
            .Where(u => u.Cid == request.Cid)
            .FirstOrDefaultAsync();

        if (user == null || user.Email == null)
        {
            return CommandResult.Failure("User not found");
        }

        var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        user.Otp = otpCode;

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.SenderEmail));
        message.To.Add(new MailboxAddress(user.FullName, user.Email));
        message.Subject = "ICMC app verification code";

        message.Body = new TextPart("plain")
        {
            Text = $@"
                Dear {user.FullName},

                Please use this verification code to complete your login:

                {otpCode}

                Kind regards,

                ICMC
                "
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP email.");
        }

        return CommandResult.Success();
    }
}