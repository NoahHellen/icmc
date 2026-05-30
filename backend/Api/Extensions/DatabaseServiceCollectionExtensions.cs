using EntityFramework;
using Microsoft.EntityFrameworkCore;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseAccess(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        return serviceCollection.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DatabaseContext"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)));
    }
}