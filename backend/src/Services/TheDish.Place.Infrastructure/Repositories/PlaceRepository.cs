using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TheDish.Place.Application.Interfaces;
using TheDish.Place.Domain.Entities;
using TheDish.Place.Infrastructure.Data;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Infrastructure.Repositories;

public class PlaceRepository : IPlaceRepository
{
    private readonly PlaceDbContext _context;

    public PlaceRepository(PlaceDbContext context)
    {
        _context = context;
    }

    public async Task<PlaceEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Places
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<PlaceEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Places
            .Include(p => p.Photos.OrderBy(pp => pp.DisplayOrder).ThenBy(pp => pp.UploadedAt))
            .Include(p => p.MenuItems)
            .Include(p => p.Certifications)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<PlaceEntity>> GetNearbyPlacesAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken cancellationToken = default)
    {
        var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var userLocation = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        
        // Convert radius from km to meters for PostGIS
        var radiusMeters = radiusKm * 1000;

        return await _context.Places
            .Where(p => !p.IsDeleted && p.Status == Domain.Enums.PlaceStatus.Active)
            .Where(p => p.Location.Distance(userLocation) <= radiusMeters)
            .OrderBy(p => p.Location.Distance(userLocation))
            .Include(p => p.Photos.Where(pp => pp.IsFeatured).Take(1))
            .Take(50) // Limit results
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PlaceEntity>> SearchPlacesAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _context.Places
            .Where(p => !p.IsDeleted && p.Status == Domain.Enums.PlaceStatus.Active)
            .AsQueryable();

        // Text search
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Search in Name and Address using EF.Functions for case-insensitive search
            query = query.Where(p => 
                EF.Functions.ILike(p.Name, $"%{searchTerm}%") || 
                EF.Functions.ILike(p.Address, $"%{searchTerm}%"));
        }

        // Cuisine filter
        if (cuisineTypes != null && cuisineTypes.Any())
        {
            query = query.Where(p => p.CuisineTypes.Any(ct => cuisineTypes.Contains(ct)));
        }

        // Dietary tags filter
        if (dietaryTags != null && dietaryTags.Any())
        {
            foreach (var tag in dietaryTags)
            {
                query = query.Where(p => p.DietaryTags.ContainsKey(tag) && p.DietaryTags[tag]);
            }
        }

        // Price range filter
        if (minPriceRange.HasValue)
        {
            query = query.Where(p => p.PriceRange >= minPriceRange.Value);
        }
        if (maxPriceRange.HasValue)
        {
            query = query.Where(p => p.PriceRange <= maxPriceRange.Value);
        }

        // Rating filter
        if (minRating.HasValue)
        {
            query = query.Where(p => p.AverageRating >= minRating.Value);
        }

        // Geospatial filter
        if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
        {
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var userLocation = geometryFactory.CreatePoint(new Coordinate(longitude.Value, latitude.Value));
            var radiusMeters = radiusKm.Value * 1000;

            query = query
                .Where(p => p.Location.Distance(userLocation) <= radiusMeters)
                .OrderBy(p => p.Location.Distance(userLocation));
        }
        else
        {
            // Default ordering by rating and review count
            query = query.OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.ReviewCount);
        }

        var results = await query
            .Include(p => p.Photos.Where(pp => pp.IsFeatured).Take(1))
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        // If search term provided, also filter by cuisine types (after materialization)
        // This allows us to search in the List<string> CuisineTypes which is converted from DB
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            var filteredResults = results.Where(p => 
                p.CuisineTypes.Any(ct => ct.ToLower().Contains(searchTermLower))
            ).ToList();
            
            // Combine with results that matched Name/Address (already in results)
            // Remove duplicates and return
            var allMatches = results.Union(filteredResults).ToList();
            return allMatches;
        }

        return results;
    }

    public async Task<PlaceEntity> AddAsync(PlaceEntity place, CancellationToken cancellationToken = default)
    {
        await _context.Places.AddAsync(place, cancellationToken);
        return place;
    }

    public async Task UpdateAsync(PlaceEntity place, CancellationToken cancellationToken = default)
    {
        _context.Places.Update(place);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Places
            .AnyAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }
}

