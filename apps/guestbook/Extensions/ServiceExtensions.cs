using Guestbook.Services;

namespace Guestbook.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddGuestbookServices(this IServiceCollection services)
    {
        // Register MongoDB service
        services.AddSingleton<MongoDbService>();

        // Add session support for admin authentication
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromHours(1);
        });

        return services;
    }
}