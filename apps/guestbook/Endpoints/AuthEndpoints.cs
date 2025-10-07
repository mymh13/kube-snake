using Guestbook.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Guestbook.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // POST /api/login - Admin login (returns HTML swap)
        app.MapPost("/api/login", (HttpContext context, [FromForm] string username, [FromForm] string password) =>
        {
            var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "admin";
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "changeme";

            if (username == adminUsername && password == adminPassword)
            {
                context.Session.SetString("IsAdmin", "true");
                return Results.Content(HtmlHelper.RenderAdminPanel(), "text/html");
            }

            return Results.Content("<p style='color: red;'>Invalid credentials</p>", "text/html");
        })
        .DisableAntiforgery(); // Disable CSRF for API endpoint

        // POST /api/logout - Clear admin session
        app.MapPost("/api/logout", (HttpContext context) =>
        {
            context.Session.Clear();
            return Results.Content(HtmlHelper.RenderLoginForm(), "text/html");
        })
        .DisableAntiforgery(); // Disable CSRF for API endpoint
    }
}