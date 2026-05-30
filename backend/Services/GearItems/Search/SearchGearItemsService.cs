using Audacia.Commands;
using Domain.Utils;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.GearItems.Dtos;

namespace Services.GearItems.Search;

/// <summary>
/// The service that searches for all gear items.
/// </summary>
public class SearchGearItemsService : ISearchGearItemsService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<SearchGearItemsService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public SearchGearItemsService(DatabaseContext context, ILogger<SearchGearItemsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// The asynchronous method that searches for all gear items.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<CommandResult<GearItemDto[]>> Handle(SearchGearItemsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(SearchGearItemsRequest));
        ArgumentNullException.ThrowIfNull(request);

        var query = _context.GearItems.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            var normalisedSearchTerm = ToughTagNormaliser.Normalise(request.Search);
            query = query.Where(gi =>
                (gi.Brand != null && gi.Brand.ToLower().Contains(searchTerm)) ||
                (gi.Model != null && gi.Model.ToLower().Contains(searchTerm)) ||
                (gi.ToughTag != null && normalisedSearchTerm != null && gi.ToughTag.Contains(normalisedSearchTerm))
            );
        }

        var filterToughTag = ToughTagNormaliser.Normalise(request.ToughTag);

        query = query.Where(gi =>
            (string.IsNullOrEmpty(request.Brand) || (gi.Brand != null && gi.Brand.ToLower().Contains(request.Brand.ToLower()))) &&
            (request.DateOfPurchase == null || gi.DateOfPurchase == request.DateOfPurchase) &&
            (request.ExpectedReturnDate == null || gi.ExpectedReturnDate == request.ExpectedReturnDate) &&
            (request.GearCategory == null || gi.GearCategory == request.GearCategory) &&
            (request.InspectedByUserId == null || gi.InspectedByUserId == request.InspectedByUserId) &&
            (request.LastInspection == null || gi.LastInspection == request.LastInspection) &&
            (request.Length == null || gi.Length == request.Length) &&
            (request.LentDate == null || gi.LentDate == request.LentDate) &&
            (request.LentToUserId == null || gi.LentToUserId == request.LentToUserId) &&
            (request.LentByUserId == null || gi.LentByUserId == request.LentByUserId) &&
            (request.ManufacturerExpiry == null || gi.ManufacturerExpiry == request.ManufacturerExpiry) &&
            (string.IsNullOrEmpty(request.Model) || (gi.Model != null && gi.Model.ToLower().Contains(request.Model.ToLower()))) &&
            (request.NextInspection == null || gi.NextInspection == request.NextInspection) &&
            (request.ReturnedDate == null || gi.ReturnedDate == request.ReturnedDate) &&
            (request.Size == null || gi.Size == request.Size) &&
            (request.Sex == null || gi.Sex == request.Sex) &&
            (request.StorageLocation == null || gi.StorageLocation == request.StorageLocation) &&
            (string.IsNullOrEmpty(request.ToughTag) || (gi.ToughTag != null && filterToughTag != null && gi.ToughTag.Contains(filterToughTag)))
        );

        var result = await query
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
            .ToArrayAsync(cancellationToken);

        _logger.LogInformation("Search found {Count} gear items matching the criteria.", result.Length);

        return CommandResult.WithResult(result);
    }
}
