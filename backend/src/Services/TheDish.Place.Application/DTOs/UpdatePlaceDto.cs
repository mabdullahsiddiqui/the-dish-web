namespace TheDish.Place.Application.DTOs;

public class UpdatePlaceDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public int? PriceRange { get; set; }
    public List<string>? CuisineTypes { get; set; }
    public Dictionary<string, object>? HoursOfOperation { get; set; }
    public List<string>? Amenities { get; set; }
    public string? ParkingInfo { get; set; }
}










