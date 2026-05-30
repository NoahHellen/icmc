using Audacia.Commands;
using MediatR;

namespace Services.Users.Auth;

/// <summary>
/// The interface for the service that creates a JWT token.
/// </summary>
public interface ICreateJwtService : IRequestHandler<CreateJwtRequest, CommandResult<string>>
{
}