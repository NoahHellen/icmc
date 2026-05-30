using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Services.Integrations.ImgBB;

/// <summary>
/// The client for ImgBB.
/// </summary>
public class ImgBBClient : IImgBBClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ImgBBClient> _logger;
    private readonly string _apiKey;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public ImgBBClient(HttpClient httpClient, ILogger<ImgBBClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["ImgBB:ApiKey"] ?? string.Empty;
    }

    /// <summary>
    /// The method to upload an image to ImgBB and receive the public image url.
    /// </summary>
    /// <param name="imageData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> UploadGearItemImage(IFormFile imageData, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing {MethodName}", nameof(UploadGearItemImage));

        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("ImgBB API Key is missing. Please provide it in configuration.");
            return string.Empty;
        }

        var uri = new Uri($"https://api.imgbb.com/1/upload?key={_apiKey}");

        using var content = new MultipartFormDataContent();

        using var fileStream = imageData.OpenReadStream();
        var streamContent = new StreamContent(fileStream);

        if (!string.IsNullOrEmpty(imageData.ContentType))
        {
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageData.ContentType);
        }

        content.Add(streamContent, "image", imageData.FileName);

        try
        {
            var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ImgBB upload failed with status code {StatusCode}. Response: {Response}", response.StatusCode, result);
                return string.Empty;
            }

            using var document = JsonDocument.Parse(result);
            if (document.RootElement.TryGetProperty("data", out var data) && 
                data.TryGetProperty("url", out var url))
            {
                var imageUrl = url.GetString() ?? string.Empty;
                _logger.LogInformation("Successfully uploaded image to ImgBB: {ImageUrl}", imageUrl);
                return imageUrl;
            }

            _logger.LogWarning("ImgBB upload returned an unexpected response structure: {Response}", result);
            return string.Empty;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error uploading gear item image to ImgBB.");
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public async Task DeleteImage(string imageUrl, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing {MethodName} for ImageUrl: {ImageUrl}", nameof(DeleteImage), imageUrl);

        if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(_apiKey))
        {
            return;
        }

        try
        {
            var uri = new Uri(imageUrl);
            if (!uri.Host.Contains("ibb.co"))
            {
                _logger.LogWarning("ImageUrl is not an ImgBB URL, skipping deletion: {ImageUrl}", imageUrl);
                return;
            }

            var segments = uri.Segments;
            var id = segments.Length > 1 ? segments[1].Trim('/') : string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Could not extract ImgBB image ID from URL: {ImageUrl}", imageUrl);
                return;
            }

            var deleteUri = new Uri($"https://api.imgbb.com/1/image/{id}?key={_apiKey}");

            var response = await _httpClient.DeleteAsync(deleteUri, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogWarning("ImgBB deletion failed for ID {ImageId}. Status: {StatusCode}. Response: {Response}", id, response.StatusCode, result);
                return;
            }

            _logger.LogInformation("Successfully requested deletion of ImgBB image: {ImageId}", id);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error deleting image from ImgBB for URL: {ImageUrl}", imageUrl);
        }
    }
}
