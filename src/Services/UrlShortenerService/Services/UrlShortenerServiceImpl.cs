using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using URLShortener.Shared.Models;
using URLShortener.UrlShortenerService.Data;

namespace URLShortener.UrlShortenerService.Services;

/// <summary>
/// Implementation của service xử lý logic nghiệp vụ rút gọn URL
/// </summary>
public class UrlShortenerServiceImpl : IUrlShortenerService
{
    private readonly UrlShortenerDbContext _dbContext;
    private readonly ILogger<UrlShortenerServiceImpl> _logger;
    private const int ShortCodeLength = 7;
    private const int MaxRetries = 5;

    public UrlShortenerServiceImpl(
        UrlShortenerDbContext dbContext,
        ILogger<UrlShortenerServiceImpl> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UrlMapping> CreateShortUrlAsync(string originalUrl, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating short URL for: {OriginalUrl}", originalUrl);

        // Kiểm tra xem URL đã tồn tại chưa
        var existingMapping = await _dbContext.UrlMappings
            .FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl, cancellationToken);

        if (existingMapping != null)
        {
            _logger.LogInformation("URL already exists with short code: {ShortCode}", existingMapping.ShortCode);
            return existingMapping;
        }

        // Tạo short code mới
        var shortCode = await GenerateUniqueShortCodeAsync(cancellationToken);

        var urlMapping = new UrlMapping
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.UrlMappings.Add(urlMapping);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created short URL with code: {ShortCode}", shortCode);

        return urlMapping;
    }

    public async Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UrlMappings
            .AnyAsync(u => u.ShortCode == shortCode, cancellationToken);
    }

    /// <summary>
    /// Tạo short code ngẫu nhiên và đảm bảo tính duy nhất
    /// </summary>
    private async Task<string> GenerateUniqueShortCodeAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < MaxRetries; i++)
        {
            var shortCode = GenerateRandomShortCode();

            var exists = await ShortCodeExistsAsync(shortCode, cancellationToken);

            if (!exists)
            {
                return shortCode;
            }

            _logger.LogWarning("Short code collision detected: {ShortCode}. Retrying...", shortCode);
        }

        // Nếu vẫn trùng sau MaxRetries lần thử, sử dụng GUID
        var guidShortCode = GenerateGuidBasedShortCode();
        _logger.LogWarning("Using GUID-based short code after {MaxRetries} retries: {ShortCode}", MaxRetries, guidShortCode);

        return guidShortCode;
    }

    /// <summary>
    /// Tạo short code ngẫu nhiên sử dụng ký tự alphanumeric
    /// </summary>
    private static string GenerateRandomShortCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = new char[ShortCodeLength];

        for (int i = 0; i < ShortCodeLength; i++)
        {
            result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        }

        return new string(result);
    }

    /// <summary>
    /// Tạo short code dựa trên GUID (fallback khi có collision)
    /// </summary>
    private static string GenerateGuidBasedShortCode()
    {
        var guid = Guid.NewGuid().ToString("N");
        return guid.Substring(0, ShortCodeLength);
    }
}
