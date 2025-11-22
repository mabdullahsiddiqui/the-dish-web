using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Queries;

public class GetPlaceByIdQuery : IRequest<Response<PlaceDto>>
{
    public Guid PlaceId { get; set; }
}











