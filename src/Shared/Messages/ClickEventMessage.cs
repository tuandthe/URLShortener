namespace URLShortener.Shared.Messages;

/// <summary>
/// Message được gửi qua RabbitMQ khi có click event
/// </summary>
public class ClickEventMessage
{
    public required string ShortCode { get; set; }

    public DateTime Timestamp { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }
}
