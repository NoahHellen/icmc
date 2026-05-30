using Audacia.Commands;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.GearItems.Dtos;

namespace Services.GearItems.Get;

/// <summary>
/// The service that gets a gear item.
/// </summary>
public class GetGearItemService : IGetGearItemService
{
  private readonly DatabaseContext _context;
  private readonly ILogger<GetGearItemService> _logger;

  /// <summary>
  /// Constructor for the class.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="logger"></param>
  public GetGearItemService(
    DatabaseContext context,
    ILogger<GetGearItemService> logger
  )
  {
    _context = context;
    _logger = logger;
  }
  /// <summary>
  /// Method that handles the asynchronous operation.
  /// </summary>
  public async Task<CommandResult<GearItemDto>> Handle(GetGearItemRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName}", nameof(GetGearItemRequest));
    ArgumentNullException.ThrowIfNull(request);

    var result = await _context.GearItems
        .Where(gi => gi.Id == request.Id)
        .Select(gi => new GearItemDto(
          Id: gi.Id,
          Brand: gi.Brand,
          DateOfPurchase: gi.DateOfPurchase,
          ExpectedReturnDate: gi.ExpectedReturnDate,
          GearCategory: gi.GearCategory,
          ImageUrl: gi.ImageUrl,
          InspectedByUserId: gi.InspectedByUserId,
          LastInspection: gi.LastInspection,
          Length: gi.Length,
          LentByUserId: gi.LentByUserId,
          LentByUserFullName: gi.LentByUser != null ? gi.LentByUser.FullName : null,
          LentDate: gi.LentDate,
          LentToUserId: gi.LentToUserId,
          LentToUserFullName: gi.LentToUser != null ? gi.LentToUser.FullName : null,
          ManufacturerExpiry: gi.ManufacturerExpiry,
          Model: gi.Model,
          NextInspection: gi.NextInspection,
          ReturnedDate: gi.ReturnedDate,
          Sex: gi.Sex,
          Size: gi.Size,
          StorageLocation: gi.StorageLocation,
          ToughTag: gi.ToughTag
        ))
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
    if (result == null)
    {
      return CommandResult.Failure<GearItemDto>("No gear item found");
    }

    return CommandResult.WithResult(result);
  }
}