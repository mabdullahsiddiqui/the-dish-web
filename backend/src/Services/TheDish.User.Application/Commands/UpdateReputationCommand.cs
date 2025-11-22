using MediatR;
using TheDish.Common.Application.Common;

namespace TheDish.User.Application.Commands;

public class UpdateReputationCommand : IRequest<Response<bool>>
{
    public Guid UserId { get; set; }
    public int Points { get; set; }
}


