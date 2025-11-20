using MediatR;
using TheDish.Common.Application.Common;

namespace TheDish.Review.Application.Commands;

public class DeleteReviewCommand : IRequest<Response<bool>>
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
}










