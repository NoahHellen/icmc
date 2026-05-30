using Audacia.Commands;
using Domain.Entities;
using EntityFramework;
using Microsoft.Extensions.Logging;

namespace Services.Logbook.Add;

/// <summary>
/// The service to handle adding a log to the logbook.
/// </summary>
public class AddLogService : IAddLogService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<AddLogService> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public AddLogService(DatabaseContext context, ILogger<AddLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles adding a log.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommandResult> Handle(AddLogRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName} for GearItemId: {GearItemId}", nameof(AddLogRequest), request?.GearItemId);
        ArgumentNullException.ThrowIfNull(request);

        var log = new Log
        {
            GearItemId = request.GearItemId,
            InspectedByUserId = request.InspectedByUserId,
            LentToUserId = request.LentToUserId,
            LentByUserId = request.LentByUserId,
            LentDate = request.LentDate,
            ReturnedDate = request.ReturnedDate,
            Notes = request.Notes
        };

        _context.Logbook.Add(log);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Successfully added log entry for GearItemId: {GearItemId}", request.GearItemId);

        return CommandResult.Success();
    }
}