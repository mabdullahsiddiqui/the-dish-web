using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Interfaces;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Queries;

public class GetPlaceByIdQueryHandler : IRequestHandler<GetPlaceByIdQuery, Response<PlaceDto>>
{
    private readonly IPlaceRepository _placeRepository;
    private readonly ILogger<GetPlaceByIdQueryHandler> _logger;

    public GetPlaceByIdQueryHandler(
        IPlaceRepository placeRepository,
        ILogger<GetPlaceByIdQueryHandler> logger)
    {
        _placeRepository = placeRepository;
        _logger = logger;
    }

    public async Task<Response<PlaceDto>> Handle(GetPlaceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var place = await _placeRepository.GetByIdWithDetailsAsync(request.PlaceId, cancellationToken);
            if (place == null)
            {
                return Response<PlaceDto>.FailureResult("Place not found");
            }

            var placeDto = MapToDto(place);

            return Response<PlaceDto>.SuccessResult(placeDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting place: {PlaceId}", request.PlaceId);
            return Response<PlaceDto>.FailureResult("An error occurred while retrieving the place");
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

