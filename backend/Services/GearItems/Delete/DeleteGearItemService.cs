using Audacia.Commands;
using EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Integrations.ImgBB;

namespace Services.GearItems.Delete;

/// <summary>
/// The service that deletes a gear item.
/// </summary>
public class DeleteGearItemService : IDeleteGearItemService
{
  private readonly DatabaseContext _context;
  private readonly IImgBBClient _imgBBClient;
  private readonly ILogger<DeleteGearItemService> _logger;

  /// <summary>
  /// The constructor for the class.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="imgBBClient"></param>
  /// <param name="logger"></param>
  public DeleteGearItemService(
    DatabaseContext context,
    IImgBBClient imgBBClient,
    ILogger<DeleteGearItemService> logger
  )
  {
    _context = context;
    _imgBBClient = imgBBClient;
    _logger = logger;
  }
  /// <summary>
  /// Handles deleting a gear item.
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<CommandResult> Handle(DeleteGearItemRequest request, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Handling {RequestName} for GearItemId: {GearItemId}", nameof(DeleteGearItemRequest), request?.Id);
    ArgumentNullException.ThrowIfNull(request);

    var result = await _context.GearItems
        .FirstOrDefaultAsync(gi => gi.Id == request.Id)
        .ConfigureAwait(false);

    if (result == null)
    {
      _logger.LogWarning("Gear item with ID {GearItemId} not found for deletion.", request.Id);
      return CommandResult.Failure("Gear item not found.");
    }

    if (!string.IsNullOrEmpty(result.ImageUrl))
    {
        _logger.LogInformation("Deleting image from ImgBB for deleted GearItemId: {GearItemId}. ImageUrl: {ImageUrl}", request.Id, result.ImageUrl);
        await _imgBBClient.DeleteImage(result.ImageUrl, cancellationToken).ConfigureAwait(false);
    }

    _context.GearItems.Remove(result);

    await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    _logger.LogInformation("Successfully deleted gear item with ID: {GearItemId}", request.Id);

    return CommandResult.Success();

  }
}