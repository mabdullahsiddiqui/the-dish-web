using MediatR;
using TheDish.Common.Application.Common;

namespace TheDish.User.Application.Commands;

public class ForgotPasswordCommand : IRequest<Response<bool>>
{
    public string Email { get; set; } = string.Empty;
}

