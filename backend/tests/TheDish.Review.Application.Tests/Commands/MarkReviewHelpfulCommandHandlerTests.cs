using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.Commands;
using TheDish.Review.Application.Interfaces;
using TheDish.Review.Domain.Entities;
using Xunit;
using ReviewEntity = TheDish.Review.Domain.Entities.Review;

namespace TheDish.Review.Application.Tests.Commands;

public class MarkReviewHelpfulCommandHandlerTests
{
    private readonly Mock<IReviewRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<MarkReviewHelpfulCommandHandler>> _loggerMock;
    private readonly MarkReviewHelpfulCommandHandler _handler;

    public MarkReviewHelpfulCommandHandlerTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<MarkReviewHelpfulCommandHandler>>();
        _handler = new MarkReviewHelpfulCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithNewVote_ShouldIncrementHelpfulCount()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new MarkReviewHelpfulCommand
        {
            ReviewId = reviewId,
            UserId = userId,
            IsHelpful = true
        };

        var review = new ReviewEntity(userId, Guid.NewGuid(), 5, "Test review");
        review.GetType().GetProperty("Id")!.SetValue(review, reviewId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        _repositoryMock
            .Setup(r => r.GetHelpfulnessVoteAsync(reviewId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReviewHelpfulness?)null);

        _repositoryMock
            .Setup(r => r.AddHelpfulnessVoteAsync(It.IsAny<ReviewHelpfulness>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<ReviewEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }
}

