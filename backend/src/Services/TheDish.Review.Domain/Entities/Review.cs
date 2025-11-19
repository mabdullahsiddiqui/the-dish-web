using NetTopologySuite.Geometries;
using TheDish.Common.Domain.Entities;
using TheDish.Review.Domain.Enums;

namespace TheDish.Review.Domain.Entities;

public class Review : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid PlaceId { get; private set; }
    public int Rating { get; private set; } // 1-5
    public string Text { get; private set; } = string.Empty;
    
    // Photos
    public List<string> PhotoUrls { get; private set; } = new();
    
    // Dietary accuracy (JSONB)
    public Dictionary<string, string> DietaryAccuracy { get; private set; } = new(); // dietaryType -> "accurate" | "inaccurate" | "unsure"
    
    // GPS verification
    public bool GpsVerified { get; private set; } = false;
    public Point? CheckInLocation { get; private set; } // PostGIS Point
    
    // Helpfulness
    public int HelpfulCount { get; private set; } = 0;
    public int NotHelpfulCount { get; private set; } = 0;
    
    // Status
    public ReviewStatus Status { get; private set; } = ReviewStatus.Active;
    
    // Navigation properties
    public virtual ICollection<ReviewPhoto> Photos { get; private set; } = new List<ReviewPhoto>();
    public virtual ICollection<ReviewHelpfulness> HelpfulnessVotes { get; private set; } = new List<ReviewHelpfulness>();

    private Review() { }

    public Review(
        Guid userId,
        Guid placeId,
        int rating,
        string text)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(userId));
        if (placeId == Guid.Empty)
            throw new ArgumentException("Place ID is required", nameof(placeId));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Review text is required", nameof(text));

        UserId = userId;
        PlaceId = placeId;
        Rating = rating;
        Text = text;
        PhotoUrls = new List<string>();
        DietaryAccuracy = new Dictionary<string, string>();
    }

    public void SetGpsVerification(bool verified, double? latitude = null, double? longitude = null)
    {
        GpsVerified = verified;
        
        if (verified && latitude.HasValue && longitude.HasValue)
        {
            CheckInLocation = new Point(longitude.Value, latitude.Value) { SRID = 4326 };
        }
        
        UpdateTimestamp();
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Review text is required", nameof(text));

        Text = text;
        UpdateTimestamp();
    }

    public void UpdateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        Rating = rating;
        UpdateTimestamp();
    }

    public void AddPhotoUrl(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new ArgumentException("Photo URL is required", nameof(photoUrl));

        if (!PhotoUrls.Contains(photoUrl))
        {
            PhotoUrls.Add(photoUrl);
            UpdateTimestamp();
        }
    }

    public void SetDietaryAccuracy(Dictionary<string, string> dietaryAccuracy)
    {
        DietaryAccuracy = dietaryAccuracy ?? new Dictionary<string, string>();
        UpdateTimestamp();
    }

    public void IncrementHelpfulCount()
    {
        HelpfulCount++;
        UpdateTimestamp();
    }

    public void IncrementNotHelpfulCount()
    {
        NotHelpfulCount++;
        UpdateTimestamp();
    }

    public void DecrementHelpfulCount()
    {
        if (HelpfulCount > 0)
        {
            HelpfulCount--;
            UpdateTimestamp();
        }
    }

    public void DecrementNotHelpfulCount()
    {
        if (NotHelpfulCount > 0)
        {
            NotHelpfulCount--;
            UpdateTimestamp();
        }
    }

    public void ChangeStatus(ReviewStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }
}









