namespace TheDish.Place.Application.DTOs;

public class PlacePhotoDto
{
    public Guid Id { get; set; }
    public Guid PlaceId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public Guid UploadedBy { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime UploadedAt { get; set; }
}











