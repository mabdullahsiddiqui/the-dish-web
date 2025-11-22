using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Application.Commands;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Response<bool>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteReviewCommandHandler> _logger;

    public DeleteReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteReviewCommandHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<bool>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
            {
                return Response<bool>.FailureResult("Review not found");
            }

            // Check authorization
            if (review.UserId != request.UserId)
            {
                return Response<bool>.FailureResult("You are not authorized to delete this review");
            }

            await _reviewRepository.DeleteAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Review deleted successfully: {ReviewId}", request.ReviewId);

            return Response<bool>.SuccessResult(true, "Review deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review: {ReviewId}", request.ReviewId);
            return Response<bool>.FailureResult("An error occurred while deleting the review");
        }
    }
}











