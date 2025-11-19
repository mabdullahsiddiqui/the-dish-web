namespace TheDish.Place.Application.DTOs;

public class PlaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public List<string> CuisineTypes { get; set; } = new();
    public int PriceRange { get; set; }
    public Dictionary<string, bool> DietaryTags { get; set; } = new();
    public Dictionary<string, int> TrustScores { get; set; } = new();
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public Guid? ClaimedBy { get; set; }
    public bool IsVerified { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<PlacePhotoDto> Photos { get; set; } = new();
    public List<MenuItemDto> MenuItems { get; set; } = new();
    public double? DistanceKm { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}









