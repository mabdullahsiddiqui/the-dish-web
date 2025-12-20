using TheDish.Review.Application.Events;

namespace TheDish.Review.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishReviewCreatedAsync(ReviewCreatedEvent reviewEvent, CancellationToken cancellationToken = default);
}








