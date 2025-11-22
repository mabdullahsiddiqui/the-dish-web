using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Interfaces;
using TheDish.Review.Domain.Entities;
using ReviewEntity = TheDish.Review.Domain.Entities.Review;

namespace TheDish.Review.Application.Commands;

public class MarkReviewHelpfulCommandHandler : IRequestHandler<MarkReviewHelpfulCommand, Response<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkReviewHelpfulCommandHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public MarkReviewHelpfulCommandHandler(
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork,
        ILogger<MarkReviewHelpfulCommandHandler> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
    }

    public async Task<Response<ReviewDto>> Handle(MarkReviewHelpfulCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
            {
                return Response<ReviewDto>.FailureResult("Review not found");
            }

            // Check if user already voted
            var existingVote = await _reviewRepository.GetHelpfulnessVoteAsync(request.ReviewId, request.UserId, cancellationToken);

            if (existingVote != null)
            {
                // Update existing vote
                var wasHelpful = existingVote.IsHelpful;
                existingVote.UpdateVote(request.IsHelpful);
                await _reviewRepository.UpdateHelpfulnessVoteAsync(existingVote, cancellationToken);

                // Update counts
                if (wasHelpful && !request.IsHelpful)
                {
                    review.DecrementHelpfulCount();
                    review.IncrementNotHelpfulCount();
                }
                else if (!wasHelpful && request.IsHelpful)
                {
                    review.DecrementNotHelpfulCount();
                    review.IncrementHelpfulCount();
                }
            }
            else
            {
                // Create new vote
                var vote = new ReviewHelpfulness(request.ReviewId, request.UserId, request.IsHelpful);
                await _reviewRepository.AddHelpfulnessVoteAsync(vote, cancellationToken);

                // Update counts
                if (request.IsHelpful)
                {
                    review.IncrementHelpfulCount();
                }
                else
                {
                    review.IncrementNotHelpfulCount();
                }
            }

            await _reviewRepository.UpdateAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update review author's reputation based on helpful votes
            // +1 point for helpful, -0.5 points for not helpful
            try
            {
                var reputationChange = request.IsHelpful ? 1.0 : -0.5;
                if (existingVote != null)
                {
                    // If changing vote, adjust reputation accordingly
                    var wasHelpful = existingVote.IsHelpful;
                    if (wasHelpful && !request.IsHelpful)
                    {
                        reputationChange = -1.5; // Remove helpful (+1) and add not helpful (-0.5)
                    }
                    else if (!wasHelpful && request.IsHelpful)
                    {
                        reputationChange = 1.5; // Remove not helpful (-0.5) and add helpful (+1)
                    }
                    else
                    {
                        reputationChange = 0; // No change
                    }
                }

                if (reputationChange != 0)
                {
                    await UpdateUserReputationAsync(review.UserId, reputationChange, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail the review vote if reputation update fails
                _logger.LogWarning(ex, "Failed to update user reputation for review author: {UserId}", review.UserId);
            }

            var reviewDto = await MapToDtoAsync(review, cancellationToken);

            _logger.LogInformation("Review helpfulness updated: {ReviewId} by {UserId}", request.ReviewId, request.UserId);

            return Response<ReviewDto>.SuccessResult(reviewDto, "Vote recorded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking review helpful: {ReviewId}", request.ReviewId);
            return Response<ReviewDto>.FailureResult("An error occurred while recording the vote");
        }
    }

    private async Task<ReviewDto> MapToDtoAsync(ReviewEntity review, CancellationToken cancellationToken)
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

    private async Task UpdateUserReputationAsync(Guid userId, double points, CancellationToken cancellationToken)
    {
        try
        {
            var userServiceUrl = _configuration["Services:UserService:BaseUrl"] ?? "http://localhost:5001";
            var updateUrl = $"{userServiceUrl}/api/v1/users/{userId}/reputation";

            var requestBody = new { points = (int)Math.Round(points) };
            var response = await _httpClient.PostAsJsonAsync(updateUrl, requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to update user reputation: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling User Service to update reputation for user: {UserId}", userId);
            throw;
        }
    }
}

