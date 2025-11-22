using TheDish.Common.Domain.Entities;

namespace TheDish.Review.Domain.Entities;

public class ReviewHelpfulness : BaseEntity
{
    public Guid ReviewId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsHelpful { get; private set; }
    
    // Navigation property
    public virtual Review Review { get; private set; } = null!;

    private ReviewHelpfulness() { }

    public ReviewHelpfulness(Guid reviewId, Guid userId, bool isHelpful)
    {
        if (reviewId == Guid.Empty)
            throw new ArgumentException("Review ID is required", nameof(reviewId));
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(userId));

        ReviewId = reviewId;
        UserId = userId;
        IsHelpful = isHelpful;
    }

    public void UpdateVote(bool isHelpful)
    {
        IsHelpful = isHelpful;
        UpdateTimestamp();
    }
}










