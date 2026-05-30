using System.Text.Json.Serialization;

namespace Services.Integrations.Icu.Models;

/// <summary>
/// The interface for an ICMC member from the ICU client.
/// </summary>
public class IcuMember
{
    /// <summary>
    /// The first name of the member.
    /// </summary>
    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }
    /// <summary>
    /// The surname of the member.
    /// </summary>
    [JsonPropertyName("Surname")]
    public string? Surname { get; set; }
    /// <summary>
    /// The CID of the member.
    /// </summary>
    [JsonPropertyName("CID")]
    public string? Cid { get; set; }
    /// <summary>
    /// The post name of the member.
    /// </summary>
    [JsonPropertyName("Email")]
    public string? Email { get; set; }
    /// <summary>
    /// The login of the member.
    /// </summary>
    [JsonPropertyName("Login")]
    public string? Login { get; set; }
    /// <summary>
    /// The order number of the member.
    /// </summary>
    [JsonPropertyName("OrderNo")]
    public int OrderNo { get; set; }
    /// <summary>
    /// The member type.
    /// </summary>
    [JsonPropertyName("MemberType")]
    public string? MemberType { get; set; }
}