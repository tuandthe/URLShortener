namespace URLShortener.Shared.DTOs;

/// <summary>
/// DTO cho response lỗi chuẩn
/// </summary>
public class ErrorResponse
{
    public required string Message { get; set; }

    public int StatusCode { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Dictionary<string, string[]>? Errors { get; set; }
}
