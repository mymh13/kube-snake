using Guestbook.Endpoints;

namespace Guestbook.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapGuestbookEndpoints(this WebApplication app)
    {
        // Enable sessions middleware
        app.UseSession();

        // Map endpoint groups
        AuthEndpoints.MapAuthEndpoints(app);
        MessageEndpoints.MapMessageEndpoints(app);

        return app;
    }
}