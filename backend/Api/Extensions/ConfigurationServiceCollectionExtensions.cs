using Api.Constants;
using Microsoft.Extensions.Options;
using Services.Email.Configuration;
using Services.Integrations.Icu.Configuration;
using Services.Users.Auth.Configuration;

public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .Configure<IcuConfig>(configuration.GetSection(ConfigurationSections.IcuConfig))
            .Configure<AuthConfig>(configuration.GetSection(ConfigurationSections.AuthConfig))
            .Configure<EmailConfig>(configuration.GetSection(ConfigurationSections.EmailConfig))
            .AddSingleton(sp => sp.GetRequiredService<IOptions<IcuConfig>>().Value)
            .AddSingleton(sp => sp.GetRequiredService<IOptions<AuthConfig>>().Value)
            .AddSingleton(sp => sp.GetRequiredService<IOptions<EmailConfig>>().Value);

        return serviceCollection;
    }
}