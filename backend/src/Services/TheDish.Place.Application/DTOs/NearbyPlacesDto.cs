namespace TheDish.Place.Application.DTOs;

public class NearbyPlacesDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10.0;
    public List<string>? DietaryFilters { get; set; }
    public List<string>? CuisineFilters { get; set; }
    public int? PriceRange { get; set; }
}










