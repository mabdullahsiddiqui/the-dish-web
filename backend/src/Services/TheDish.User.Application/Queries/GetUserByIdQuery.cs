using MediatR;
using TheDish.Common.Application.Common;
using TheDish.User.Application.DTOs;

namespace TheDish.User.Application.Queries;

public class GetUserByIdQuery : IRequest<Response<UserDto>>
{
    public Guid UserId { get; set; }
}


