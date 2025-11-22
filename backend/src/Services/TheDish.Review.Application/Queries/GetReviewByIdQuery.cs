using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;

namespace TheDish.Review.Application.Queries;

public class GetReviewByIdQuery : IRequest<Response<ReviewDto>>
{
    public Guid ReviewId { get; set; }
}











