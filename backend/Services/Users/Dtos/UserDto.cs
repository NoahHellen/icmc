using Domain.Entities;

namespace Services.Users.Dtos;

/// <summary>
/// DTO for a user.
/// </summary>
/// <param name="Id"></param>
/// <param name="Cid"></param>
/// <param name="Email"></param>
/// <param name="FullName"></param>
/// <param name="IsAdmin"></param>
/// <param name="MemberType"></param>
public record UserDto(
  int Id,
  string? Cid,
  string? Email,
  string? FullName,
  bool? IsAdmin,
  MemberType? MemberType
);