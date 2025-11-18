using Microsoft.EntityFrameworkCore;
using TheDish.Review.Application.Interfaces;
using TheDish.Review.Domain.Entities;
using TheDish.Review.Infrastructure.Data;
using ReviewEntity = TheDish.Review.Domain.Entities.Review;
using ReviewHelpfulnessEntity = TheDish.Review.Domain.Entities.ReviewHelpfulness;

namespace TheDish.Review.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ReviewDbContext _context;

    public ReviewRepository(ReviewDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<ReviewEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.Photos)
            .Include(r => r.HelpfulnessVotes)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<ReviewEntity>> GetReviewsByPlaceIdAsync(
        Guid placeId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.PlaceId == placeId && !r.IsDeleted && r.Status == Domain.Enums.ReviewStatus.Active)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.Photos)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ReviewEntity>> GetReviewsByUserIdAsync(
        Guid userId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.UserId == userId && !r.IsDeleted && r.Status == Domain.Enums.ReviewStatus.Active)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.Photos)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ReviewEntity>> GetRecentReviewsAsync(
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => !r.IsDeleted && r.Status == Domain.Enums.ReviewStatus.Active)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.Photos)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReviewEntity?> GetReviewByUserAndPlaceAsync(
        Guid userId,
        Guid placeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => 
                r.UserId == userId && 
                r.PlaceId == placeId && 
                !r.IsDeleted, 
                cancellationToken);
    }

    public async Task<ReviewEntity> AddAsync(ReviewEntity review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
        return review;
    }

    public async Task UpdateAsync(ReviewEntity review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Update(review);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(ReviewEntity review, CancellationToken cancellationToken = default)
    {
        review.MarkAsDeleted();
        _context.Reviews.Update(review);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AnyAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<int> GetReviewCountByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .CountAsync(r => r.PlaceId == placeId && !r.IsDeleted && r.Status == Domain.Enums.ReviewStatus.Active, cancellationToken);
    }

    public async Task<decimal> GetAverageRatingByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default)
    {
        var average = await _context.Reviews
            .Where(r => r.PlaceId == placeId && !r.IsDeleted && r.Status == Domain.Enums.ReviewStatus.Active)
            .Select(r => (decimal?)r.Rating)
            .AverageAsync(cancellationToken);

        return average ?? 0;
    }

    public async Task<ReviewHelpfulnessEntity?> GetHelpfulnessVoteAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.ReviewHelpfulness
            .FirstOrDefaultAsync(rh => rh.ReviewId == reviewId && rh.UserId == userId, cancellationToken);
    }

    public async Task AddHelpfulnessVoteAsync(ReviewHelpfulnessEntity vote, CancellationToken cancellationToken = default)
    {
        await _context.ReviewHelpfulness.AddAsync(vote, cancellationToken);
        await Task.CompletedTask;
    }

    public async Task UpdateHelpfulnessVoteAsync(ReviewHelpfulnessEntity vote, CancellationToken cancellationToken = default)
    {
        _context.ReviewHelpfulness.Update(vote);
        await Task.CompletedTask;
    }
}

