using Guestbook.Extensions;

// Load .env file (for local development)
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register services using extension method
builder.Services.AddGuestbookServices();

var app = builder.Build();

// Map endpoints using extension method
app.MapGuestbookEndpoints();

app.Run();