using TheDish.Review.Application.DTOs;

namespace TheDish.Review.Application.Interfaces;

public interface IReviewAnalysisService
{
    Task<ReviewAnalysisDto?> GetReviewAnalysisAsync(Guid reviewId, CancellationToken cancellationToken = default);
}








