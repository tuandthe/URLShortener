namespace URLShortener.Shared.Models;

/// <summary>
/// Entity đại diện cho mapping giữa URL gốc và mã rút gọn
/// </summary>
public class UrlMapping
{
    public int Id { get; set; }

    public required string OriginalUrl { get; set; }

    public required string ShortCode { get; set; }

    public DateTime CreatedAt { get; set; }
}
