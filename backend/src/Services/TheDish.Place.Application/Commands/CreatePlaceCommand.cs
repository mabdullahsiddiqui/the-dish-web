using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Commands;

public class CreatePlaceCommand : IRequest<Response<PlaceDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public int PriceRange { get; set; } = 1;
    public List<string>? CuisineTypes { get; set; }
    public Dictionary<string, object>? HoursOfOperation { get; set; }
    public List<string>? Amenities { get; set; }
    public string? ParkingInfo { get; set; }
}










