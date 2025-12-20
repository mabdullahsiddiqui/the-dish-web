using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Events;
using TheDish.Review.Application.Interfaces;
using TheDish.Review.Domain.Entities;
using ReviewEntity = TheDish.Review.Domain.Entities.Review;

namespace TheDish.Review.Application.Commands;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Response<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IGpsVerificationService _gpsVerificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CreateReviewCommandHandler> _logger;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IGpsVerificationService gpsVerificationService,
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        ILogger<CreateReviewCommandHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _gpsVerificationService = gpsVerificationService;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Response<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already has a review for this place
            var existingReview = await _reviewRepository.GetReviewByUserAndPlaceAsync(
                request.UserId,
                request.PlaceId,
                cancellationToken);

            if (existingReview != null)
            {
                return Response<ReviewDto>.FailureResult("You have already reviewed this place");
            }

            // Create review
            var review = new ReviewEntity(request.UserId, request.PlaceId, request.Rating, request.Text);

            // Set dietary accuracy if provided
            if (request.DietaryAccuracy != null)
            {
                review.SetDietaryAccuracy(request.DietaryAccuracy);
            }

            // Add photos if provided
            if (request.PhotoUrls != null && request.PhotoUrls.Any())
            {
                foreach (var url in request.PhotoUrls)
                {
                    review.AddPhoto(url, request.UserId);
                }
            }

            // GPS verification
            if (request.CheckInLatitude.HasValue && 
                request.CheckInLongitude.HasValue &&
                request.PlaceLatitude.HasValue &&
                request.PlaceLongitude.HasValue)
            {
                var isVerified = await _gpsVerificationService.VerifyProximityAsync(
                    request.PlaceLatitude.Value,
                    request.PlaceLongitude.Value,
                    request.CheckInLatitude.Value,
                    request.CheckInLongitude.Value,
                    200, // 200 meters
                    cancellationToken);

                review.SetGpsVerification(
                    isVerified,
                    request.CheckInLatitude.Value,
                    request.CheckInLongitude.Value);
            }

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish ReviewCreatedEvent to RabbitMQ
            var reviewCreatedEvent = new ReviewCreatedEvent
            {
                ReviewId = review.Id,
                PlaceId = review.PlaceId,
                UserId = review.UserId,
                Text = review.Text,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt
            };

            await _eventPublisher.PublishReviewCreatedAsync(reviewCreatedEvent, cancellationToken);

            var reviewDto = await MapToDtoAsync(review, cancellationToken);

            _logger.LogInformation("Review created successfully: {ReviewId} for place {PlaceId}", review.Id, request.PlaceId);

            return Response<ReviewDto>.SuccessResult(reviewDto, "Review created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review for place: {PlaceId}", request.PlaceId);
            return Response<ReviewDto>.FailureResult("An error occurred while creating the review");
        }
    }

    private async Task<ReviewDto> MapToDtoAsync(ReviewEntity review, CancellationToken cancellationToken)
    {
        var reviewWithDetails = await _reviewRepository.GetByIdWithDetailsAsync(review.Id, cancellationToken);
        if (reviewWithDetails == null)
        {
            throw new InvalidOperationException("Review not found after creation");
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

