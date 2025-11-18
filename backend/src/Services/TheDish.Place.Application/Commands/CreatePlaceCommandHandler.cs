using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Interfaces;
using TheDish.Place.Domain.Entities;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Commands;

public class CreatePlaceCommandHandler : IRequestHandler<CreatePlaceCommand, Response<PlaceDto>>
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePlaceCommandHandler> _logger;

    public CreatePlaceCommandHandler(
        IPlaceRepository placeRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePlaceCommandHandler> logger)
    {
        _placeRepository = placeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<PlaceDto>> Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var place = new PlaceEntity(
                request.Name,
                request.Address,
                request.Latitude,
                request.Longitude,
                request.PriceRange,
                request.CuisineTypes);

            if (!string.IsNullOrWhiteSpace(request.Phone))
                place.UpdateDetails(phone: request.Phone);
            if (!string.IsNullOrWhiteSpace(request.Website))
                place.UpdateDetails(website: request.Website);
            if (!string.IsNullOrWhiteSpace(request.Email))
                place.UpdateDetails(email: request.Email);
            if (request.HoursOfOperation != null || request.Amenities != null || !string.IsNullOrWhiteSpace(request.ParkingInfo))
            {
                place.UpdateDetails(
                    hoursOfOperation: request.HoursOfOperation,
                    amenities: request.Amenities,
                    parkingInfo: request.ParkingInfo);
            }

            await _placeRepository.AddAsync(place, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var placeDto = MapToDto(place);

            _logger.LogInformation("Place created successfully: {PlaceId} - {Name}", place.Id, place.Name);

            return Response<PlaceDto>.SuccessResult(placeDto, "Place created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating place: {Name}", request.Name);
            return Response<PlaceDto>.FailureResult("An error occurred while creating the place");
        }
    }

    private static PlaceDto MapToDto(PlaceEntity place)
    {
        return new PlaceDto
        {
            Id = place.Id,
            Name = place.Name,
            Address = place.Address,
            Latitude = place.Location.Y,
            Longitude = place.Location.X,
            Phone = place.Phone,
            Website = place.Website,
            Email = place.Email,
            CuisineTypes = place.CuisineTypes,
            PriceRange = place.PriceRange,
            DietaryTags = place.DietaryTags,
            TrustScores = place.TrustScores,
            AverageRating = place.AverageRating,
            ReviewCount = place.ReviewCount,
            ClaimedBy = place.ClaimedBy,
            IsVerified = place.IsVerified,
            Status = place.Status.ToString(),
            Photos = place.Photos.Select(p => new PlacePhotoDto
            {
                Id = p.Id,
                PlaceId = p.PlaceId,
                Url = p.Url,
                ThumbnailUrl = p.ThumbnailUrl,
                Caption = p.Caption,
                UploadedBy = p.UploadedBy,
                IsFeatured = p.IsFeatured,
                DisplayOrder = p.DisplayOrder,
                UploadedAt = p.UploadedAt
            }).ToList(),
            MenuItems = place.MenuItems.Select(m => new MenuItemDto
            {
                Id = m.Id,
                PlaceId = m.PlaceId,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                Category = m.Category,
                DietaryTags = m.DietaryTags,
                AllergenWarnings = m.AllergenWarnings,
                SpiceLevel = m.SpiceLevel,
                IsPopular = m.IsPopular,
                IsAvailable = m.IsAvailable,
                PhotoUrl = m.PhotoUrl
            }).ToList(),
            CreatedAt = place.CreatedAt,
            UpdatedAt = place.UpdatedAt
        };
    }
}

