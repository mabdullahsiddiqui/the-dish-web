using MediatR;
using TheDish.Common.Application.Common;
using TheDish.Review.Application.DTOs;

namespace TheDish.Review.Application.Commands;

public class UpdateReviewCommand : IRequest<Response<ReviewDto>>
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public int? Rating { get; set; }
    public string? Text { get; set; }
    public Dictionary<string, string>? DietaryAccuracy { get; set; }
}








