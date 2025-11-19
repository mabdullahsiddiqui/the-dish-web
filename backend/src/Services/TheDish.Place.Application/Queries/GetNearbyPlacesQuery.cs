using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Queries;

public class GetNearbyPlacesQuery : IRequest<Response<List<PlaceDto>>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10.0;
    public List<string>? DietaryFilters { get; set; }
    public List<string>? CuisineFilters { get; set; }
    public int? PriceRange { get; set; }
}









