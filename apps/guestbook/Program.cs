using Guestbook.Extensions;

// Load .env file (for local development)
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register services using extension method
builder.Services.AddGuestbookServices();

// Configure Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

// Handle /guestbook path prefix (for Ingress routing)
app.UsePathBase("/guestbook");

// Session middleware (should be before endpoints)
app.UseSession();

// Map endpoints using extension method
app.MapGuestbookEndpoints();

app.Run();