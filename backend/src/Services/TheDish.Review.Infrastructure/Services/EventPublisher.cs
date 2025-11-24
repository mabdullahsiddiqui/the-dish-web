using MassTransit;
using Microsoft.Extensions.Logging;
using TheDish.Review.Application.Events;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Infrastructure.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IPublishEndpoint publishEndpoint, ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishReviewCreatedAsync(ReviewCreatedEvent reviewEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            await _publishEndpoint.Publish(reviewEvent, cancellationToken);
            _logger.LogInformation("Published ReviewCreatedEvent for ReviewId: {ReviewId}", reviewEvent.ReviewId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ReviewCreatedEvent for ReviewId: {ReviewId}", reviewEvent.ReviewId);
            throw;
        }
    }
}






