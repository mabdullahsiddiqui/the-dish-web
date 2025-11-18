using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Interfaces;
using TheDish.Place.Domain.Entities;

namespace TheDish.Place.Application.Commands;

public class UploadPlacePhotoCommandHandler : IRequestHandler<UploadPlacePhotoCommand, Response<PlacePhotoDto>>
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IPhotoService _photoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadPlacePhotoCommandHandler> _logger;

    public UploadPlacePhotoCommandHandler(
        IPlaceRepository placeRepository,
        IPhotoService photoService,
        IUnitOfWork unitOfWork,
        ILogger<UploadPlacePhotoCommandHandler> logger)
    {
        _placeRepository = placeRepository;
        _photoService = photoService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<PlacePhotoDto>> Handle(UploadPlacePhotoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var place = await _placeRepository.GetByIdAsync(request.PlaceId, cancellationToken);
            if (place == null)
            {
                return Response<PlacePhotoDto>.FailureResult("Place not found");
            }

            // Check authorization - only owner can upload photos
            if (place.ClaimedBy != request.UserId)
            {
                return Response<PlacePhotoDto>.FailureResult("You are not authorized to upload photos for this place");
            }

            // Upload photo to S3
            var photoUrl = await _photoService.UploadPhotoAsync(
                request.PhotoStream,
                request.FileName,
                request.ContentType,
                cancellationToken);

            // Create photo entity
            var photo = new PlacePhoto(request.PlaceId, photoUrl, request.UserId, request.Caption);
            if (request.IsFeatured)
            {
                photo.SetFeatured(true);
            }

            // Set display order (calculate next available)
            var existingPhotos = place.Photos.ToList();
            var maxOrder = existingPhotos.Any() ? existingPhotos.Max(p => p.DisplayOrder) : -1;
            photo.SetDisplayOrder(maxOrder + 1);

            // Add photo to place
            place.Photos.Add(photo);
            await _placeRepository.UpdateAsync(place, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var photoDto = new PlacePhotoDto
            {
                Id = photo.Id,
                PlaceId = photo.PlaceId,
                Url = photo.Url,
                ThumbnailUrl = photo.ThumbnailUrl,
                Caption = photo.Caption,
                UploadedBy = photo.UploadedBy,
                IsFeatured = photo.IsFeatured,
                DisplayOrder = photo.DisplayOrder,
                UploadedAt = photo.UploadedAt
            };

            _logger.LogInformation("Photo uploaded successfully: {PhotoId} for place {PlaceId}", photo.Id, request.PlaceId);

            return Response<PlacePhotoDto>.SuccessResult(photoDto, "Photo uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for place: {PlaceId}", request.PlaceId);
            return Response<PlacePhotoDto>.FailureResult("An error occurred while uploading the photo");
        }
    }
}

