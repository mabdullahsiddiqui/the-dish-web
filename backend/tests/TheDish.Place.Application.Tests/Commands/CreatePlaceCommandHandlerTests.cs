using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.Commands;
using TheDish.Place.Application.Interfaces;
using Xunit;

namespace TheDish.Place.Application.Tests.Commands;

public class CreatePlaceCommandHandlerTests
{
    private readonly Mock<IPlaceRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreatePlaceCommandHandler>> _loggerMock;
    private readonly CreatePlaceCommandHandler _handler;

    public CreatePlaceCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPlaceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreatePlaceCommandHandler>>();
        _handler = new CreatePlaceCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreatePlaceCommand
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Latitude = 40.7128,
            Longitude = -74.0060,
            PriceRange = 2,
            CuisineTypes = new List<string> { "Italian" }
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Place>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Place place, CancellationToken ct) => place);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Restaurant");
        result.Data.Address.Should().Be("123 Main St");
        result.Data.Latitude.Should().Be(40.7128);
        result.Data.Longitude.Should().Be(-74.0060);
        result.Data.PriceRange.Should().Be(2);
        result.Data.CuisineTypes.Should().Contain("Italian");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Place>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithException_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreatePlaceCommand
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Latitude = 40.7128,
            Longitude = -74.0060,
            PriceRange = 2
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Place>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }
}

