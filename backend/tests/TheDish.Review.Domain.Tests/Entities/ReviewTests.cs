using FluentAssertions;
using TheDish.Review.Domain.Entities;
using TheDish.Review.Domain.Enums;
using Xunit;
using ReviewEntity = TheDish.Review.Domain.Entities.Review;

namespace TheDish.Review.Domain.Tests.Entities;

public class ReviewTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateReview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var placeId = Guid.NewGuid();

        // Act
        var review = new ReviewEntity(userId, placeId, 5, "Great food!");

        // Assert
        review.Should().NotBeNull();
        review.UserId.Should().Be(userId);
        review.PlaceId.Should().Be(placeId);
        review.Rating.Should().Be(5);
        review.Text.Should().Be("Great food!");
        review.GpsVerified.Should().BeFalse();
        review.Status.Should().Be(ReviewStatus.Active);
        review.HelpfulCount.Should().Be(0);
        review.NotHelpfulCount.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Constructor_WithInvalidRating_ShouldThrowException(int rating)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var placeId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ReviewEntity(userId, placeId, rating, "Test review"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidText_ShouldThrowException(string? text)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var placeId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ReviewEntity(userId, placeId, 5, text!));
    }

    [Fact]
    public void SetGpsVerification_WithValidCoordinates_ShouldSetVerified()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");
        var latitude = 40.7128;
        var longitude = -74.0060;

        // Act
        review.SetGpsVerification(true, latitude, longitude);

        // Assert
        review.GpsVerified.Should().BeTrue();
        review.CheckInLocation.Should().NotBeNull();
        review.CheckInLocation!.Y.Should().Be(latitude);
        review.CheckInLocation.X.Should().Be(longitude);
    }

    [Fact]
    public void SetGpsVerification_WithFalse_ShouldNotSetLocation()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");

        // Act
        review.SetGpsVerification(false);

        // Assert
        review.GpsVerified.Should().BeFalse();
        review.CheckInLocation.Should().BeNull();
    }

    [Fact]
    public void UpdateText_WithValidText_ShouldUpdateText()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Original text");

        // Act
        review.UpdateText("Updated text");

        // Assert
        review.Text.Should().Be("Updated text");
    }

    [Fact]
    public void UpdateRating_WithValidRating_ShouldUpdateRating()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");

        // Act
        review.UpdateRating(4);

        // Assert
        review.Rating.Should().Be(4);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void UpdateRating_WithInvalidRating_ShouldThrowException(int rating)
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.UpdateRating(rating));
    }

    [Fact]
    public void AddPhotoUrl_ShouldAddPhotoUrl()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");
        var photoUrl = "https://example.com/photo.jpg";

        // Act
        review.AddPhotoUrl(photoUrl);

        // Assert
        review.PhotoUrls.Should().Contain(photoUrl);
    }

    [Fact]
    public void AddPhotoUrl_WithDuplicate_ShouldNotAddTwice()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");
        var photoUrl = "https://example.com/photo.jpg";

        // Act
        review.AddPhotoUrl(photoUrl);
        review.AddPhotoUrl(photoUrl);

        // Assert
        review.PhotoUrls.Should().HaveCount(1);
        review.PhotoUrls.Should().Contain(photoUrl);
    }

    [Fact]
    public void IncrementHelpfulCount_ShouldIncrementCount()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");

        // Act
        review.IncrementHelpfulCount();
        review.IncrementHelpfulCount();

        // Assert
        review.HelpfulCount.Should().Be(2);
    }

    [Fact]
    public void IncrementNotHelpfulCount_ShouldIncrementCount()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");

        // Act
        review.IncrementNotHelpfulCount();
        review.IncrementNotHelpfulCount();

        // Assert
        review.NotHelpfulCount.Should().Be(2);
    }

    [Fact]
    public void DecrementHelpfulCount_ShouldDecrementCount()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");
        review.IncrementHelpfulCount();
        review.IncrementHelpfulCount();

        // Act
        review.DecrementHelpfulCount();

        // Assert
        review.HelpfulCount.Should().Be(1);
    }

    [Fact]
    public void DecrementHelpfulCount_WhenZero_ShouldNotGoBelowZero()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");

        // Act
        review.DecrementHelpfulCount();

        // Assert
        review.HelpfulCount.Should().Be(0);
    }

    [Fact]
    public void SetDietaryAccuracy_ShouldUpdateDietaryAccuracy()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");
        var dietaryAccuracy = new Dictionary<string, string>
        {
            { "halal", "accurate" },
            { "vegan", "inaccurate" }
        };

        // Act
        review.SetDietaryAccuracy(dietaryAccuracy);

        // Assert
        review.DietaryAccuracy.Should().ContainKey("halal");
        review.DietaryAccuracy["halal"].Should().Be("accurate");
        review.DietaryAccuracy["vegan"].Should().Be("inaccurate");
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatus()
    {
        // Arrange
        var review = new ReviewEntity(Guid.NewGuid(), Guid.NewGuid(), 5, "Test review");

        // Act
        review.ChangeStatus(ReviewStatus.Flagged);

        // Assert
        review.Status.Should().Be(ReviewStatus.Flagged);
    }
}

