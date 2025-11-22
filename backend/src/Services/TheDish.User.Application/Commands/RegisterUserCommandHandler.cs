using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Common.Application.Interfaces;
using TheDish.User.Application.DTOs;
using TheDish.User.Application.Interfaces;
using UserEntity = TheDish.User.Domain.Entities.User;
using TheDish.User.Domain.Enums;

namespace TheDish.User.Application.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Response<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Response<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            {
                return Response<AuthResponseDto>.FailureResult("Email already registered");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user
            var user = UserEntity.CreateWithEmail(
                request.Email,
                request.FirstName,
                request.LastName,
                passwordHash);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate token
            var token = _tokenService.GenerateToken(user);

            var authResponse = new AuthResponseDto
            {
                Token = token,
                User = MapToDto(user)
            };

            _logger.LogInformation("User registered successfully: {Email}", request.Email);

            return Response<AuthResponseDto>.SuccessResult(authResponse, "User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", request.Email);
            return Response<AuthResponseDto>.FailureResult("An error occurred while registering the user");
        }
    }

    private static UserDto MapToDto(UserEntity user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExternalProvider = user.ExternalProvider.ToString(),
            Reputation = user.Reputation,
            ReputationLevel = user.GetReputationLevel().ToString(),
            ReviewCount = user.ReviewCount,
            IsVerified = user.IsVerified,
            JoinDate = user.JoinDate
        };
    }
}

