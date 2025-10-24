using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortener.RedirectService.Data;
using URLShortener.RedirectService.Services;
using URLShortener.Shared.DTOs;
using URLShortener.Shared.Messages;

namespace URLShortener.RedirectService.Controllers;

/// <summary>
/// Controller xử lý redirect từ short code đến URL gốc
/// </summary>
[ApiController]
[Route("")]
public class RedirectController : ControllerBase
{
    private readonly RedirectDbContext _dbContext;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<RedirectController> _logger;

    public RedirectController(
        RedirectDbContext dbContext,
        IMessagePublisher messagePublisher,
        ILogger<RedirectController> logger)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    /// <summary>
    /// Redirect từ short code đến URL gốc
    /// </summary>
    /// <param name="shortCode">Mã rút gọn</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Redirect response hoặc 404</returns>
    [HttpGet("{shortCode}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Redirect(string shortCode, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Redirecting short code: {ShortCode}", shortCode);

            // Tìm URL mapping
            var urlMapping = await _dbContext.UrlMappings
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode, cancellationToken);

            if (urlMapping == null)
            {
                _logger.LogWarning("Short code not found: {ShortCode}", shortCode);

                return NotFound(new ErrorResponse
                {
                    Message = "URL không tồn tại",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            // Gửi message bất đồng bộ (fire-and-forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    var clickEvent = new ClickEventMessage
                    {
                        ShortCode = shortCode,
                        Timestamp = DateTime.UtcNow,
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                    };

                    await _messagePublisher.PublishClickEventAsync(clickEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish click event for short code: {ShortCode}", shortCode);
                }
            }, cancellationToken);

            _logger.LogInformation("Redirecting to: {OriginalUrl}", urlMapping.OriginalUrl);

            // Redirect ngay lập tức
            return Redirect(urlMapping.OriginalUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redirecting short code: {ShortCode}", shortCode);

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Message = "Đã xảy ra lỗi khi xử lý yêu cầu",
                StatusCode = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("api/health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Service = "RedirectService" });
    }
}
