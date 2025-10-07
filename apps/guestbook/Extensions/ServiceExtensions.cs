using Guestbook.Services;

namespace Guestbook.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddGuestbookServices(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbService>();

        // Session support for admin authentication
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Path = "/guestbook";
        });

        return services;
    }
}