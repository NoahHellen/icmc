using Audacia.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Services.GearItems.Image;

/// <summary>
/// Request to upload a gear item image.
/// </summary>
/// <param name="Id"></param>
/// <param name="ImageData"></param>
public record UploadGearItemImageRequest(
    int Id,
    IFormFile ImageData
) : IRequest<CommandResult>;