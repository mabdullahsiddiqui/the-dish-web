using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Interfaces;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Queries;

public class GetNearbyPlacesQueryHandler : IRequestHandler<GetNearbyPlacesQuery, Response<List<PlaceDto>>>
{
    private readonly IPlaceRepository _placeRepository;
    private readonly ILogger<GetNearbyPlacesQueryHandler> _logger;

    public GetNearbyPlacesQueryHandler(
        IPlaceRepository placeRepository,
        ILogger<GetNearbyPlacesQueryHandler> logger)
    {
        _placeRepository = placeRepository;
        _logger = logger;
    }

    public async Task<Response<List<PlaceDto>>> Handle(GetNearbyPlacesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var places = await _placeRepository.GetNearbyPlacesAsync(
                request.Latitude,
                request.Longitude,
                request.RadiusKm,
                cancellationToken);

            // Apply additional filters
            var filteredPlaces = places.AsQueryable();

            if (request.CuisineFilters != null && request.CuisineFilters.Any())
            {
                filteredPlaces = filteredPlaces.Where(p => p.CuisineTypes.Any(ct => request.CuisineFilters.Contains(ct)));
            }

            if (request.DietaryFilters != null && request.DietaryFilters.Any())
            {
                foreach (var tag in request.DietaryFilters)
                {
                    filteredPlaces = filteredPlaces.Where(p => p.DietaryTags.ContainsKey(tag) && p.DietaryTags[tag]);
                }
            }

            if (request.PriceRange.HasValue)
            {
                filteredPlaces = filteredPlaces.Where(p => p.PriceRange == request.PriceRange.Value);
            }

            var placesList = filteredPlaces.ToList();

            // Calculate distances and map to DTOs
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var userLocation = geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

            var placeDtos = placesList.Select(place =>
            {
                var dto = MapToDto(place);
                var distance = place.Location.Distance(userLocation) / 1000; // Convert to km
                dto.DistanceKm = distance;
                return dto;
            }).OrderBy(p => p.DistanceKm).ToList();

            return Response<List<PlaceDto>>.SuccessResult(placeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nearby places");
            return Response<List<PlaceDto>>.FailureResult("An error occurred while retrieving nearby places");
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

