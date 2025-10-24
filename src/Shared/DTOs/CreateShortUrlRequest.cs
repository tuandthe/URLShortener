using System.ComponentModel.DataAnnotations;

namespace URLShortener.Shared.DTOs;

/// <summary>
/// DTO cho request tạo URL rút gọn
/// </summary>
public class CreateShortUrlRequest
{
    [Required(ErrorMessage = "URL gốc là bắt buộc")]
    [Url(ErrorMessage = "URL không hợp lệ")]
    [MaxLength(2048, ErrorMessage = "URL quá dài")]
    public required string OriginalUrl { get; set; }
}
