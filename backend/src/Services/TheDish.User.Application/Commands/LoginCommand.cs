using MediatR;
using TheDish.Common.Application.Common;
using TheDish.User.Application.DTOs;

namespace TheDish.User.Application.Commands;

public class LoginCommand : IRequest<Response<AuthResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

