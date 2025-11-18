using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.Queries;
using TheDish.Place.Application.Interfaces;
using TheDish.Place.Domain.Entities;
using TheDish.Place.Domain.Enums;
using Xunit;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Application.Tests.Queries;

public class GetNearbyPlacesQueryHandlerTests
{
    private readonly Mock<IPlaceRepository> _repositoryMock;
    private readonly Mock<ILogger<GetNearbyPlacesQueryHandler>> _loggerMock;
    private readonly GetNearbyPlacesQueryHandler _handler;

    public GetNearbyPlacesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IPlaceRepository>();
        _loggerMock = new Mock<ILogger<GetNearbyPlacesQueryHandler>>();
        _handler = new GetNearbyPlacesQueryHandler(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnNearbyPlaces()
    {
        // Arrange
        var query = new GetNearbyPlacesQuery
        {
            Latitude = 40.7128,
            Longitude = -74.0060,
            RadiusKm = 5.0
        };

        var places = new List<PlaceEntity>
        {
            new PlaceEntity("Restaurant 1", "123 Main St", 40.7130, -74.0060, 2),
            new PlaceEntity("Restaurant 2", "456 Oak Ave", 40.7140, -74.0060, 3)
        };

        _repositoryMock
            .Setup(r => r.GetNearbyPlacesAsync(
                query.Latitude,
                query.Longitude,
                query.RadiusKm,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(places);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("Restaurant 1");
        result.Data[1].Name.Should().Be("Restaurant 2");
    }

    [Fact]
    public async Task Handle_WithCuisineFilter_ShouldFilterPlaces()
    {
        // Arrange
        var query = new GetNearbyPlacesQuery
        {
            Latitude = 40.7128,
            Longitude = -74.0060,
            RadiusKm = 5.0,
            CuisineFilters = new List<string> { "Italian" }
        };

        var place1 = new PlaceEntity("Italian Restaurant", "123 Main St", 40.7130, -74.0060, 2, new List<string> { "Italian" });
        var place2 = new PlaceEntity("Mexican Restaurant", "456 Oak Ave", 40.7140, -74.0060, 3, new List<string> { "Mexican" });
        var places = new List<PlaceEntity> { place1, place2 };

        _repositoryMock
            .Setup(r => r.GetNearbyPlacesAsync(
                query.Latitude,
                query.Longitude,
                query.RadiusKm,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(places);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(1);
        result.Data[0].Name.Should().Be("Italian Restaurant");
    }
}

