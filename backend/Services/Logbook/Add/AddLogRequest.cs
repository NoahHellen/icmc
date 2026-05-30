using Audacia.Commands;
using Domain.Entities;
using MediatR;

namespace Services.Logbook.Add;

/// <summary>
/// The request to add a log to the logbook.
/// </summary>
/// <param name="GearItemId"></param>
/// <param name="InspectedByUserId"></param>
/// <param name="LentToUserId"></param>
/// <param name="LentByUserId"></param>
/// <param name="LentDate"></param>
/// <param name="ReturnedDate"></param>
/// <param name="Notes"></param>
public record AddLogRequest(
    int GearItemId,
    int? InspectedByUserId,
    int? LentToUserId,
    int? LentByUserId,
    DateTimeOffset? LentDate,
    DateTimeOffset? ReturnedDate,
    string? Notes
) : IRequest<CommandResult>;