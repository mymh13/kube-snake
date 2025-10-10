using Microsoft.Extensions.Caching.StackExchangeRedis;
using Guestbook.Services;

namespace Guestbook.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddGuestbookServices(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbService>();

        // Session support for admin authentication
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "guestbook-redis-master:6379"; // Use your Redis service name and port
        });
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Path = "/guestbook";
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        return services;
    }
}