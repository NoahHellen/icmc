using Audacia.Commands;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Logbook.Dtos;

namespace Services.Logbook.Search;

/// <summary>
/// Service to search the logbook.
/// </summary>
public class SearchLogbookService : ISearchLogbookService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<SearchLogbookService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public SearchLogbookService(DatabaseContext context, ILogger<SearchLogbookService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles searching the logbook.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommandResult<LogDto[]>> Handle(SearchLogbookRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(SearchLogbookRequest));
        ArgumentNullException.ThrowIfNull(request);

        var query = _context.Logbook
            .Include(l => l.GearItem)
            .Include(l => l.InspectedByUser)
            .Include(l => l.LentToUser)
            .Include(l => l.LentByUser)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(l =>
                (l.GearItem != null && l.GearItem.Brand != null && l.GearItem.Brand.ToLower().Contains(searchTerm)) ||
                (l.GearItem != null && l.GearItem.Model != null && l.GearItem.Model.ToLower().Contains(searchTerm)) ||
                (l.GearItem != null && l.GearItem.ToughTag != null && l.GearItem.ToughTag.ToLower().Contains(searchTerm)) ||
                (l.LentToUser != null && l.LentToUser.FullName != null && l.LentToUser.FullName.ToLower().Contains(searchTerm)) ||
                (l.LentToUser != null && l.LentToUser.Cid != null && l.LentToUser.Cid.ToLower().Contains(searchTerm)) ||
                (l.LentToUser != null && l.LentToUser.Email != null && l.LentToUser.Email.ToLower().Contains(searchTerm)) ||
                (l.LentByUser != null && l.LentByUser.FullName != null && l.LentByUser.FullName.ToLower().Contains(searchTerm)) ||
                (l.LentByUser != null && l.LentByUser.Cid != null && l.LentByUser.Cid.ToLower().Contains(searchTerm)) ||
                (l.LentByUser != null && l.LentByUser.Email != null && l.LentByUser.Email.ToLower().Contains(searchTerm)) ||
                (l.Notes != null && l.Notes.ToLower().Contains(searchTerm))
            );
        }

        var results = await query
            .Where(l =>
                (request.GearItemId == null || l.GearItemId == request.GearItemId) &&

                (request.GearItemCategory == null || (l.GearItem != null && l.GearItem.GearCategory == request.GearItemCategory)) &&
                (string.IsNullOrEmpty(request.GearItemToughTag) || (l.GearItem != null && l.GearItem.ToughTag != null && l.GearItem.ToughTag.StartsWith(request.GearItemToughTag))) &&
                (string.IsNullOrEmpty(request.GearItemModel) || (l.GearItem != null && l.GearItem.Model != null && l.GearItem.Model.StartsWith(request.GearItemModel))) &&
                (string.IsNullOrEmpty(request.GearItemBrand) || (l.GearItem != null && l.GearItem.Brand != null && l.GearItem.Brand.StartsWith(request.GearItemBrand))) &&
                (request.GearItemStorageLocation == null || (l.GearItem != null && l.GearItem.StorageLocation == request.GearItemStorageLocation)) &&
                (request.InspectedByUserId == null || l.InspectedByUserId == request.InspectedByUserId) &&
                (string.IsNullOrEmpty(request.InspectedByUserFullName) || (l.InspectedByUser != null && l.InspectedByUser.FullName != null && l.InspectedByUser.FullName.StartsWith(request.InspectedByUserFullName))) &&
                (string.IsNullOrEmpty(request.InspectedByUserCid) || (l.InspectedByUser != null && l.InspectedByUser.Cid != null && l.InspectedByUser.Cid.StartsWith(request.InspectedByUserCid))) &&
                (string.IsNullOrEmpty(request.InspectedByUserEmail) || (l.InspectedByUser != null && l.InspectedByUser.Email != null && l.InspectedByUser.Email.StartsWith(request.InspectedByUserEmail))) &&
                (request.LentToUserId == null || l.LentToUserId == request.LentToUserId) &&
                (string.IsNullOrEmpty(request.LentToUserFullName) || (l.LentToUser != null && l.LentToUser.FullName != null && l.LentToUser.FullName.StartsWith(request.LentToUserFullName))) &&
                (string.IsNullOrEmpty(request.LentToUserCid) || (l.LentToUser != null && l.LentToUser.Cid != null && l.LentToUser.Cid.StartsWith(request.LentToUserCid))) &&
                (string.IsNullOrEmpty(request.LentToUserEmail) || (l.LentToUser != null && l.LentToUser.Email != null && l.LentToUser.Email.StartsWith(request.LentToUserEmail))) &&
                (request.LentByUserId == null || l.LentByUserId == request.LentByUserId) &&
                (string.IsNullOrEmpty(request.LentByUserFullName) || (l.LentByUser != null && l.LentByUser.FullName != null && l.LentByUser.FullName.StartsWith(request.LentByUserFullName))) &&
                (string.IsNullOrEmpty(request.LentByUserCid) || (l.LentByUser != null && l.LentByUser.Cid != null && l.LentByUser.Cid.StartsWith(request.LentByUserCid))) &&
                (string.IsNullOrEmpty(request.LentByUserEmail) || (l.LentByUser != null && l.LentByUser.Email != null && l.LentByUser.Email.StartsWith(request.LentByUserEmail))) &&
                (request.LentDate == null || l.LentDate == request.LentDate) &&
                (request.ReturnedDate == null || l.ReturnedDate == request.ReturnedDate) &&
                (string.IsNullOrEmpty(request.Notes) || (l.Notes != null && l.Notes.StartsWith(request.Notes)))
            )
            .OrderByDescending(l => l.ReturnedDate)
            .Select(l => new LogDto(
                l.Id,
                l.GearItemId,
                l.GearItem != null ? l.GearItem.GearCategory : null,
                l.GearItem != null ? l.GearItem.ToughTag : null,
                l.GearItem != null ? l.GearItem.Model : null,
                l.GearItem != null ? l.GearItem.Brand : null,
                l.GearItem != null ? l.GearItem.StorageLocation : null,
                l.InspectedByUserId,
                l.InspectedByUser != null ? l.InspectedByUser.FullName : null,
                l.InspectedByUser != null ? l.InspectedByUser.Cid : null,
                l.InspectedByUser != null ? l.InspectedByUser.Email : null,
                l.LentToUserId,
                l.LentToUser != null ? l.LentToUser.FullName : null,
                l.LentToUser != null ? l.LentToUser.Cid : null,
                l.LentToUser != null ? l.LentToUser.Email : null,
                l.LentByUserId,
                l.LentByUser != null ? l.LentByUser.FullName : null,
                l.LentByUser != null ? l.LentByUser.Cid : null,
                l.LentByUser != null ? l.LentByUser.Email : null,
                l.LentDate,
                l.ReturnedDate,
                l.Notes
            ))
            .ToArrayAsync(cancellationToken);

        return CommandResult.WithResult(results);
    }
}