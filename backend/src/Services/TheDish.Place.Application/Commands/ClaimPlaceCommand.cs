using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Place.Application.DTOs;

namespace TheDish.Place.Application.Commands;

public class ClaimPlaceCommand : IRequest<Response<PlaceDto>>
{
    public Guid PlaceId { get; set; }
    public Guid UserId { get; set; }
}








