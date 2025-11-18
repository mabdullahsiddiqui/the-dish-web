using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;

namespace TheDish.Review.Application.Commands;

public class MarkReviewHelpfulCommand : IRequest<Response<ReviewDto>>
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public bool IsHelpful { get; set; }
}








