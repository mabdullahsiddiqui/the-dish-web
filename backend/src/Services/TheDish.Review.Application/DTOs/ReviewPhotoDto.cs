namespace TheDish.Review.Application.DTOs;

public class ReviewPhotoDto
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}










