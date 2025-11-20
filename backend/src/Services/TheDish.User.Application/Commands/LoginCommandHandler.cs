using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.User.Application.DTOs;
using TheDish.User.Application.Interfaces;

namespace TheDish.User.Application.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Response<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                return Response<AuthResponseDto>.FailureResult("Invalid email or password");
            }

            // Check if user has password (not social login only)
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return Response<AuthResponseDto>.FailureResult("This account uses social login. Please sign in with your social provider.");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Response<AuthResponseDto>.FailureResult("Invalid email or password");
            }

            // Generate token
            var token = _tokenService.GenerateToken(user);

            var authResponse = new AuthResponseDto
            {
                Token = token,
                User = MapToDto(user)
            };

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return Response<AuthResponseDto>.SuccessResult(authResponse, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login: {Email}", request.Email);
            return Response<AuthResponseDto>.FailureResult("An error occurred during login");
        }
    }

    private static UserDto MapToDto(Domain.Entities.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExternalProvider = user.ExternalProvider.ToString(),
            Reputation = user.Reputation,
            ReviewCount = user.ReviewCount,
            IsVerified = user.IsVerified,
            JoinDate = user.JoinDate
        };
    }
}

