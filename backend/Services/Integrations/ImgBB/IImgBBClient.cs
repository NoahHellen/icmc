using Microsoft.AspNetCore.Http;

namespace Services.Integrations.ImgBB;

/// <summary>
/// The interface for the ImgBB client.
/// </summary>
public interface IImgBBClient
{
    /// <summary>
    /// A method that uploads the binary image to ImgBB and returns the public image url.
    /// </summary>
    /// <param name="imageData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> UploadGearItemImage(IFormFile imageData, CancellationToken cancellationToken);

    /// <summary>
    /// A method that deletes an image from ImgBB given its public url.
    /// </summary>
    /// <param name="imageUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteImage(string imageUrl, CancellationToken cancellationToken);
}
