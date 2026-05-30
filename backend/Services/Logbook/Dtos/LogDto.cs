using Domain.Entities;

namespace Services.Logbook.Dtos;

/// <summary>
/// DTO for a log entry.
/// </summary>
public record LogDto(
    int Id,
    int GearItemId,
    GearCategory? GearItemCategory,
    string? GearItemToughTag,
    string? GearItemModel,
    string? GearItemBrand,
    StorageLocation? GearItemStorageLocation,
    int? InspectedByUserId,
    string? InspectedByUserFullName,
    string? InspectedByUserCid,
    string? InspectedByUserEmail,
    int? LentToUserId,
    string? LentToUserFullName,
    string? LentToUserCid,
    string? LentToUserEmail,
    int? LentByUserId,
    string? LentByUserFullName,
    string? LentByUserCid,
    string? LentByUserEmail,
    DateTimeOffset? LentDate,
    DateTimeOffset? ReturnedDate,
    string? Notes
);
