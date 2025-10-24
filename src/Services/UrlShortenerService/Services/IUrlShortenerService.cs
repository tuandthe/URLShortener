using URLShortener.Shared.Models;

namespace URLShortener.UrlShortenerService.Services;

/// <summary>
/// Interface cho service xử lý logic nghiệp vụ rút gọn URL
/// </summary>
public interface IUrlShortenerService
{
    Task<UrlMapping> CreateShortUrlAsync(string originalUrl, CancellationToken cancellationToken = default);

    Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default);
}
