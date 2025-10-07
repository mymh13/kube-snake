using Guestbook.Endpoints;

namespace Guestbook.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapGuestbookEndpoints(this WebApplication app)
    {
        // Add session middleware
        app.UseSession();

        // Map endpoint groups
        app.MapMessageEndpoints();
        app.MapAuthEndpoints();

        return app;
    }
}