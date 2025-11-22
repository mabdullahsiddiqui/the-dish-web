using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.Commands;
using TheDish.Review.Application.Interfaces;
using Xunit;

namespace TheDish.Review.Application.Tests.Commands;

public class CreateReviewCommandHandlerTests
{
    private readonly Mock<IReviewRepository> _repositoryMock;
    private readonly Mock<IGpsVerificationService> _gpsServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateReviewCommandHandler>> _loggerMock;
    private readonly CreateReviewCommandHandler _handler;

    public CreateReviewCommandHandlerTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
        _gpsServiceMock = new Mock<IGpsVerificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateReviewCommandHandler>>();
        _handler = new CreateReviewCommandHandler(
            _repositoryMock.Object,
            _gpsServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var placeId = Guid.NewGuid();
        var command = new CreateReviewCommand
        {
            UserId = userId,
            PlaceId = placeId,
            Rating = 5,
            Text = "Great food!",
            CheckInLatitude = 40.7128,
            CheckInLongitude = -74.0060,
            PlaceLatitude = 40.7128,
            PlaceLongitude = -74.0060
        };

        _repositoryMock
            .Setup(r => r.GetReviewByUserAndPlaceAsync(userId, placeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Review?)null);

        _gpsServiceMock
            .Setup(g => g.VerifyProximityAsync(
                40.7128, -74.0060,
                40.7128, -74.0060,
                200,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Review>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Review review, CancellationToken ct) => review);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken ct) =>
            {
                var review = new Domain.Entities.Review(userId, placeId, 5, "Great food!");
                review.SetGpsVerification(true, 40.7128, -74.0060);
                return review;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Rating.Should().Be(5);
        result.Data.Text.Should().Be("Great food!");
        result.Data.GpsVerified.Should().BeTrue();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Review>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _gpsServiceMock.Verify(g => g.VerifyProximityAsync(
            40.7128, -74.0060,
            40.7128, -74.0060,
            200,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingReview_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var placeId = Guid.NewGuid();
        var command = new CreateReviewCommand
        {
            UserId = userId,
            PlaceId = placeId,
            Rating = 5,
            Text = "Great food!"
        };

        var existingReview = new Domain.Entities.Review(userId, placeId, 4, "Previous review");

        _repositoryMock
            .Setup(r => r.GetReviewByUserAndPlaceAsync(userId, placeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already reviewed");
    }

    [Fact]
    public async Task Handle_WithGpsBeyond200Meters_ShouldNotVerify()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var placeId = Guid.NewGuid();
        var command = new CreateReviewCommand
        {
            UserId = userId,
            PlaceId = placeId,
            Rating = 5,
            Text = "Great food!",
            CheckInLatitude = 40.7200, // Far away
            CheckInLongitude = -74.0060,
            PlaceLatitude = 40.7128,
            PlaceLongitude = -74.0060
        };

        _repositoryMock
            .Setup(r => r.GetReviewByUserAndPlaceAsync(userId, placeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Review?)null);

        _gpsServiceMock
            .Setup(g => g.VerifyProximityAsync(
                40.7128, -74.0060,
                40.7200, -74.0060,
                200,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Review>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Review review, CancellationToken ct) => review);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken ct) =>
            {
                var review = new Domain.Entities.Review(userId, placeId, 5, "Great food!");
                review.SetGpsVerification(false, 40.7200, -74.0060);
                return review;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data!.GpsVerified.Should().BeFalse();
    }
}











