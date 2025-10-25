using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using URLShortener.AnalyticsService.Data;
using URLShortener.Shared.Messages;
using URLShortener.Shared.Models;

namespace URLShortener.AnalyticsService;

/// <summary>
/// Worker service lắng nghe message từ RabbitMQ và lưu click events vào database
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string QueueName = "click_events";

    public Worker(
        ILogger<Worker> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Analytics Worker Service starting...");
        try
        {
            // Run database migration
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();

            _logger.LogInformation("Starting database migration...");
            await dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migration completed successfully");

            await InitializeRabbitMqAsync();
            await base.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Analytics Worker Service");
            throw;
        }
    }

    private async Task InitializeRabbitMqAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest",
            VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/"
        };

        var retryCount = 0;
        const int maxRetries = 3;

        while (retryCount < maxRetries)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _logger.LogInformation("RabbitMQ connection established successfully");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogError(ex, "Failed to connect to RabbitMQ. Retry {RetryCount}/{MaxRetries}", retryCount, maxRetries);

                if (retryCount >= maxRetries)
                {
                    throw;
                }

                await Task.Delay(5000); // Đợi 5 giây trước khi retry
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Analytics Worker Service is running");

        if (_channel == null)
        {
            _logger.LogError("RabbitMQ channel is not initialized");
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<ClickEventMessage>(json);

                if (message != null)
                {
                    await ProcessClickEventAsync(message, stoppingToken);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Started consuming messages from queue: {QueueName}", QueueName);

        // Giữ worker service chạy
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessClickEventAsync(ClickEventMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing click event for short code: {ShortCode}", message.ShortCode);

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();

        var clickEvent = new ClickEvent
        {
            ShortCode = message.ShortCode,
            Timestamp = message.Timestamp,
            UserAgent = message.UserAgent,
            IpAddress = message.IpAddress
        };

        dbContext.ClickEvents.Add(clickEvent);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Click event saved successfully for short code: {ShortCode}", message.ShortCode);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Analytics Worker Service is stopping");

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            _channel.Dispose();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            _connection.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }
}
