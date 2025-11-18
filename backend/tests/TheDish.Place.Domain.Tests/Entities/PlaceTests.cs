using FluentAssertions;
using TheDish.Place.Domain.Entities;
using TheDish.Place.Domain.Enums;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using Xunit;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Domain.Tests.Entities;

public class PlaceTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePlace()
    {
        // Arrange & Act
        var place = new PlaceEntity(
            "Test Restaurant",
            "123 Main St",
            40.7128,
            -74.0060,
            2,
            new List<string> { "Italian", "Pizza" });

        // Assert
        place.Should().NotBeNull();
        place.Name.Should().Be("Test Restaurant");
        place.Address.Should().Be("123 Main St");
        place.Location.Y.Should().Be(40.7128);
        place.Location.X.Should().Be(-74.0060);
        place.PriceRange.Should().Be(2);
        place.CuisineTypes.Should().Contain("Italian");
        place.CuisineTypes.Should().Contain("Pizza");
        place.Status.Should().Be(PlaceStatus.Active);
        place.AverageRating.Should().Be(0);
        place.ReviewCount.Should().Be(0);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdatePlace()
    {
        // Arrange
        var place = new PlaceEntity("Test Restaurant", "123 Main St", 40.7128, -74.0060, 2);

        // Act
        place.UpdateDetails(
            name: "Updated Restaurant",
            phone: "555-1234",
            website: "https://example.com",
            priceRange: 3,
            cuisineTypes: new List<string> { "Mexican" },
            amenities: new List<string> { "Parking", "WiFi" });

        // Assert
        place.Name.Should().Be("Updated Restaurant");
        place.Phone.Should().Be("555-1234");
        place.Website.Should().Be("https://example.com");
        place.PriceRange.Should().Be(3);
        place.CuisineTypes.Should().Contain("Mexican");
    }

    [Fact]
    public void UpdateLocation_WithValidCoordinates_ShouldUpdateLocation()
    {
        // Arrange
        var place = new PlaceEntity("Test Restaurant", "123 Main St", 40.7128, -74.0060, 2);
        var newLat = 40.7580;
        var newLon = -73.9855;

        // Act
        place.UpdateLocation(newLat, newLon);

        // Assert
        place.Location.Y.Should().Be(newLat);
        place.Location.X.Should().Be(newLon);
    }

    [Fact]
    public void Claim_WithUserId_ShouldSetClaimedBy()
    {
        // Arrange
        var place = new PlaceEntity("Test Restaurant", "123 Main St", 40.7128, -74.0060, 2);
        var userId = Guid.NewGuid();

        // Act
        place.Claim(userId);

        // Assert
        place.ClaimedBy.Should().Be(userId);
    }

    [Fact]
    public void Verify_ShouldSetIsVerifiedToTrue()
    {
        // Arrange
        var place = new PlaceEntity("Test Restaurant", "123 Main St", 40.7128, -74.0060, 2);

        // Act
        place.Verify();

        // Assert
        place.IsVerified.Should().BeTrue();
    }

    [Fact]
    public void UpdateRating_WithValidRating_ShouldUpdateRating()
    {
        // Arrange
        var place = new PlaceEntity("Test Restaurant", "123 Main St", 40.7128, -74.0060, 2);

        // Act
        place.UpdateRating(4.5m, 10);

        // Assert
        place.AverageRating.Should().Be(4.5m);
        place.ReviewCount.Should().Be(10);
    }

    [Fact]
    public void UpdateDietaryTags_ShouldUpdateTags()
    {
        // Arrange
        var place = new PlaceEntity("Test Restaurant", "123 Main St", 40.7128, -74.0060, 2);
        var tags = new Dictionary<string, bool>
        {
            { "halal", true },
            { "vegan", false }
        };

        // Act
        place.UpdateDietaryTags(tags);

        // Assert
        place.DietaryTags.Should().ContainKey("halal");
        place.DietaryTags["halal"].Should().BeTrue();
        place.DietaryTags["vegan"].Should().BeFalse();
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatus()
    {
        // Arrange
        var place = new PlaceEntity("Test Restaurant", "123 Main St", 40.7128, -74.0060, 2);

        // Act
        place.ChangeStatus(PlaceStatus.Inactive);

        // Assert
        place.Status.Should().Be(PlaceStatus.Inactive);
    }
}

