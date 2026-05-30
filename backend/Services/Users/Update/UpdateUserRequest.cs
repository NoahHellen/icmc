using Audacia.Commands;
using Domain.Entities;
using MediatR;


namespace Services.Users.Update;

/// <summary>
/// A request to update a user.
/// </summary>
/// <param name="Id"></param>
/// <param name="Cid"></param>
/// <param name="FullName"></param>
/// <param name="Email"></param>
/// <param name="IsAdmin"></param>
/// <param name="MemberType"></param>
public record UpdateUserRequest(int Id, string? Cid, string? FullName, string? Email, bool? IsAdmin, MemberType? MemberType) : IRequest<CommandResult>
{
}
