using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Replace environment variable placeholders in ocelot.json
var ocelotConfig = File.ReadAllText("ocelot.json");
ocelotConfig = ocelotConfig
    .Replace("#{URL_SHORTENER_HOST}#", Environment.GetEnvironmentVariable("URL_SHORTENER_HOST") ?? "urlshortener-service")
    .Replace("#{REDIRECT_SERVICE_HOST}#", Environment.GetEnvironmentVariable("REDIRECT_SERVICE_HOST") ?? "redirect-service")
    .Replace("#{GATEWAY_BASE_URL}#", Environment.GetEnvironmentVariable("GATEWAY_BASE_URL") ?? "http://gateway:8080");

var tempOcelotPath = Path.Combine(Path.GetTempPath(), "ocelot.runtime.json");
File.WriteAllText(tempOcelotPath, ocelotConfig);

// Add Ocelot configuration with replaced values
builder.Configuration.AddJsonFile(tempOcelotPath, optional: false, reloadOnChange: false);

// Add services to the container
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

// Add Ocelot
builder.Services.AddOcelot();

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

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
