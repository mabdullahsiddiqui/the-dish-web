using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Interfaces;
using System.Text.Json.Serialization;

namespace TheDish.Review.Application.Queries;

public class GetReviewsByPlaceIdQueryHandler : IRequestHandler<GetReviewsByPlaceIdQuery, Response<ReviewListResponseDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<GetReviewsByPlaceIdQueryHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GetReviewsByPlaceIdQueryHandler(
        IReviewRepository reviewRepository,
        ILogger<GetReviewsByPlaceIdQueryHandler> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
    }

    public async Task<Response<ReviewListResponseDto>> Handle(GetReviewsByPlaceIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var skip = (request.Page - 1) * request.PageSize;

            var reviews = await _reviewRepository.GetReviewsByPlaceIdAsync(
                request.PlaceId,
                skip,
                request.PageSize,
                cancellationToken);

            var reviewsList = reviews.ToList();
            var totalCount = await _reviewRepository.GetReviewCountByPlaceIdAsync(request.PlaceId, cancellationToken);

            // Get unique user IDs
            var userIds = reviewsList.Select(r => r.UserId).Distinct().ToList();
            
            // Fetch user information from User Service
            var userInfoDict = await FetchUserInfoAsync(userIds, cancellationToken);

            var reviewDtos = reviewsList.Select(r => MapToDto(r, userInfoDict.GetValueOrDefault(r.UserId))).ToList();

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
            _logger.LogError(ex, "Error getting reviews for place: {PlaceId}", request.PlaceId);
            return Response<ReviewListResponseDto>.FailureResult("An error occurred while retrieving reviews");
        }
    }

    private async Task<Dictionary<Guid, UserInfoDto>> FetchUserInfoAsync(List<Guid> userIds, CancellationToken cancellationToken)
    {
        var userInfoDict = new Dictionary<Guid, UserInfoDto>();
        
        if (userIds.Count == 0)
        {
            return userInfoDict;
        }

        var userServiceUrl = _configuration["Services:UserService:BaseUrl"] ?? "http://localhost:5001";
        
        // Configure JSON options to handle case-insensitive property names
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _logger.LogInformation("Fetching user info for {Count} users from {UserServiceUrl}", userIds.Count, userServiceUrl);

        // Fetch user info for each user (in parallel for better performance)
        var tasks = userIds.Select(async userId =>
        {
            try
            {
                var response = await _httpClient.GetAsync($"{userServiceUrl}/api/v1/users/{userId}", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    // Read as JsonDocument to handle flexible property types
                    var jsonDoc = await System.Text.Json.JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
                    var result = jsonDoc.RootElement;
                    
                    if (result.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
                    {
                        if (result.TryGetProperty("data", out var dataProp))
                        {
                            var firstName = dataProp.TryGetProperty("firstName", out var fn) ? fn.GetString() 
                                          : dataProp.TryGetProperty("FirstName", out var fn2) ? fn2.GetString() : string.Empty;
                            var lastName = dataProp.TryGetProperty("lastName", out var ln) ? ln.GetString() 
                                         : dataProp.TryGetProperty("LastName", out var ln2) ? ln2.GetString() : string.Empty;
                            var reviewCount = dataProp.TryGetProperty("reviewCount", out var rc) ? rc.GetInt32() 
                                            : dataProp.TryGetProperty("ReviewCount", out var rc2) ? rc2.GetInt32() : 0;
                            var isVerified = dataProp.TryGetProperty("isVerified", out var iv) ? iv.GetBoolean() 
                                           : dataProp.TryGetProperty("IsVerified", out var iv2) ? iv2.GetBoolean() : false;
                            var reputation = dataProp.TryGetProperty("reputation", out var rep) ? rep.GetInt32() 
                                           : dataProp.TryGetProperty("Reputation", out var rep2) ? rep2.GetInt32() : 0;
                            
                            // Handle ReputationLevel as both string and integer
                            string reputationLevel = "Bronze";
                            if (dataProp.TryGetProperty("reputationLevel", out var rl) || dataProp.TryGetProperty("ReputationLevel", out rl))
                            {
                                if (rl.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    reputationLevel = rl.GetString() ?? "Bronze";
                                }
                                else if (rl.ValueKind == System.Text.Json.JsonValueKind.Number)
                                {
                                    var intValue = rl.GetInt32();
                                    reputationLevel = intValue switch
                                    {
                                        0 => "Bronze",
                                        1 => "Silver",
                                        2 => "Gold",
                                        3 => "Platinum",
                                        4 => "Diamond",
                                        _ => "Bronze"
                                    };
                                }
                            }
                            
                            _logger.LogDebug("Successfully fetched user info for {UserId}: {FirstName} {LastName}", 
                                userId, firstName, lastName);
                            return new KeyValuePair<Guid, UserInfoDto>(userId, new UserInfoDto
                            {
                                FirstName = firstName ?? string.Empty,
                                LastName = lastName ?? string.Empty,
                                ReviewCount = reviewCount,
                                IsVerified = isVerified,
                                Reputation = reputation,
                                ReputationLevel = reputationLevel
                            });
                        }
                    }
                    else
                    {
                        var message = result.TryGetProperty("message", out var msg) ? msg.GetString() 
                                    : result.TryGetProperty("Message", out var msg2) ? msg2.GetString() 
                                    : "Unknown error";
                        _logger.LogWarning("User service returned unsuccessful response for user {UserId}: {Message}", 
                            userId, message);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Failed to fetch user {UserId}: HTTP {StatusCode}. Response: {ErrorContent}", 
                        userId, response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while fetching user info for user {UserId}", userId);
            }
            return new KeyValuePair<Guid, UserInfoDto>(userId, null!);
        });

        var results = await Task.WhenAll(tasks);
        var successCount = 0;
        foreach (var kvp in results)
        {
            if (kvp.Value != null)
            {
                userInfoDict[kvp.Key] = kvp.Value;
                successCount++;
            }
        }

        _logger.LogInformation("Successfully fetched user info for {SuccessCount} out of {TotalCount} users", 
            successCount, userIds.Count);

        return userInfoDict;
    }


    private static ReviewDto MapToDto(Domain.Entities.Review review, UserInfoDto? userInfo = null)
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
            UpdatedAt = review.UpdatedAt,
            User = userInfo
        };
    }
}










