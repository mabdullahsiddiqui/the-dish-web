namespace TheDish.Review.Application.DTOs;

public class UpdateReviewDto
{
    public int? Rating { get; set; }
    public string? Text { get; set; }
    public Dictionary<string, string>? DietaryAccuracy { get; set; }
}










