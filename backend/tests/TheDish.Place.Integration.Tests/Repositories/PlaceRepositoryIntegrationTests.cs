using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TheDish.Place.Infrastructure.Data;
using TheDish.Place.Infrastructure.Repositories;
using TheDish.Place.Domain.Entities;
using Xunit;

namespace TheDish.Place.Integration.Tests.Repositories;

public class PlaceRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly PlaceDbContext _context;
    private readonly PlaceRepository _repository;

    public PlaceRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _context = fixture.Context;
        _repository = new PlaceRepository(_context);
    }

    [Fact]
    public async Task GetNearbyPlaces_WithValidLocation_ShouldReturnPlacesWithinRadius()
    {
        // Arrange
        var centerLat = 40.7128;
        var centerLon = -74.0060;
        var radiusKm = 5.0;

        // Create test places
        var place1 = new Place("Restaurant 1", "123 Main St", centerLat, centerLon, 2);
        var place2 = new Place("Restaurant 2", "456 Oak Ave", centerLat + 0.01, centerLon, 2); // ~1km away
        var place3 = new Place("Restaurant 3", "789 Pine St", centerLat + 0.1, centerLon, 2); // ~10km away

        _context.Places.AddRange(place1, place2, place3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetNearbyPlacesAsync(
            centerLat, 
            centerLon, 
            radiusKm, 
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(2); // At least place1 and place2 should be within 5km
        result.Should().Contain(p => p.Name == "Restaurant 1");
        result.Should().Contain(p => p.Name == "Restaurant 2");
    }

    [Fact]
    public async Task SearchPlaces_WithCuisineFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var place1 = new Place("Italian Restaurant", "123 Main St", 40.7128, -74.0060, 2);
        place1.UpdateDetails(cuisineTypes: new List<string> { "Italian" });
        var place2 = new Place("Mexican Restaurant", "456 Oak Ave", 40.7130, -74.0060, 2);
        place2.UpdateDetails(cuisineTypes: new List<string> { "Mexican" });

        _context.Places.AddRange(place1, place2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchPlacesAsync(
            searchTerm: null,
            cuisineTypes: new List<string> { "Italian" },
            latitude: 40.7128,
            longitude: -74.0060,
            radiusKm: 10.0,
            cancellationToken: CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(p => p.Name == "Italian Restaurant");
        result.Should().NotContain(p => p.Name == "Mexican Restaurant");
    }
}

public class DatabaseFixture : IDisposable
{
    public PlaceDbContext Context { get; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<PlaceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new PlaceDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

