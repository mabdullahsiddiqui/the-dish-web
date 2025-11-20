using MediatR;
using TheDish.Common.Application.Common;
using TheDish.User.Application.DTOs;

namespace TheDish.User.Application.Commands;

public class RegisterUserCommand : IRequest<Response<AuthResponseDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

