using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Common.Application.Interfaces;
using TheDish.User.Application.DTOs;
using TheDish.User.Application.Interfaces;
using UserEntity = TheDish.User.Domain.Entities.User;
using TheDish.User.Domain.Enums;

namespace TheDish.User.Application.Commands;

public class SocialLoginCommandHandler : IRequestHandler<SocialLoginCommand, Response<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IOAuthService _oauthService;
    private readonly ILogger<SocialLoginCommandHandler> _logger;

    public SocialLoginCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IOAuthService oauthService,
        ILogger<SocialLoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _oauthService = oauthService;
        _logger = logger;
    }

    public async Task<Response<AuthResponseDto>> Handle(SocialLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate token and get user info
            OAuthUserInfo? userInfo = null;
            ExternalProvider provider;

            if (request.Provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
            {
                provider = ExternalProvider.Google;
                try
                {
                    userInfo = await _oauthService.ValidateGoogleTokenAsync(request.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Google token validation failed: {Message}", ex.Message);
                    return Response<AuthResponseDto>.FailureResult($"Google token validation failed: {ex.Message}");
                }
            }
            else if (request.Provider.Equals("Facebook", StringComparison.OrdinalIgnoreCase))
            {
                provider = ExternalProvider.Facebook;
                try
                {
                    userInfo = await _oauthService.ValidateFacebookTokenAsync(request.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Facebook token validation failed: {Message}", ex.Message);
                    return Response<AuthResponseDto>.FailureResult($"Facebook token validation failed: {ex.Message}");
                }
            }
            else
            {
                return Response<AuthResponseDto>.FailureResult("Invalid provider");
            }

            if (userInfo == null)
            {
                _logger.LogWarning("OAuth service returned null user info for provider: {Provider}", request.Provider);
                return Response<AuthResponseDto>.FailureResult("Invalid token or unable to retrieve user information");
            }

            // Check if user exists by external provider ID
            var user = await _userRepository.GetByExternalProviderAsync(userInfo.Id, provider, cancellationToken);

            // If not found by provider ID, check by email (account linking)
            if (user == null && !string.IsNullOrEmpty(userInfo.Email))
            {
                user = await _userRepository.GetByEmailAsync(userInfo.Email, cancellationToken);
                
                // Link social account to existing email account
                if (user != null)
                {
                    user.LinkSocialAccount(provider, userInfo.Id, userInfo.Email);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            // Create new user if doesn't exist
            if (user == null)
            {
                user = UserEntity.CreateWithSocialLogin(
                    userInfo.Email,
                    userInfo.FirstName,
                    userInfo.LastName,
                    provider,
                    userInfo.Id);

                await _userRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Generate token
            var token = _tokenService.GenerateToken(user);

            var authResponse = new AuthResponseDto
            {
                Token = token,
                User = MapToDto(user)
            };

            _logger.LogInformation("Social login successful: {Provider} - {Email}", request.Provider, userInfo.Email);

            return Response<AuthResponseDto>.SuccessResult(authResponse, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during social login: {Provider}", request.Provider);
            return Response<AuthResponseDto>.FailureResult("An error occurred during social login");
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
            ReviewCount = user.ReviewCount,
            IsVerified = user.IsVerified,
            JoinDate = user.JoinDate
        };
    }
}

