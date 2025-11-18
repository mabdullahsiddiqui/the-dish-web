namespace TheDish.Review.Application.DTOs;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PlaceId { get; set; }
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<string> PhotoUrls { get; set; } = new();
    public Dictionary<string, string> DietaryAccuracy { get; set; } = new();
    public bool GpsVerified { get; set; }
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ReviewPhotoDto> Photos { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}








