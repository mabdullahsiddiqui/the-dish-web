namespace TheDish.Place.Application.DTOs;

public class MenuItemDto
{
    public Guid Id { get; set; }
    public Guid PlaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
    public List<string> DietaryTags { get; set; } = new();
    public List<string> AllergenWarnings { get; set; } = new();
    public int? SpiceLevel { get; set; }
    public bool IsPopular { get; set; }
    public bool IsAvailable { get; set; }
    public string? PhotoUrl { get; set; }
}









