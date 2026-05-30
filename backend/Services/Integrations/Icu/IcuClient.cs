using System.Text.Json;
using Microsoft.Extensions.Logging;
using Services.Integrations.Icu.Configuration;
using Services.Integrations.Icu.Models;

namespace Services.Integrations.Icu;

/// <summary>
/// The client for ICU.
/// </summary>
public class IcuClient : IIcuClient
{
    private readonly HttpClient _httpClient;
    private readonly IcuConfig _config;
    private readonly ILogger<IcuClient> _logger;

    /// <summary>
    /// The constructor for the class.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="config"></param>
    /// <param name="logger"></param>
    public IcuClient(HttpClient httpClient, IcuConfig config, ILogger<IcuClient> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    private HttpRequestMessage CreateRequest(string path)
    {
        var centreCode = 116;

        var uri = new Uri($"https://eactivities.union.ic.ac.uk/API/CSP/{centreCode}/reports/{path}");

        var request = new HttpRequestMessage(HttpMethod.Get, uri);

        request.Headers.Add("X-API-Key", _config.ApiKey);

        return request;
    }

    /// <summary>
    /// Method to get ICMC committee members from the ICU client.
    /// </summary>
    /// <returns></returns>
    public async Task<List<IcuCommitteeMember>> GetCommitteeMembersFromIcuAsync()
    {
        _logger.LogDebug("Executing {MethodName}", nameof(GetCommitteeMembersFromIcuAsync));
        using var request = CreateRequest("committee");

        try
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ICU committee members fetch failed with status code {StatusCode}.", response.StatusCode);
                return new List<IcuCommitteeMember>();
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("ICU committee members fetch returned empty content.");
                return new List<IcuCommitteeMember>();
            }

            var committeeMembers = JsonSerializer.Deserialize<List<IcuCommitteeMember>>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
            return committeeMembers ?? new List<IcuCommitteeMember>();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error getting committee members from ICU.");
            return new List<IcuCommitteeMember>();
        }
    }

    /// <summary>
    /// Method to get ICMC members from the ICU client.
    /// </summary>
    /// <returns></returns>
    public async Task<List<IcuMember>> GetMembersFromIcuAsync()
    {
        _logger.LogDebug("Executing {MethodName}", nameof(GetMembersFromIcuAsync));
        using var request = CreateRequest("members");

        try
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ICU members fetch failed with status code {StatusCode}.", response.StatusCode);
                return new List<IcuMember>();
            }
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("ICU members fetch returned empty content.");
                return new List<IcuMember>();
            }

            var members = JsonSerializer.Deserialize<List<IcuMember>>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
            return members ?? new List<IcuMember>();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error getting members from ICU.");
            return new List<IcuMember>();
        }
    }
}