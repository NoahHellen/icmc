using Services.Integrations.Icu;

public static class IcuClientServiceCollectionExtensions
{
    public static IServiceCollection AddIcuClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IIcuClient, IcuClient>();

        return serviceCollection;
    }
}