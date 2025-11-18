using TheDish.Review.Domain.Entities;
using ReviewEntity = TheDish.Review.Domain.Entities.Review;
using ReviewHelpfulnessEntity = TheDish.Review.Domain.Entities.ReviewHelpfulness;

namespace TheDish.Review.Application.Interfaces;

public interface IReviewRepository
{
    Task<ReviewEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReviewEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReviewEntity>> GetReviewsByPlaceIdAsync(
        Guid placeId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ReviewEntity>> GetReviewsByUserIdAsync(
        Guid userId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ReviewEntity>> GetRecentReviewsAsync(
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
    Task<ReviewEntity?> GetReviewByUserAndPlaceAsync(
        Guid userId,
        Guid placeId,
        CancellationToken cancellationToken = default);
    Task<ReviewEntity> AddAsync(ReviewEntity review, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReviewEntity review, CancellationToken cancellationToken = default);
    Task DeleteAsync(ReviewEntity review, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetReviewCountByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default);
    Task<decimal> GetAverageRatingByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default);
    Task<ReviewHelpfulnessEntity?> GetHelpfulnessVoteAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
    Task AddHelpfulnessVoteAsync(ReviewHelpfulnessEntity vote, CancellationToken cancellationToken = default);
    Task UpdateHelpfulnessVoteAsync(ReviewHelpfulnessEntity vote, CancellationToken cancellationToken = default);
}

