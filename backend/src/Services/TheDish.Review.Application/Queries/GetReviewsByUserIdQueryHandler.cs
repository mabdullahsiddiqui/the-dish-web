using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Application.Queries;

public class GetReviewsByUserIdQueryHandler : IRequestHandler<GetReviewsByUserIdQuery, Response<ReviewListResponseDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<GetReviewsByUserIdQueryHandler> _logger;

    public GetReviewsByUserIdQueryHandler(
        IReviewRepository reviewRepository,
        ILogger<GetReviewsByUserIdQueryHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<Response<ReviewListResponseDto>> Handle(GetReviewsByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var skip = (request.Page - 1) * request.PageSize;

            var reviews = await _reviewRepository.GetReviewsByUserIdAsync(
                request.UserId,
                skip,
                request.PageSize,
                cancellationToken);

            var reviewsList = reviews.ToList();
            // Note: We'd need a count method for user reviews if needed
            var totalCount = reviewsList.Count == request.PageSize ? (request.Page * request.PageSize) + 1 : (request.Page - 1) * request.PageSize + reviewsList.Count;

            var reviewDtos = reviewsList.Select(MapToDto).ToList();

            var response = new ReviewListResponseDto
            {
                Reviews = reviewDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Response<ReviewListResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reviews for user: {UserId}", request.UserId);
            return Response<ReviewListResponseDto>.FailureResult("An error occurred while retrieving reviews");
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








