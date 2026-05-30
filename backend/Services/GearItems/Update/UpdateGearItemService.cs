using Audacia.Commands;
using Domain.Utils;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Integrations.ImgBB;

namespace Services.GearItems.Update;

/// <summary>
/// Handles updating a gear item.
/// </summary>
public class UpdateGearItemService : IUpdateGearItemService
{
    private readonly DatabaseContext _context;
    private readonly IImgBBClient _imgBBClient;
    private readonly ILogger<UpdateGearItemService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="imgBBClient"></param>
    /// <param name="logger"></param>
    public UpdateGearItemService(
        DatabaseContext context,
        IImgBBClient imgBBClient,
        ILogger<UpdateGearItemService> logger
    )
    {
        _context = context;
        _imgBBClient = imgBBClient;
        _logger = logger;
    }

    /// <summary>
    /// Handles updating a gear item.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<CommandResult> Handle(UpdateGearItemRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName} for GearItemId: {GearItemId}", nameof(UpdateGearItemRequest), request?.Id);
        ArgumentNullException.ThrowIfNull(request);

        var gearItem = await _context.GearItems
            .FirstOrDefaultAsync(gi => gi.Id == request.Id, cancellationToken)
            .ConfigureAwait(false);

        if (gearItem == null)
        {
            _logger.LogWarning("Gear item with ID {GearItemId} not found for update.", request.Id);
            return CommandResult.Failure("Gear item not found");
        }

        var normalisedToughTag = ToughTagNormaliser.Normalise(request.ToughTag);

        if (normalisedToughTag != null && normalisedToughTag != gearItem.ToughTag)
        {
            var exists = await _context.GearItems
                .AnyAsync(gi => gi.ToughTag == normalisedToughTag && gi.Id != request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (exists)
            {
                _logger.LogWarning("Gear item with ToughTag {ToughTag} already exists.", normalisedToughTag);
                return CommandResult.Failure($"Gear item with tough tag '{request.ToughTag}' already exists.");
            }
        }

        gearItem.Brand = request.Brand ?? gearItem.Brand;
        gearItem.DateOfPurchase = request.DateOfPurchase ?? gearItem.DateOfPurchase;
        gearItem.ExpectedReturnDate = request.ExpectedReturnDate;
        gearItem.GearCategory = request.GearCategory ?? gearItem.GearCategory;
        gearItem.ImageUrl = request.ImageUrl ?? gearItem.ImageUrl;
        gearItem.InspectedByUserId = request.InspectedByUserId;
        gearItem.LastInspection = request.LastInspection;
        gearItem.Length = request.Length ?? gearItem.Length;
        gearItem.LentByUserId = request.LentByUserId;
        gearItem.LentDate = request.LentDate;
        gearItem.LentToUserId = request.LentToUserId;
        gearItem.ManufacturerExpiry = request.ManufacturerExpiry;
        gearItem.Model = request.Model ?? gearItem.Model;
        gearItem.NextInspection = request.NextInspection;
        gearItem.ReturnedDate = request.ReturnedDate;
        gearItem.Sex = request.Sex ?? gearItem.Sex;
        gearItem.Size = request.Size ?? gearItem.Size;
        gearItem.StorageLocation = request.StorageLocation ?? gearItem.StorageLocation;
        gearItem.ToughTag = normalisedToughTag ?? gearItem.ToughTag;

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Successfully updated gear item with ID: {GearItemId}", request.Id);

        return CommandResult.Success();
    }
}
