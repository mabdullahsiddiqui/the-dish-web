using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.Commands;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.API.Controllers;

[ApiController]
[Route("api/v1/places/{placeId}/photos")]
public class PlacePhotosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlacePhotosController> _logger;

    public PlacePhotosController(IMediator mediator, ILogger<PlacePhotosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [RequestSizeLimit(10_000_000)] // 10MB limit
    public async Task<ActionResult<Response<PlacePhotoDto>>> UploadPhoto(
        Guid placeId,
        IFormFile file,
        [FromForm] string? caption = null,
        [FromForm] bool isFeatured = false)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(Response<PlacePhotoDto>.FailureResult("No file uploaded"));
        }

        // TODO: Extract user ID from JWT token
        var userId = Guid.NewGuid(); // Placeholder

        var command = new UploadPlacePhotoCommand
        {
            PlaceId = placeId,
            UserId = userId,
            PhotoStream = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Caption = caption,
            IsFeatured = isFeatured
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}









