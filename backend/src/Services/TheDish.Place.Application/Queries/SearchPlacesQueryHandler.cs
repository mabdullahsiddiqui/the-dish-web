using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Application.Interfaces;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Queries;

public class SearchPlacesQueryHandler : IRequestHandler<SearchPlacesQuery, Response<SearchPlacesResponseDto>>
{
    private readonly IPlaceRepository _placeRepository;
    private readonly ILogger<SearchPlacesQueryHandler> _logger;

    public SearchPlacesQueryHandler(
        IPlaceRepository placeRepository,
        ILogger<SearchPlacesQueryHandler> logger)
    {
        _placeRepository = placeRepository;
        _logger = logger;
    }

    public async Task<Response<SearchPlacesResponseDto>> Handle(SearchPlacesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var skip = (request.Page - 1) * request.PageSize;

            var places = await _placeRepository.SearchPlacesAsync(
                request.SearchTerm,
                request.CuisineTypes,
                request.DietaryTags,
                request.MinPriceRange,
                request.MaxPriceRange,
                request.MinRating,
                request.Latitude,
                request.Longitude,
                request.RadiusKm,
                skip,
                request.PageSize,
                cancellationToken);

            var placesList = places.ToList();

            // Calculate distances if location provided
            Point? userLocation = null;
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                userLocation = geometryFactory.CreatePoint(new Coordinate(request.Longitude.Value, request.Latitude.Value));
            }

            var placeDtos = placesList.Select(place =>
            {
                var dto = MapToDto(place);
                if (userLocation != null)
                {
                    var distance = place.Location.Distance(userLocation) / 1000; // Convert to km
                    dto.DistanceKm = distance;
                }
                return dto;
            }).ToList();

            // Note: For accurate total count, we'd need a separate count query
            // For now, we'll estimate based on returned results
            var totalCount = placesList.Count == request.PageSize ? (request.Page * request.PageSize) + 1 : (request.Page - 1) * request.PageSize + placesList.Count;

            var response = new SearchPlacesResponseDto
            {
                Places = placeDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Response<SearchPlacesResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching places");
            return Response<SearchPlacesResponseDto>.FailureResult("An error occurred while searching places");
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

