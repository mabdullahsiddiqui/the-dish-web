using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Application.Queries;

public class GetReviewAnalysisQueryHandler : IRequestHandler<GetReviewAnalysisQuery, Response<ReviewAnalysisDto>>
{
    private readonly IReviewAnalysisService _reviewAnalysisService;
    private readonly ILogger<GetReviewAnalysisQueryHandler> _logger;

    public GetReviewAnalysisQueryHandler(
        IReviewAnalysisService reviewAnalysisService,
        ILogger<GetReviewAnalysisQueryHandler> logger)
    {
        _reviewAnalysisService = reviewAnalysisService;
        _logger = logger;
    }

    public async Task<Response<ReviewAnalysisDto>> Handle(GetReviewAnalysisQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var analysis = await _reviewAnalysisService.GetReviewAnalysisAsync(request.ReviewId, cancellationToken);

            if (analysis == null)
            {
                return Response<ReviewAnalysisDto>.FailureResult("Review analysis not found. The review may not have been analyzed yet.");
            }

            return Response<ReviewAnalysisDto>.SuccessResult(analysis, "Review analysis retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review analysis for ReviewId: {ReviewId}", request.ReviewId);
            return Response<ReviewAnalysisDto>.FailureResult("An error occurred while retrieving review analysis");
        }
    }
}








