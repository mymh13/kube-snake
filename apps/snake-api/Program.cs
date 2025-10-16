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

// Background timer to auto-move snake
var timer = new System.Timers.Timer(300);
var gameState = new GameState(timer);

timer.Elapsed += (sender, e) => gameState.Move(gameState.CurrentDirection);
timer.Start();

app.MapGet("/render", () =>
{
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