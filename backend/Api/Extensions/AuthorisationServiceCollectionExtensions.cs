using Microsoft.AspNetCore.Authorization;

public static class AuthorisationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorisation(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return serviceCollection;
    }
}