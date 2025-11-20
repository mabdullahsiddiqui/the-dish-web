using MediatR;
using TheDish.Common.Application.Common;

namespace TheDish.User.Application.Commands;

public class ResetPasswordCommand : IRequest<Response<bool>>
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

