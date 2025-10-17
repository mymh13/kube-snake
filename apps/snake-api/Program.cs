using SnakeApi;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".SnakeGame.Session";
    options.Cookie.SameSite = SameSiteMode.Lax; // Important for cross-origin
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "https://kube-snake.mymh.dev",
                "http://localhost:8080",
                "http://localhost:3000"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // ✅ Required for session cookies!
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSession();
app.UsePathBase("/snake-api");

// Try to connect to Redis
RedisGameStateStore? redisStore = null;
try
{
    var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";
    redisStore = new RedisGameStateStore(redisConnection);
    Console.WriteLine("✅ Connected to Redis successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Redis not available, running without shared state: {ex.Message}");
}

// Store game states per session
var gameStates = new ConcurrentDictionary<string, GameState>();

// Helper to get or create game state for session
GameState GetGameState(HttpContext context)
{
    // Ensure session is loaded
    var sessionId = context.Session.GetString("SessionId");
    if (string.IsNullOrEmpty(sessionId))
    {
        sessionId = Guid.NewGuid().ToString();
        context.Session.SetString("SessionId", sessionId);
        Console.WriteLine($"🆕 New session created: {sessionId}");
    }

    return gameStates.GetOrAdd(sessionId, _ =>
    {
        var timer = new System.Timers.Timer(300);
        var state = new GameState(timer, redisStore, sessionId);

        timer.Elapsed += (sender, e) =>
        {
            if (state.IsGameStarted && !state.IsGameOver && !state.IsGamePaused)
            {
                state.Move(state.CurrentDirection);
            }
        };
        timer.Start();

        Console.WriteLine($"🎮 Game state initialized for session: {sessionId}");
        return state;
    });
}

// Endpoints
app.MapGet("/render", async (HttpContext context) =>
{
    await context.Session.LoadAsync(); // ✅ Ensure session is loaded
    var gameState = GetGameState(context);
    return Results.Content(gameState.RenderHTML(), "text/html");
});

app.MapPost("/start", async (HttpContext context) =>
{
    await context.Session.LoadAsync();
    var gameState = GetGameState(context);
    gameState.Start();
    Console.WriteLine($"▶️ Game started for session: {context.Session.GetString("SessionId")}");
    return Results.Ok();
});

app.MapPost("/pause", async (HttpContext context) =>
{
    await context.Session.LoadAsync();
    var gameState = GetGameState(context);
    gameState.Pause();
    Console.WriteLine($"⏸️ Game paused for session: {context.Session.GetString("SessionId")}");
    return Results.Ok();
});

app.MapPost("/reset", async (HttpContext context) =>
{
    await context.Session.LoadAsync();
    var gameState = GetGameState(context);
    gameState.Reset();
    Console.WriteLine($"🔄 Game reset for session: {context.Session.GetString("SessionId")}");
    return Results.Ok();
});

app.MapPost("/move", async (HttpContext context, MoveRequest request) =>
{
    await context.Session.LoadAsync();
    var gameState = GetGameState(context);
    gameState.Move(request.Direction);
    return Results.Ok();
});

app.Run();

public record MoveRequest(string Direction);