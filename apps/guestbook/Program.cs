using Guestbook.Extensions;

// Load .env file (for local development)
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add CORS support
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
              .AllowCredentials();
    });
});

// Register services using extension method
builder.Services.AddGuestbookServices();

// Configure Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

// Enable CORS (must be before other middleware)
app.UseCors("AllowAll");

// Handle /guestbook path prefix (for Ingress routing)
app.UsePathBase("/guestbook");

// Session middleware (should be before endpoints)
app.UseSession();

// Map endpoints using extension method
app.MapGuestbookEndpoints();

app.Run();