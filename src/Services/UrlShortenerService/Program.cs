using Microsoft.EntityFrameworkCore;
using URLShortener.UrlShortenerService.Data;
using URLShortener.UrlShortenerService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register services
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerServiceImpl>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();

    try
    {
        logger.LogInformation("Starting database migration...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database");
        // Không throw exception để service vẫn chạy
    }
}

app.Run();
