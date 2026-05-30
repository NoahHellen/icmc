using Audacia.Commands;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Integrations.ImgBB;

namespace Services.GearItems.Image;

/// <summary>
/// The service that uploads a gear item image to ImgBB and updates the gear item with the image url.
/// </summary>
public class UploadGearItemImageService : IUploadGearItemImageService
{
    private readonly IImgBBClient _imgBBClient;
    private readonly DatabaseContext _context;
    private readonly ILogger<UploadGearItemImageService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="imgBBClient"></param>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public UploadGearItemImageService(IImgBBClient imgBBClient, DatabaseContext context, ILogger<UploadGearItemImageService> logger)
    {
        _imgBBClient = imgBBClient;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles uploading the image and updating the gear item.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommandResult> Handle(UploadGearItemImageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName} for GearItemId: {GearItemId}", nameof(UploadGearItemImageRequest), request?.Id);
        ArgumentNullException.ThrowIfNull(request);

        var gearItem = await _context.GearItems
            .FirstOrDefaultAsync(gi => gi.Id == request.Id, cancellationToken)
            .ConfigureAwait(false);

        if (gearItem == null)
        {
            _logger.LogWarning("Gear item with ID {GearItemId} not found for image upload.", request.Id);
            return CommandResult.Failure("Gear item not found");
        }

        if (!string.IsNullOrEmpty(gearItem.ImageUrl))
        {
            _logger.LogInformation("Deleting existing image for GearItemId: {GearItemId}. ImageUrl: {ImageUrl}", request.Id, gearItem.ImageUrl);
            await _imgBBClient.DeleteImage(gearItem.ImageUrl, cancellationToken).ConfigureAwait(false);
        }

        var imageUrl = await _imgBBClient.UploadGearItemImage(request.ImageData, cancellationToken);

        if (string.IsNullOrEmpty(imageUrl))
        {
            _logger.LogWarning("Failed to upload image to ImgBB for GearItemId: {GearItemId}.", request.Id);
            return CommandResult.Failure("Failed to upload image to ImgBB");
        }

        gearItem.ImageUrl = imageUrl;

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Successfully uploaded image and updated GearItemId: {GearItemId}. ImageUrl: {ImageUrl}", request.Id, imageUrl);

        return CommandResult.Success();

    }
}