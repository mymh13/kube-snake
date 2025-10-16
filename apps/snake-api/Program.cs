var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UsePathBase("/snake-api");

// In-memory game state (later: Redis for multi-replica)
var gameState = new GameState();

app.MapGet("/render", () =>
{
    return Results.Content(gameState.RenderHTML(), "text/html");
});

app.MapPost("/move", (MoveRequest request) =>
{
    gameState.Move(request.Direction);
    return Results.Ok();
});

app.Run();

record MoveRequest(string Direction);