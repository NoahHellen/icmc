using Audacia.Commands;
using MediatR;

namespace Services.GearItems.Image;

/// <summary>
/// The interface for uploading a gear item image to Catbox.
/// </summary>
public interface IUploadGearItemImageService : IRequestHandler<UploadGearItemImageRequest, CommandResult>
{
}