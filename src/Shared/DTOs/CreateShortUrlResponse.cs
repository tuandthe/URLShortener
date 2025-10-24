namespace URLShortener.Shared.DTOs;

/// <summary>
/// DTO cho response trả về URL rút gọn
/// </summary>
public class CreateShortUrlResponse
{
    public required string ShortUrl { get; set; }

    public required string ShortCode { get; set; }

    public required string OriginalUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
