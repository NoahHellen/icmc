using Audacia.Commands;
using MediatR;

namespace Services.Users.Auth;

/// <summary>
/// The request to generate a JWT token.
/// </summary>
/// <param name="Cid"></param>
/// <param name="Otp"></param>
public record CreateJwtRequest(
    string Cid,
    string Otp
) : IRequest<CommandResult<string>>;