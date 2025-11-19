using TheDish.Common.Domain.Entities;

namespace TheDish.Review.Domain.Entities;

public class ReviewPhoto : BaseEntity
{
    public Guid ReviewId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; private set; }
    public string? Caption { get; private set; }
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual Review Review { get; private set; } = null!;

    private ReviewPhoto() { }

    public ReviewPhoto(Guid reviewId, string url, Guid uploadedBy, string? caption = null)
    {
        if (reviewId == Guid.Empty)
            throw new ArgumentException("Review ID is required", nameof(reviewId));
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo URL is required", nameof(url));
        if (uploadedBy == Guid.Empty)
            throw new ArgumentException("Uploader ID is required", nameof(uploadedBy));

        ReviewId = reviewId;
        Url = url;
        UploadedBy = uploadedBy;
        Caption = caption;
        UploadedAt = DateTime.UtcNow;
    }

    public void SetThumbnail(string thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        UpdateTimestamp();
    }

    public void UpdateCaption(string? caption)
    {
        Caption = caption;
        UpdateTimestamp();
    }
}









