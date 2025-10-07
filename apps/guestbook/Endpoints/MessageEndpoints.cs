using Guestbook.Helpers;
using Guestbook.Models;
using Guestbook.Services;
using Microsoft.AspNetCore.Mvc;

namespace Guestbook.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        // GET /api/messages - Get last 10 messages (public, returns HTML)
        app.MapGet("/api/messages", async (MongoDbService mongoDb) =>
        {
            var messages = await mongoDb.GetRecentMessagesAsync();
            var html = HtmlHelper.RenderMessageList(messages);
            return Results.Content(html, "text/html");
        });

        // POST /api/messages - Create message (admin only, returns updated message list)
        app.MapPost("/api/messages", async (HttpContext context, MongoDbService mongoDb, [FromForm] string text) =>
        {
            if (!AuthHelper.IsAdmin(context))
            {
                return Results.Unauthorized();
            }

            var message = new Message
            {
                Text = text,
                CreatedBy = "admin"
            };

            await mongoDb.CreateMessageAsync(message);

            var messages = await mongoDb.GetRecentMessagesAsync();
            var html = HtmlHelper.RenderMessageList(messages);
            return Results.Content(html, "text/html");
        })
        .DisableAntiforgery(); // Disable CSRF for API endpoint

        // DELETE /api/messages/{id} - Delete message (admin only, returns updated list)
        app.MapDelete("/api/messages/{id}", async (HttpContext context, MongoDbService mongoDb, string id) =>
        {
            if (!AuthHelper.IsAdmin(context))
            {
                return Results.Unauthorized();
            }

            await mongoDb.DeleteMessageAsync(id);

            var messages = await mongoDb.GetRecentMessagesAsync();
            var html = HtmlHelper.RenderMessageList(messages);
            return Results.Content(html, "text/html");
        });
    }
}