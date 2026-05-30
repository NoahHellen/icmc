using Services.Integrations.ImgBB;

public static class ImgBBClientServiceCollectionExtensions
{
    public static IServiceCollection AddImgBBClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IImgBBClient, ImgBBClient>();
        return serviceCollection;
    }
}
