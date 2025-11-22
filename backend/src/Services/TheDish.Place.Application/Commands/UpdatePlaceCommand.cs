using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Commands;

public class UpdatePlaceCommand : IRequest<Response<PlaceDto>>
{
    public Guid PlaceId { get; set; }
    public Guid UserId { get; set; } // For authorization check
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











