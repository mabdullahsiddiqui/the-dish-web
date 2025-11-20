using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Application.Commands;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Response<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateReviewCommandHandler> _logger;

    public UpdateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateReviewCommandHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<ReviewDto>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
            {
                return Response<ReviewDto>.FailureResult("Review not found");
            }

            // Check authorization
            if (review.UserId != request.UserId)
            {
                return Response<ReviewDto>.FailureResult("You are not authorized to update this review");
            }

            // Update fields
            if (request.Rating.HasValue)
            {
                review.UpdateRating(request.Rating.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Text))
            {
                review.UpdateText(request.Text);
            }

            if (request.DietaryAccuracy != null)
            {
                review.SetDietaryAccuracy(request.DietaryAccuracy);
            }

            await _reviewRepository.UpdateAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var reviewDto = await MapToDtoAsync(review, cancellationToken);

            _logger.LogInformation("Review updated successfully: {ReviewId}", request.ReviewId);

            return Response<ReviewDto>.SuccessResult(reviewDto, "Review updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review: {ReviewId}", request.ReviewId);
            return Response<ReviewDto>.FailureResult("An error occurred while updating the review");
        }
    }

    private async Task<ReviewDto> MapToDtoAsync(Domain.Entities.Review review, CancellationToken cancellationToken)
    {
        var reviewWithDetails = await _reviewRepository.GetByIdWithDetailsAsync(review.Id, cancellationToken);
        if (reviewWithDetails == null)
        {
            throw new InvalidOperationException("Review not found after update");
        }

        return new ReviewDto
        {
            Id = reviewWithDetails.Id,
            UserId = reviewWithDetails.UserId,
            PlaceId = reviewWithDetails.PlaceId,
            Rating = reviewWithDetails.Rating,
            Text = reviewWithDetails.Text,
            PhotoUrls = reviewWithDetails.PhotoUrls,
            DietaryAccuracy = reviewWithDetails.DietaryAccuracy,
            GpsVerified = reviewWithDetails.GpsVerified,
            CheckInLatitude = reviewWithDetails.CheckInLocation?.Y,
            CheckInLongitude = reviewWithDetails.CheckInLocation?.X,
            HelpfulCount = reviewWithDetails.HelpfulCount,
            NotHelpfulCount = reviewWithDetails.NotHelpfulCount,
            Status = reviewWithDetails.Status.ToString(),
            Photos = reviewWithDetails.Photos.Select(p => new ReviewPhotoDto
            {
                Id = p.Id,
                ReviewId = p.ReviewId,
                Url = p.Url,
                ThumbnailUrl = p.ThumbnailUrl,
                Caption = p.Caption,
                UploadedBy = p.UploadedBy,
                UploadedAt = p.UploadedAt
            }).ToList(),
            CreatedAt = reviewWithDetails.CreatedAt,
            UpdatedAt = reviewWithDetails.UpdatedAt
        };
    }
}










