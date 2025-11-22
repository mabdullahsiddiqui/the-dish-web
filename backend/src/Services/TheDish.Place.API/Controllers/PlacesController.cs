using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.Commands;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Queries;

namespace TheDish.Place.API.Controllers;

[ApiController]
[Route("api/v1/places")]
public class PlacesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlacesController> _logger;

    public PlacesController(IMediator mediator, ILogger<PlacesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<PlaceDto>>> GetPlace(Guid id)
    {
        var query = new GetPlaceByIdQuery { PlaceId = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("nearby")]
    public async Task<ActionResult<Response<List<PlaceDto>>>> GetNearbyPlaces(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10.0,
        [FromQuery] List<string>? dietaryFilters = null,
        [FromQuery] List<string>? cuisineFilters = null,
        [FromQuery] int? priceRange = null)
    {
        var query = new GetNearbyPlacesQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            DietaryFilters = dietaryFilters,
            CuisineFilters = cuisineFilters,
            PriceRange = priceRange
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<Response<SearchPlacesResponseDto>>> SearchPlaces(
        [FromQuery] string? searchTerm = null,
        [FromQuery] List<string>? cuisineTypes = null,
        [FromQuery] List<string>? dietaryTags = null,
        [FromQuery] int? minPriceRange = null,
        [FromQuery] int? maxPriceRange = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] double? latitude = null,
        [FromQuery] double? longitude = null,
        [FromQuery] double? radiusKm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new SearchPlacesQuery
        {
            SearchTerm = searchTerm,
            CuisineTypes = cuisineTypes,
            DietaryTags = dietaryTags,
            MinPriceRange = minPriceRange > 0 ? minPriceRange : null,
            MaxPriceRange = maxPriceRange > 0 ? maxPriceRange : null,
            MinRating = minRating,
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
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
    public async Task<ActionResult<Response<PlaceDto>>> CreatePlace([FromBody] CreatePlaceDto dto)
    {
        var command = new CreatePlaceCommand
        {
            Name = dto.Name,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Phone = dto.Phone,
            Website = dto.Website,
            Email = dto.Email,
            PriceRange = dto.PriceRange,
            CuisineTypes = dto.CuisineTypes,
            HoursOfOperation = dto.HoursOfOperation,
            Amenities = dto.Amenities,
            ParkingInfo = dto.ParkingInfo
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetPlace), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Response<PlaceDto>>> UpdatePlace(
        Guid id,
        [FromBody] UpdatePlaceDto dto)
    {
        // TODO: Extract user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

        var command = new UpdatePlaceCommand
        {
            PlaceId = id,
            UserId = userId,
            Name = dto.Name,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Phone = dto.Phone,
            Website = dto.Website,
            Email = dto.Email,
            PriceRange = dto.PriceRange,
            CuisineTypes = dto.CuisineTypes,
            HoursOfOperation = dto.HoursOfOperation,
            Amenities = dto.Amenities,
            ParkingInfo = dto.ParkingInfo
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/claim")]
    public async Task<ActionResult<Response<PlaceDto>>> ClaimPlace(Guid id)
    {
        // TODO: Extract user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

        var command = new ClaimPlaceCommand
        {
            PlaceId = id,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}/rating")]
    public async Task<ActionResult<Response<PlaceDto>>> UpdatePlaceRating(
        Guid id,
        [FromBody] UpdatePlaceRatingDto dto)
    {
        var command = new UpdatePlaceRatingCommand
        {
            PlaceId = id,
            AverageRating = dto.AverageRating,
            ReviewCount = dto.ReviewCount
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}










