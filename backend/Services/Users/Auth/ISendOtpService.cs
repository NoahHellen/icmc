using Audacia.Commands;
using MediatR;

namespace Services.Users.Auth;

/// <summary>
/// The interface for the service that sends an OTP to a user's email address.
/// </summary>
public interface ISendOtpService : IRequestHandler<SendOtpRequest, CommandResult>
{
}