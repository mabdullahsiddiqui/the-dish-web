using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Queries;

public class SearchPlacesQuery : IRequest<Response<SearchPlacesResponseDto>>
{
    public string? SearchTerm { get; set; }
    public List<string>? CuisineTypes { get; set; }
    public List<string>? DietaryTags { get; set; }
    public int? MinPriceRange { get; set; }
    public int? MaxPriceRange { get; set; }
    public decimal? MinRating { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}











