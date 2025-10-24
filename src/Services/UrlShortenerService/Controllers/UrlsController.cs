using Microsoft.AspNetCore.Mvc;
using URLShortener.Shared.DTOs;
using URLShortener.UrlShortenerService.Services;

namespace URLShortener.UrlShortenerService.Controllers;

/// <summary>
/// Controller xử lý các request tạo URL rút gọn
/// </summary>
[ApiController]
[Route("api/urls")]
public class UrlsController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly ILogger<UrlsController> _logger;
    private readonly IConfiguration _configuration;

    public UrlsController(
        IUrlShortenerService urlShortenerService,
        ILogger<UrlsController> logger,
        IConfiguration configuration)
    {
        _urlShortenerService = urlShortenerService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Tạo URL rút gọn mới
    /// </summary>
    /// <param name="request">Request chứa URL gốc</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response chứa URL rút gọn</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateShortUrlResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateShortUrlResponse>> CreateShortUrl(
        [FromBody] CreateShortUrlRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received request to create short URL for: {OriginalUrl}", request.OriginalUrl);

            // Validate URL
            if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out var uri))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "URL không hợp lệ",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            // Tạo short URL
            var urlMapping = await _urlShortenerService.CreateShortUrlAsync(request.OriginalUrl, cancellationToken);

            // Lấy base URL từ configuration
            var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5000";
            var shortUrl = $"{baseUrl}/{urlMapping.ShortCode}";

            var response = new CreateShortUrlResponse
            {
                ShortUrl = shortUrl,
                ShortCode = urlMapping.ShortCode,
                OriginalUrl = urlMapping.OriginalUrl,
                CreatedAt = urlMapping.CreatedAt
            };

            _logger.LogInformation("Successfully created short URL: {ShortUrl}", shortUrl);

            return CreatedAtAction(nameof(CreateShortUrl), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating short URL");

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Message = "Đã xảy ra lỗi khi tạo URL rút gọn",
                StatusCode = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Service = "UrlShortenerService" });
    }
}
