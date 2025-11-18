using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TheDish.Common.Application.Common;
using TheDish.Review.API.Controllers;
using TheDish.Review.Application.Commands;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Queries;
using Xunit;

namespace TheDish.Review.API.Tests.Controllers;

public class ReviewsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ReviewsController>> _loggerMock;
    private readonly ReviewsController _controller;

    public ReviewsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ReviewsController>>();
        _controller = new ReviewsController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetReview_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var reviewDto = new ReviewDto
        {
            Id = reviewId,
            UserId = Guid.NewGuid(),
            PlaceId = Guid.NewGuid(),
            Rating = 5,
            Text = "Great food!"
        };

        var response = Response<ReviewDto>.SuccessResult(reviewDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetReviewByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetReview(reviewId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CreateReview_WithValidDto_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateReviewDto
        {
            PlaceId = Guid.NewGuid(),
            Rating = 5,
            Text = "Great food!",
            CheckInLatitude = 40.7128,
            CheckInLongitude = -74.0060,
            PlaceLatitude = 40.7128,
            PlaceLongitude = -74.0060
        };

        var reviewDto = new ReviewDto
        {
            Id = Guid.NewGuid(),
            PlaceId = createDto.PlaceId,
            Rating = 5,
            Text = "Great food!",
            GpsVerified = true
        };

        var response = Response<ReviewDto>.SuccessResult(reviewDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateReviewCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateReview(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task MarkReviewHelpful_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var reviewDto = new ReviewDto
        {
            Id = reviewId,
            HelpfulCount = 1
        };

        var response = Response<ReviewDto>.SuccessResult(reviewDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<MarkReviewHelpfulCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.MarkReviewHelpful(reviewId, true);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }
}








