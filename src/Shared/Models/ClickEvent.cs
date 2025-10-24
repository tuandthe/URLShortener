namespace URLShortener.Shared.Models;

/// <summary>
/// Entity đại diện cho sự kiện click vào link rút gọn
/// </summary>
public class ClickEvent
{
    public int Id { get; set; }

    public required string ShortCode { get; set; }

    public DateTime Timestamp { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }
}
