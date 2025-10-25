using Microsoft.EntityFrameworkCore;
using URLShortener.AnalyticsService;
using URLShortener.AnalyticsService.Data;

var builder = Host.CreateApplicationBuilder(args);

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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
