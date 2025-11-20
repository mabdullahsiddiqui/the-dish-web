using TheDish.Common.Domain.Entities;

namespace TheDish.Place.Domain.Entities;

public class PlacePhoto : BaseEntity
{
    public Guid PlaceId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; private set; }
    public string? Caption { get; private set; }
    public Guid UploadedBy { get; private set; }
    public bool IsFeatured { get; private set; } = false;
    public int DisplayOrder { get; private set; } = 0;
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual Place Place { get; private set; } = null!;

    private PlacePhoto() { }

    public PlacePhoto(Guid placeId, string url, Guid uploadedBy, string? caption = null)
    {
        if (placeId == Guid.Empty)
            throw new ArgumentException("Place ID is required", nameof(placeId));
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo URL is required", nameof(url));
        if (uploadedBy == Guid.Empty)
            throw new ArgumentException("Uploader ID is required", nameof(uploadedBy));

        PlaceId = placeId;
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

    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
        UpdateTimestamp();
    }

    public void SetDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
        UpdateTimestamp();
    }

    public void UpdateCaption(string? caption)
    {
        Caption = caption;
        UpdateTimestamp();
    }
}










