using SnakeApi;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

app.UsePathBase("/snake-api");

// Create game state WITH auto-timer (needed for rate-limiting)
var timer = new System.Timers.Timer(200); // 200ms = 5 FPS
var gameState = new GameState(timer);

// Auto-move the snake on timer
timer.Elapsed += (sender, e) =>
{
    if (gameState.IsGameStarted && !gameState.IsGameOver && !gameState.IsGamePaused)
    {
        gameState.Move(gameState.CurrentDirection);
    }
};
timer.Start();

// SSE endpoint (keep for future, but not used now)
app.MapGet("/game-stream", async (HttpContext context) =>
{
    context.Response.Headers.Add("Content-Type", "text/event-stream");
    context.Response.Headers.Add("Cache-Control", "no-cache");
    context.Response.Headers.Add("Connection", "keep-alive");

    while (!context.RequestAborted.IsCancellationRequested)
    {
        var html = gameState.RenderHTML();

        await context.Response.WriteAsync($"event: gameUpdate\n");
        await context.Response.WriteAsync($"data: {html}\n\n");
        await context.Response.Body.FlushAsync();

        await Task.Delay(200);
    }
});

app.MapGet("/render", (HttpContext context) =>
{
    // Disable caching
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    // Just render current state - don't move!
    // The timer handles movement now
    return Results.Content(gameState.RenderHTML(), "text/html");
});

app.MapPost("/move", async (HttpRequest request) =>
{
    var form = await request.ReadFormAsync();
    var direction = form["direction"].ToString();

    gameState.ChangeDirection(direction);
    return Results.Ok();
});

app.MapPost("/start", () =>
{
    gameState.Start();
    return Results.Ok();
});

app.MapPost("/pause", () =>
{
    gameState.TogglePause();
    return Results.Ok();
});

app.MapPost("/reset", () =>
{
    gameState.Reset();
    return Results.Ok();
});

app.MapGet("/status", () =>
{
    return Results.Ok(new { started = gameState.IsGameStarted });
});

app.Run();

record MoveRequest(string Direction);