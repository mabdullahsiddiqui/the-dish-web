using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.Commands;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Queries;

namespace TheDish.Review.API.Controllers;

[ApiController]
[Route("api/v1/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IMediator mediator, ILogger<ReviewsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<ReviewDto>>> GetReview(Guid id)
    {
        var query = new GetReviewByIdQuery { ReviewId = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("place/{placeId}")]
    public async Task<ActionResult<Response<ReviewListResponseDto>>> GetReviewsByPlace(
        Guid placeId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetReviewsByPlaceIdQuery
        {
            PlaceId = placeId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<Response<ReviewListResponseDto>>> GetReviewsByUser(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetReviewsByUserIdQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("recent")]
    public async Task<ActionResult<Response<ReviewListResponseDto>>> GetRecentReviews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetRecentReviewsQuery
        {
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Response<ReviewDto>>> CreateReview([FromBody] CreateReviewDto dto)
    {
        // Log authentication details for debugging
        _logger.LogInformation("CreateReview called. User authenticated: {IsAuthenticated}, Identity name: {IdentityName}", 
            User.Identity?.IsAuthenticated, User.Identity?.Name);
        
        // Extract user ID from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User ID claim value: {UserIdClaim}", userIdClaim ?? "null");
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Failed to extract user ID from token. Claim value: {ClaimValue}", userIdClaim);
            return Unauthorized(Response<ReviewDto>.FailureResult("Invalid user authentication"));
        }
        
        _logger.LogInformation("Creating review for user: {UserId}, place: {PlaceId}", userId, dto.PlaceId);

        var command = new CreateReviewCommand
        {
            UserId = userId,
            PlaceId = dto.PlaceId,
            Rating = dto.Rating,
            Text = dto.Text,
            DietaryAccuracy = dto.DietaryAccuracy,
            CheckInLatitude = dto.CheckInLatitude,
            CheckInLongitude = dto.CheckInLongitude,
            PlaceLatitude = dto.PlaceLatitude,
            PlaceLongitude = dto.PlaceLongitude
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetReview), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<Response<ReviewDto>>> UpdateReview(
        Guid id,
        [FromBody] UpdateReviewDto dto)
    {
        // Extract user ID from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(Response<ReviewDto>.FailureResult("Invalid user authentication"));
        }

        var command = new UpdateReviewCommand
        {
            ReviewId = id,
            UserId = userId,
            Rating = dto.Rating,
            Text = dto.Text,
            DietaryAccuracy = dto.DietaryAccuracy
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<Response<bool>>> DeleteReview(Guid id)
    {
        // Extract user ID from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(Response<bool>.FailureResult("Invalid user authentication"));
        }

        var command = new DeleteReviewCommand
        {
            ReviewId = id,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/helpful")]
    [Authorize]
    public async Task<ActionResult<Response<ReviewDto>>> MarkReviewHelpful(
        Guid id,
        [FromBody] bool isHelpful)
    {
        // Extract user ID from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(Response<ReviewDto>.FailureResult("Invalid user authentication"));
        }

        var command = new MarkReviewHelpfulCommand
        {
            ReviewId = id,
            UserId = userId,
            IsHelpful = isHelpful
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}/analyze")]
    public async Task<ActionResult<Response<ReviewAnalysisDto>>> GetReviewAnalysis(Guid id)
    {
        var query = new GetReviewAnalysisQuery { ReviewId = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}










