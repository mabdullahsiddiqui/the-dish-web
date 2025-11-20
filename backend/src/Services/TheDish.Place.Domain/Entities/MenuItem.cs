using TheDish.Common.Domain.Entities;

namespace TheDish.Place.Domain.Entities;

public class MenuItem : BaseEntity
{
    public Guid PlaceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal? Price { get; private set; }
    public string? Category { get; private set; }
    public List<string> DietaryTags { get; private set; } = new();
    public List<string> AllergenWarnings { get; private set; } = new();
    public int? SpiceLevel { get; private set; } // 0-5
    public bool IsPopular { get; private set; } = false;
    public bool IsAvailable { get; private set; } = true;
    public string? PhotoUrl { get; private set; }
    
    // Navigation property
    public virtual Place Place { get; private set; } = null!;

    private MenuItem() { }

    public MenuItem(
        Guid placeId,
        string name,
        decimal? price = null,
        string? category = null,
        string? description = null)
    {
        if (placeId == Guid.Empty)
            throw new ArgumentException("Place ID is required", nameof(placeId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Menu item name is required", nameof(name));

        PlaceId = placeId;
        Name = name;
        Price = price;
        Category = category;
        Description = description;
        DietaryTags = new List<string>();
        AllergenWarnings = new List<string>();
    }

    public void UpdateDetails(
        string? name = null,
        string? description = null,
        decimal? price = null,
        string? category = null,
        string? photoUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        Description = description;
        Price = price;
        Category = category;
        PhotoUrl = photoUrl;
        UpdateTimestamp();
    }

    public void SetDietaryTags(List<string> dietaryTags)
    {
        DietaryTags = dietaryTags ?? new List<string>();
        UpdateTimestamp();
    }

    public void SetAllergenWarnings(List<string> allergenWarnings)
    {
        AllergenWarnings = allergenWarnings ?? new List<string>();
        UpdateTimestamp();
    }

    public void SetSpiceLevel(int? spiceLevel)
    {
        if (spiceLevel.HasValue && (spiceLevel.Value < 0 || spiceLevel.Value > 5))
            throw new ArgumentException("Spice level must be between 0 and 5", nameof(spiceLevel));

        SpiceLevel = spiceLevel;
        UpdateTimestamp();
    }

    public void SetPopular(bool isPopular)
    {
        IsPopular = isPopular;
        UpdateTimestamp();
    }

    public void SetAvailable(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdateTimestamp();
    }
}










