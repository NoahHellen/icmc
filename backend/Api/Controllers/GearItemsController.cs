using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.GearItems.Add;
using Services.GearItems.Delete;
using Services.GearItems.Get;
using Services.GearItems.Image;
using Services.GearItems.Search;
using Services.GearItems.Update;

namespace Api.Controllers;

/// <summary>
/// Controller for requests relating to gear items.
/// </summary>
[ApiController]
[Route("gear-items")]
public class GearItemsController : ControllerBase
{
    private readonly IGetGearItemService _getGearItemService;
    private readonly ISearchGearItemsService _searchGearItemsService;
    private readonly IDeleteGearItemService _deleteGearItemService;
    private readonly IAddGearItemService _addGearItemService;
    private readonly IUpdateGearItemService _updateGearItemService;
    private readonly IUploadGearItemImageService _uploadGearItemImageService;
    private readonly ILogger<GearItemsController> _logger;

    public GearItemsController(
      IGetGearItemService getGearItemService,
      ISearchGearItemsService searchGearItemsService,
      IDeleteGearItemService deleteGearItemService,
      IAddGearItemService addGearItemService,
      IUpdateGearItemService updateGearItemService,
      IUploadGearItemImageService uploadGearItemImageService,
      ILogger<GearItemsController> logger
    )
    {
        _getGearItemService = getGearItemService;
        _searchGearItemsService = searchGearItemsService;
        _deleteGearItemService = deleteGearItemService;
        _addGearItemService = addGearItemService;
        _updateGearItemService = updateGearItemService;
        _uploadGearItemImageService = uploadGearItemImageService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGearItem([FromRoute] int id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(GetGearItemRequest));
        var request = new GetGearItemRequest(id);
        var result = await _getGearItemService.Handle(request, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Ok(result.Output);
        }

        return NotFound(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> SearchGearItems([FromQuery] SearchGearItemsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(SearchGearItemsRequest));
        var result = await _searchGearItemsService.Handle(request, cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            return Ok(result.Output);
        }
        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGearItem(int id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(DeleteGearItemRequest));
        var request = new DeleteGearItemRequest(id);
        var result = await _deleteGearItemService.Handle(request, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> AddGearItem(AddGearItemRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(AddGearItemRequest));
        var result = await _addGearItemService.Handle(request, cancellationToken);

        if (result.IsSuccess)
        {
            return Created();
        }
        return BadRequest(result.Errors);
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateGearItem(int id, UpdateGearItemRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(UpdateGearItemRequest));
        if (id != request.Id)
        {
            return BadRequest("Id in URL must match Id in request body");
        }

        var result = await _updateGearItemService.Handle(request, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadGearItemImage([FromForm] UploadGearItemImageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling {RequestName}", nameof(UploadGearItemImageRequest));
        var result = await _uploadGearItemImageService.Handle(request, cancellationToken);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }
}