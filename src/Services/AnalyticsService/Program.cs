using Microsoft.EntityFrameworkCore;
using URLShortener.AnalyticsService;
using URLShortener.AnalyticsService.Data;

var builder = Host.CreateApplicationBuilder(args);

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Parse PostgreSQL URI if needed (Render format)
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres"))
{
    try
    {
        var uri = new Uri(connectionString);
        var db = uri.AbsolutePath.Trim('/');
        var userInfo = uri.UserInfo.Split(':');
        var port = uri.Port > 0 ? uri.Port : 5432; // Default to 5432 if not specified
        connectionString = $"Host={uri.Host};Port={port};Database={db};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing connection string: {ex.Message}");
        throw;
    }
}

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Worker Service
builder.Services.AddHostedService<Worker>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var host = builder.Build();

host.Run();
