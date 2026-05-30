using Audacia.Commands;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.Users.Update;

/// <summary>
/// The service for updating a user.
/// </summary>
public class UpdateUserService : IUpdateUserService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UpdateUserService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public UpdateUserService(
        DatabaseContext context,
        ILogger<UpdateUserService> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles updating a user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<CommandResult> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(UpdateUserRequest));
        ArgumentNullException.ThrowIfNull(request);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            .ConfigureAwait(false);

        if (user == null)
        {
            return CommandResult.Failure("User not found");
        }

        user.Cid = request.Cid ?? user.Cid;
        user.FullName = request.FullName ?? user.FullName;
        user.Email = request.Email ?? user.Email;
        user.IsAdmin = request.IsAdmin ?? user.IsAdmin;
        user.MemberType = request.MemberType ?? user.MemberType;

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return CommandResult.Success();
    }
}