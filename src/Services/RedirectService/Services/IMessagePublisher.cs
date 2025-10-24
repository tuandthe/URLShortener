using URLShortener.Shared.Messages;

namespace URLShortener.RedirectService.Services;

/// <summary>
/// Interface cho Message Publisher
/// </summary>
public interface IMessagePublisher : IDisposable
{
    Task PublishClickEventAsync(ClickEventMessage message, CancellationToken cancellationToken = default);
}
