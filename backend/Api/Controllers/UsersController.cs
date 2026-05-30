using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Users.Add;
using Services.Users.Auth;
using Services.Users.Delete;
using Services.Users.Get;
using Services.Users.Search;
using Services.Users.Update;

namespace Api.Controllers;

/// <summary>
/// Controller for requests relating to users.
/// </summary>
[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
  private readonly IGetUserService _getUserService;
  private readonly IDeleteUserService _deleteUserService;
  private readonly IAddUserService _addUserService;
  private readonly IUpdateUserService _updateUserService;
  private readonly ISearchUsersService _searchUsersService;
  private readonly ISendOtpService _sendOtpService;
  private readonly ICreateJwtService _createJwtService;
  private readonly ILogger<UsersController> _logger;

  public UsersController(
    IGetUserService getUserService,
    IDeleteUserService deleteUserService,
    IAddUserService addUserService,
    IUpdateUserService updateUserService,
    ISearchUsersService searchUsersService,
    ISendOtpService sendOtpService,
    ICreateJwtService createJwtService,
    ILogger<UsersController> logger
  )
  {
    _getUserService = getUserService;
    _deleteUserService = deleteUserService;
    _addUserService = addUserService;
    _updateUserService = updateUserService;
    _searchUsersService = searchUsersService;
    _sendOtpService = sendOtpService;
    _createJwtService = createJwtService;
    _logger = logger;
  }

  [HttpGet("me")]
  public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(GetUserRequest));
    var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
    {
      return Unauthorized();
    }

    var request = new GetUserRequest(userId);
    var result = await _getUserService.Handle(request, cancellationToken).ConfigureAwait(false);

    if (result.IsSuccess)
    {
      return Ok(result.Output);
    }

    return NotFound(result.Errors);
  }

  [HttpGet("{id}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetUser([FromRoute] int id, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(GetUserRequest));
    var request = new GetUserRequest(id);
    var result = await _getUserService.Handle(request, cancellationToken).ConfigureAwait(false);

    if (result.IsSuccess)
    {
      return Ok(result.Output);
    }

    return NotFound(result.Errors);
  }

  [HttpGet]
  public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(SearchUsersRequest));
    var result = await _searchUsersService.Handle(request, cancellationToken).ConfigureAwait(false);
    if (result.IsSuccess)
    {
      return Ok(result.Output);
    }

    return NotFound(result.Errors);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(DeleteUserRequest));
    var request = new DeleteUserRequest(id);
    var result = await _deleteUserService.Handle(request, cancellationToken);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return BadRequest(result.Errors);
  }

  [HttpPost]
  public async Task<IActionResult> AddUser(AddUserRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(AddUserRequest));
    var result = await _addUserService.Handle(request, cancellationToken);

    if (result.IsSuccess)
    {
      return Created();
    }
    return BadRequest(result.Errors);
  }


  [HttpPatch("{id}")]
  public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(UpdateUserRequest));
    if (id != request.Id)
    {
      return BadRequest("Id in URL must match Id in request body");
    }

    var result = await _updateUserService.Handle(request, cancellationToken);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return BadRequest(result.Errors);
  }

  [HttpPost("auth/otp")]
  [AllowAnonymous]
  public async Task<IActionResult> SendOtp(SendOtpRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(SendOtpRequest));
    var result = await _sendOtpService.Handle(request, cancellationToken);

    if (result.IsSuccess)
    {
      return NoContent();
    }

    return BadRequest(result.Errors);
  }

  [HttpPost("auth/jwt")]
  [AllowAnonymous]
  public async Task<IActionResult> CreateJwt(CreateJwtRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(CreateJwtRequest));
    var result = await _createJwtService.Handle(request, cancellationToken);

    if (result.IsSuccess)
    {
      return Ok(result.Output);
    }

    return BadRequest(result.Errors);
  }
}
