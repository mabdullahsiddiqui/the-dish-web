using NetTopologySuite.Geometries;
using TheDish.Common.Domain.Entities;
using TheDish.Place.Domain.Enums;

namespace TheDish.Place.Domain.Entities;

public class Place : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public Point Location { get; private set; } = null!; // PostGIS Point
    public string? Phone { get; private set; }
    public string? Website { get; private set; }
    public string? Email { get; private set; }
    
    // Cuisine and pricing
    public List<string> CuisineTypes { get; private set; } = new();
    public int PriceRange { get; private set; } // 1-4
    
    // Dietary information as JSONB
    public Dictionary<string, bool> DietaryTags { get; private set; } = new();
    public Dictionary<string, int> TrustScores { get; private set; } = new();
    
    // Ratings and reviews
    public decimal AverageRating { get; private set; } = 0;
    public int ReviewCount { get; private set; } = 0;
    
    // Business claim
    public Guid? ClaimedBy { get; private set; }
    public bool IsVerified { get; private set; } = false;
    public PlaceStatus Status { get; private set; } = PlaceStatus.Active;
    
    // Additional info
    public Dictionary<string, object>? HoursOfOperation { get; private set; }
    public List<string> Amenities { get; private set; } = new();
    public string? ParkingInfo { get; private set; }
    
    // Navigation properties
    public virtual ICollection<PlacePhoto> Photos { get; private set; } = new List<PlacePhoto>();
    public virtual ICollection<MenuItem> MenuItems { get; private set; } = new List<MenuItem>();
    public virtual ICollection<DietaryCertification> Certifications { get; private set; } = new List<DietaryCertification>();
    
    private Place() { }

    public Place(
        string name,
        string address,
        double latitude,
        double longitude,
        int priceRange,
        List<string>? cuisineTypes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Place name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required", nameof(address));
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));
        if (priceRange < 1 || priceRange > 4)
            throw new ArgumentException("Price range must be between 1 and 4", nameof(priceRange));

        Name = name;
        Address = address;
        Location = new Point(longitude, latitude) { SRID = 4326 }; // WGS84
        PriceRange = priceRange;
        CuisineTypes = cuisineTypes ?? new List<string>();
        DietaryTags = new Dictionary<string, bool>();
        TrustScores = new Dictionary<string, int>();
        Amenities = new List<string>();
    }

    public void UpdateDetails(
        string? name = null,
        string? address = null,
        string? phone = null,
        string? website = null,
        string? email = null,
        int? priceRange = null,
        List<string>? cuisineTypes = null,
        Dictionary<string, object>? hoursOfOperation = null,
        List<string>? amenities = null,
        string? parkingInfo = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        if (!string.IsNullOrWhiteSpace(address))
            Address = address;
        Phone = phone;
        Website = website;
        Email = email;
        if (priceRange.HasValue && priceRange.Value >= 1 && priceRange.Value <= 4)
            PriceRange = priceRange.Value;
        if (cuisineTypes != null)
            CuisineTypes = cuisineTypes;
        HoursOfOperation = hoursOfOperation;
        if (amenities != null)
            Amenities = amenities;
        ParkingInfo = parkingInfo;
        UpdateTimestamp();
    }

    public void UpdateLocation(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Location = new Point(longitude, latitude) { SRID = 4326 };
        UpdateTimestamp();
    }

    public void Claim(Guid userId)
    {
        ClaimedBy = userId;
        UpdateTimestamp();
    }

    public void Verify()
    {
        IsVerified = true;
        UpdateTimestamp();
    }

    public void UpdateDietaryTags(Dictionary<string, bool> dietaryTags)
    {
        DietaryTags = dietaryTags ?? new Dictionary<string, bool>();
        UpdateTimestamp();
    }

    public void UpdateTrustScores(Dictionary<string, int> trustScores)
    {
        TrustScores = trustScores ?? new Dictionary<string, int>();
        UpdateTimestamp();
    }

    public void UpdateRating(decimal averageRating, int reviewCount)
    {
        if (averageRating < 0 || averageRating > 5)
            throw new ArgumentException("Average rating must be between 0 and 5", nameof(averageRating));
        if (reviewCount < 0)
            throw new ArgumentException("Review count cannot be negative", nameof(reviewCount));

        AverageRating = averageRating;
        ReviewCount = reviewCount;
        UpdateTimestamp();
    }

    public void ChangeStatus(PlaceStatus status)
    {
        Status = status;
        UpdateTimestamp();
    }
}








