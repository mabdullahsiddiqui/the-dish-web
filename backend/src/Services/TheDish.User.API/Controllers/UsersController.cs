using MediatR;
using Microsoft.AspNetCore.Mvc;
using TheDish.Common.Application.Common;
using TheDish.User.Application.Commands;
using TheDish.User.Application.DTOs;
using TheDish.User.Application.Queries;

namespace TheDish.User.API.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Response<AuthResponseDto>>> Register([FromBody] RegisterUserDto dto)
    {
        var command = new RegisterUserCommand
        {
            Email = dto.Email,
            Password = dto.Password,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response<AuthResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var command = new LoginCommand
        {
            Email = dto.Email,
            Password = dto.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("auth/google")]
    public async Task<ActionResult<Response<AuthResponseDto>>> GoogleLogin([FromBody] SocialLoginDto dto)
    {
        var command = new SocialLoginCommand
        {
            Provider = "Google",
            Token = dto.Token
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("auth/facebook")]
    public async Task<ActionResult<Response<AuthResponseDto>>> FacebookLogin([FromBody] SocialLoginDto dto)
    {
        var command = new SocialLoginCommand
        {
            Provider = "Facebook",
            Token = dto.Token
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<Response<bool>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var command = new ForgotPasswordCommand
        {
            Email = dto.Email
        };

        var result = await _mediator.Send(command);

        // Always return success for security (prevent email enumeration)
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<Response<bool>>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var command = new ResetPasswordCommand
        {
            Email = dto.Email,
            Code = dto.Code,
            NewPassword = dto.NewPassword
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<UserDto>>> GetUser(Guid id)
    {
        var query = new GetUserByIdQuery
        {
            UserId = id
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/reputation")]
    public async Task<ActionResult<Response<bool>>> UpdateReputation(
        Guid id,
        [FromBody] UpdateReputationRequest request)
    {
        var command = new UpdateReputationCommand
        {
            UserId = id,
            Points = request.Points
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}

public class UpdateReputationRequest
{
    public int Points { get; set; }
}

