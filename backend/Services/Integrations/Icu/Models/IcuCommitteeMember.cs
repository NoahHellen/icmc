using System.Text.Json.Serialization;

namespace Services.Integrations.Icu.Models;

/// <summary>
/// The interface for an ICMC committee member from the Icu client.
/// </summary>
public class IcuCommitteeMember
{
    /// <summary>
    /// The first name of the committee member.
    /// </summary>
    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }
    /// <summary>
    /// The surname of the committee member.
    /// </summary>
    [JsonPropertyName("Surname")]
    public string? Surname { get; set; }
    /// <summary>
    /// The CID of the committee member.
    /// </summary>
    [JsonPropertyName("CID")]
    public string? Cid { get; set; }
    /// <summary>
    /// The post name of the committee member.
    /// </summary>
    [JsonPropertyName("PostName")]
    public string? PostName { get; set; }
    /// <summary>
    /// The phone number of the committee member.
    /// </summary>
    [JsonPropertyName("PhoneNo")]
    public string? PhoneNo { get; set; }
    /// <summary>
    /// The start date of the committee member.
    /// </summary>
    [JsonPropertyName("StartDate")]
    public string? StartDate { get; set; }
    /// <summary>
    /// The end date of the committee member.
    /// </summary>
    [JsonPropertyName("EndDate")]
    public string? EndDate { get; set; }
}