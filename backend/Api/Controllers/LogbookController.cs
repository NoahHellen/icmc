using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Logbook.Add;
using Services.Logbook.Search;

namespace Api.Controllers;

/// <summary>
/// Controller for requests relating to gear items.
/// </summary>
[ApiController]
[Route("logbook")]
public class LogbookController : ControllerBase
{
    private readonly IAddLogService _addLogService;
    private readonly ISearchLogbookService _searchLogbookService;
    private readonly ILogger<LogbookController> _logger;

    public LogbookController(IAddLogService addLogService, ISearchLogbookService searchLogbookService, ILogger<LogbookController> logger)
    {
        _addLogService = addLogService;
        _searchLogbookService = searchLogbookService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddLog(AddLogRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(AddLogRequest));
        var result = await _addLogService.Handle(request, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Created();
        }
        return BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> SearchLogbook([FromQuery] SearchLogbookRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(SearchLogbookRequest));
        var result = await _searchLogbookService.Handle(request, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Ok(result.Output);
        }

        return BadRequest(result.Errors);
    }
}