using SnakeApi;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // GDPR: Mark as essential
    options.Cookie.Name = ".SnakeGame.Session";
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://kube-snake.mymh.dev")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Important for cookies!
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSession(); // Enable sessions
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
    // Get or create session ID
    var sessionId = context.Session.GetString("SessionId");
    if (string.IsNullOrEmpty(sessionId))
    {
        sessionId = Guid.NewGuid().ToString();
        context.Session.SetString("SessionId", sessionId);
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

        return state;
    });
}

// Endpoints
app.MapGet("/render", (HttpContext context) =>
{
    var gameState = GetGameState(context);
    return Results.Content(gameState.RenderHTML(), "text/html");
});

app.MapPost("/start", (HttpContext context) =>
{
    var gameState = GetGameState(context);
    gameState.Start();
    return Results.Ok();
});

app.MapPost("/pause", (HttpContext context) =>
{
    var gameState = GetGameState(context);
    gameState.Pause();
    return Results.Ok();
});

app.MapPost("/reset", (HttpContext context) =>
{
    var gameState = GetGameState(context);
    gameState.Reset();
    return Results.Ok();
});

app.MapPost("/move", (HttpContext context, MoveRequest request) =>
{
    var gameState = GetGameState(context);
    gameState.Move(request.Direction);
    return Results.Ok();
});

app.Run();

public record MoveRequest(string Direction);