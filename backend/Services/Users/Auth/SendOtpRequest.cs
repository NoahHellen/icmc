using Audacia.Commands;
using MediatR;

namespace Services.Users.Auth;

/// <summary>
/// The request to send an OTP to a user's email address.
/// </summary>
/// <param name="Cid"></param>
public record SendOtpRequest(
    string Cid
) : IRequest<CommandResult>;