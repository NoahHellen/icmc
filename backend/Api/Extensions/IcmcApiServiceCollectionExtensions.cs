using Services.GearItems.Add;
using Services.GearItems.Delete;
using Services.GearItems.Get;
using Services.GearItems.Image;
using Services.GearItems.Search;
using Services.GearItems.Update;
using Services.Logbook.Add;
using Services.Logbook.Search;
using Services.Users.Add;
using Services.Users.Auth;
using Services.Users.Delete;
using Services.Users.Get;
using Services.Users.Search;
using Services.Users.Update;

public static class IcmcApiServiceCollectionExtensions
{
    public static IServiceCollection AddIcmcApiServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddGearItemsServices()
            .AddUsersServices()
            .AddLogbookServices();
    }

    public static IServiceCollection AddGearItemsServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<IAddGearItemService, AddGearItemService>()
            .AddTransient<IDeleteGearItemService, DeleteGearItemService>()
            .AddTransient<IGetGearItemService, GetGearItemService>()
            .AddTransient<ISearchGearItemsService, SearchGearItemsService>()
            .AddTransient<IUpdateGearItemService, UpdateGearItemService>()
            .AddTransient<IUploadGearItemImageService, UploadGearItemImageService>();
    }

    public static IServiceCollection AddUsersServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<IAddUserService, AddUserService>()
            .AddTransient<ISearchUsersService, SearchUsersService>()
            .AddTransient<IDeleteUserService, DeleteUserService>()
            .AddTransient<IGetUserService, GetUserService>()
            .AddTransient<IUpdateUserService, UpdateUserService>()
            .AddTransient<ISendOtpService, SendOtpService>()
            .AddTransient<ICreateJwtService, CreateJwtService>();
    }

    public static IServiceCollection AddLogbookServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<IAddLogService, AddLogService>()
            .AddTransient<ISearchLogbookService, SearchLogbookService>();
    }
}
