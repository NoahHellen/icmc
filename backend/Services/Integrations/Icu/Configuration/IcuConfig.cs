namespace Services.Integrations.Icu.Configuration;

/// <summary>
/// Represents the 'IcuConfig' section of the app settings.
/// </summary>
public class IcuConfig
{
    /// <summary>
    /// The API key of the ICU client.
    /// </summary>
    public string? ApiKey { get; set; }
}