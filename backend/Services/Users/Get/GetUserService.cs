using Audacia.Commands;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Users.Dtos;

namespace Services.Users.Get;

/// <summary>
/// The service that gets a user.
/// </summary>
public class GetUserService : IGetUserService
{
  private readonly DatabaseContext _context;
  private readonly ILogger<GetUserService> _logger;

  /// <summary>
  /// Constructor for the class.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="logger"></param>
  public GetUserService(
    DatabaseContext context,
    ILogger<GetUserService> logger
  )
  {
    _context = context;
    _logger = logger;
  }
  /// <summary>
  /// Method that handles the asynchronous operation.
  /// </summary>
  public async Task<CommandResult<UserDto>> Handle(GetUserRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(GetUserRequest));
    ArgumentNullException.ThrowIfNull(request);

    var result = await _context.Users
        .Where(u => u.Id == request.Id)
        .Select(u => new UserDto
        (
          u.Id,
          u.Cid,
          u.Email,
          u.FullName,
          u.IsAdmin,
          u.MemberType
        ))
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
    if (result == null)
    {
      _logger.LogWarning("User with ID {UserId} not found.", request.Id);
      return CommandResult.Failure<UserDto>("No user found");
    }

    return CommandResult.WithResult(result);
  }
}