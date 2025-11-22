using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Commands;

public class UpdatePlaceRatingCommand : IRequest<Response<PlaceDto>>
{
    public Guid PlaceId { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
}


