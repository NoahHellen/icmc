namespace Services.Users.Auth.Configuration;

/// <summary>
/// Represents the 'AuthConfig' section of the app settings.
/// </summary>
public class AuthConfig
{
    /// <summary>
    /// The JWT secret key.
    /// </summary>
    public string? JwtSecretKey { get; set; }
}