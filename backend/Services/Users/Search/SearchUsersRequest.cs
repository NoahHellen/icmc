using Audacia.Commands;
using Domain.Entities;
using MediatR;
using Services.Users.Dtos;

namespace Services.Users.Search;

/// <summary>
/// The request that searches for users.
/// </summary>
public record SearchUsersRequest
(
    string? Cid,
    string? Email,
    string? FullName,
    bool? IsAdmin,
    MemberType? MemberType,
    string? Search
) : IRequest<CommandResult<UserDto[]>>;