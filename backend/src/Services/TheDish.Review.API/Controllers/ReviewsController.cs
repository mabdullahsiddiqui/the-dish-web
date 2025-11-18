using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<Response<ReviewDto>>> CreateReview([FromBody] CreateReviewDto dto)
    {
        // TODO: Extract user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

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
    public async Task<ActionResult<Response<ReviewDto>>> UpdateReview(
        Guid id,
        [FromBody] UpdateReviewDto dto)
    {
        // TODO: Extract user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

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
    public async Task<ActionResult<Response<bool>>> DeleteReview(Guid id)
    {
        // TODO: Extract user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

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
    public async Task<ActionResult<Response<ReviewDto>>> MarkReviewHelpful(
        Guid id,
        [FromBody] bool isHelpful)
    {
        // TODO: Extract user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

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
}








