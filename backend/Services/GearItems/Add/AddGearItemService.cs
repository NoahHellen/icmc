using Audacia.Commands;
using Domain.Entities;
using Domain.Utils;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.GearItems.Add;

/// <summary>
/// The service that adds a gear item.
/// </summary>
public class AddGearItemService : IAddGearItemService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AddGearItemService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public AddGearItemService(
        DatabaseContext context,
        ILogger<AddGearItemService> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles adding a gear item.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<CommandResult> Handle(AddGearItemRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName} for ToughTag: {ToughTag}", nameof(AddGearItemRequest), request?.ToughTag);
        ArgumentNullException.ThrowIfNull(request);

        var normalisedToughTag = ToughTagNormaliser.Normalise(request.ToughTag);

        if (normalisedToughTag != null)
        {
            var exists = await _context.GearItems
                .AnyAsync(gi => gi.ToughTag == normalisedToughTag, cancellationToken)
                .ConfigureAwait(false);

            if (exists)
            {
                _logger.LogWarning("Gear item with ToughTag {ToughTag} already exists.", normalisedToughTag);
                return CommandResult.Failure($"Gear item with tough tag '{request.ToughTag}' already exists.");
            }
        }

        var gearItem = new GearItem
        {
            Brand = request.Brand,
            DateOfPurchase = request.DateOfPurchase,
            ExpectedReturnDate = request.ExpectedReturnDate,
            GearCategory = request.GearCategory,
            ImageUrl = request.ImageUrl,
            InspectedByUserId = request.InspectedByUserId,
            LastInspection = request.LastInspection,
            Length = request.Length,
            LentByUserId = request.LentByUserId,
            LentDate = request.LentDate,
            LentToUserId = request.LentToUserId,
            ManufacturerExpiry = request.ManufacturerExpiry,
            Model = request.Model,
            NextInspection = request.NextInspection,
            ReturnedDate = request.ReturnedDate,
            Sex = request.Sex,
            Size = request.Size,
            StorageLocation = request.StorageLocation,
            ToughTag = normalisedToughTag
        };

        _context.GearItems.Add(gearItem);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Successfully added gear item with ToughTag: {ToughTag}", normalisedToughTag);

        return CommandResult.Success();
    }

}
