using TheDish.Place.Domain.Entities;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Interfaces;

public interface IPlaceRepository
{
    Task<PlaceEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PlaceEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlaceEntity>> GetNearbyPlacesAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<PlaceEntity>> SearchPlacesAsync(
        string? searchTerm = null,
        List<string>? cuisineTypes = null,
        List<string>? dietaryTags = null,
        int? minPriceRange = null,
        int? maxPriceRange = null,
        decimal? minRating = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusKm = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
    Task<PlaceEntity> AddAsync(PlaceEntity place, CancellationToken cancellationToken = default);
    Task UpdateAsync(PlaceEntity place, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

