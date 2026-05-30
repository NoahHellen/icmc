using Audacia.Commands;
using Domain.Entities;
using EntityFramework;
using Microsoft.Extensions.Logging;

namespace Services.Users.Add;

/// <summary>
/// The service that adds a user.
/// </summary>
public class AddUserService : IAddUserService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AddUserService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public AddUserService(
        DatabaseContext context,
        ILogger<AddUserService> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles adding a user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<CommandResult> Handle(AddUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(AddUserRequest));
        ArgumentNullException.ThrowIfNull(request);

        var user = new User
        {
            Cid = request.Cid,
            FullName = request.FullName,
            Email = request.Email,
            IsAdmin = request.IsAdmin,
            MemberType = request.MemberType,
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return CommandResult.Success();
    }
}