using Audacia.Commands;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.Users.Delete;

/// <summary>
/// The service that deletes a user.
/// </summary>
public class DeleteUserService : IDeleteUserService
{
  private readonly DatabaseContext _context;
  private readonly ILogger<DeleteUserService> _logger;

  /// <summary>
  /// The constructor for the class.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="logger"></param>
  public DeleteUserService(
    DatabaseContext context,
    ILogger<DeleteUserService> logger
  )
  {
    _context = context;
    _logger = logger;
  }
  /// <summary>
  /// Handles deleting a user.
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<CommandResult> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(DeleteUserRequest));
    ArgumentNullException.ThrowIfNull(request);

    var result = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == request.Id)
        .ConfigureAwait(false);

    if (result == null)
    {
      return CommandResult.Failure();
    }

    _context.Users.Remove(result);

    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    return CommandResult.Success();

  }
}