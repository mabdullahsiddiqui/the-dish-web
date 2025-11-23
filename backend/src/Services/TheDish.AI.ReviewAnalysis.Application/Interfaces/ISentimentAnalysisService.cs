using TheDish.AI.ReviewAnalysis.Application.DTOs;

namespace TheDish.AI.ReviewAnalysis.Application.Interfaces;

public interface ISentimentAnalysisService
{
    Task<ReviewAnalysisResult> AnalyzeReviewAsync(string reviewText, CancellationToken cancellationToken = default);
}




