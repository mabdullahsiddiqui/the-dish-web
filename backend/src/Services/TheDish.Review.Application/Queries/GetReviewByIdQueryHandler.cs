using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Application.Queries;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Response<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<GetReviewByIdQueryHandler> _logger;

    public GetReviewByIdQueryHandler(
        IReviewRepository reviewRepository,
        ILogger<GetReviewByIdQueryHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<Response<ReviewDto>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewRepository.GetByIdWithDetailsAsync(request.ReviewId, cancellationToken);
            if (review == null)
            {
                return Response<ReviewDto>.FailureResult("Review not found");
            }

            var reviewDto = MapToDto(review);

            return Response<ReviewDto>.SuccessResult(reviewDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting review: {ReviewId}", request.ReviewId);
            return Response<ReviewDto>.FailureResult("An error occurred while retrieving the review");
        }
    }

    private static ReviewDto MapToDto(Domain.Entities.Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            PlaceId = review.PlaceId,
            Rating = review.Rating,
            Text = review.Text,
            PhotoUrls = review.PhotoUrls,
            DietaryAccuracy = review.DietaryAccuracy,
            GpsVerified = review.GpsVerified,
            CheckInLatitude = review.CheckInLocation?.Y,
            CheckInLongitude = review.CheckInLocation?.X,
            HelpfulCount = review.HelpfulCount,
            NotHelpfulCount = review.NotHelpfulCount,
            Status = review.Status.ToString(),
            Photos = review.Photos.Select(p => new ReviewPhotoDto
            {
                Id = p.Id,
                ReviewId = p.ReviewId,
                Url = p.Url,
                ThumbnailUrl = p.ThumbnailUrl,
                Caption = p.Caption,
                UploadedBy = p.UploadedBy,
                UploadedAt = p.UploadedAt
            }).ToList(),
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}











