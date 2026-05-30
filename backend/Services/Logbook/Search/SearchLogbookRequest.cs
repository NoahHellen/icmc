using Audacia.Commands;
using Domain.Entities;
using MediatR;
using Services.Logbook.Dtos;

namespace Services.Logbook.Search;

/// <summary>
/// Request to search the logbook.
/// </summary>
public record SearchLogbookRequest
(
    string? Search,
    int? GearItemId,
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
 ) : IRequest<CommandResult<LogDto[]>>;
