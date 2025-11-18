using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;

namespace TheDish.Review.Application.Queries;

public class GetReviewsByPlaceIdQuery : IRequest<Response<ReviewListResponseDto>>
{
    public Guid PlaceId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}








