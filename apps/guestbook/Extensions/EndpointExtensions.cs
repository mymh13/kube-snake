using Guestbook.Endpoints;

namespace Guestbook.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapGuestbookEndpoints(this WebApplication app)
    {
        app.MapMessageEndpoints();
        app.MapAuthEndpoints();

        return app;
    }
}