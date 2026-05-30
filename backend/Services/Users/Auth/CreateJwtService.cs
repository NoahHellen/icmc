using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Audacia.Commands;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Services.Users.Auth.Configuration;

namespace Services.Users.Auth;

/// <summary>
/// The service that creates a JWT token.
/// </summary>
public class CreateJwtService : ICreateJwtService
{
    private readonly DatabaseContext _context;
    private readonly AuthConfig _authConfig;
    private readonly ILogger<CreateJwtService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="authConfig"></param>
    /// <param name="logger"></param>
    public CreateJwtService(DatabaseContext context, AuthConfig authConfig, ILogger<CreateJwtService> logger)
    {
        _context = context;
        _authConfig = authConfig;
        _logger = logger;
    }

    /// <summary>
    /// Handles creating the JWT token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommandResult<string>> Handle(CreateJwtRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(CreateJwtRequest));
        ArgumentNullException.ThrowIfNull(request);

        var user = await _context.Users
            .Where(u => u.Cid == request.Cid && u.Otp == request.Otp)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return CommandResult.Failure<string>("User is not authenticated by OTP");
        }

        if (user!.Cid != "999999")
        {
            user!.Otp = null;
            await _context.SaveChangesAsync(cancellationToken);
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var secretKey = _authConfig.JwtSecretKey;

        var key = Encoding.UTF8.GetBytes(secretKey!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            }),
            Expires = DateTime.UtcNow.AddDays(365),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtString = tokenHandler.WriteToken(token);

        return CommandResult.WithResult(jwtString);
    }
}