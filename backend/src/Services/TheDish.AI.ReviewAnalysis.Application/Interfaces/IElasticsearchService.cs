using TheDish.AI.ReviewAnalysis.Application.DTOs;

namespace TheDish.AI.ReviewAnalysis.Application.Interfaces;

public interface IElasticsearchService
{
    Task IndexReviewAnalysisAsync(Guid placeId, Guid reviewId, ReviewAnalysisResult analysis, CancellationToken cancellationToken = default);
    Task<ReviewAnalysisResult?> GetReviewAnalysisAsync(Guid reviewId, CancellationToken cancellationToken = default);
}








