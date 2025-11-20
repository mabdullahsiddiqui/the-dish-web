namespace TheDish.Review.Application.DTOs;

public class CreateReviewDto
{
    public Guid PlaceId { get; set; }
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public Dictionary<string, string>? DietaryAccuracy { get; set; }
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    public double? PlaceLatitude { get; set; }
    public double? PlaceLongitude { get; set; }
}










