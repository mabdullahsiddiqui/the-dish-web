using MediatR;
using TheDish.Common.Application.Common;
using TheDish.User.Application.DTOs;

namespace TheDish.User.Application.Commands;

public class SocialLoginCommand : IRequest<Response<AuthResponseDto>>
{
    public string Provider { get; set; } = string.Empty; // "Google" or "Facebook"
    public string Token { get; set; } = string.Empty; // ID token for Google, Access token for Facebook
}

