using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;

namespace TheDish.Review.Application.Queries;

public class GetReviewAnalysisQuery : IRequest<Response<ReviewAnalysisDto>>
{
    public Guid ReviewId { get; set; }
}






