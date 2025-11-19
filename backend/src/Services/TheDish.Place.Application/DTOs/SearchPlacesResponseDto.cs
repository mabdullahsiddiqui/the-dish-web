namespace TheDish.Place.Application.DTOs;

public class SearchPlacesResponseDto
{
    public List<PlaceDto> Places { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}









