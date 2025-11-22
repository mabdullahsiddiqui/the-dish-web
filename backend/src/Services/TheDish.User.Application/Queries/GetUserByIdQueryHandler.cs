using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.User.Application.DTOs;
using TheDish.User.Application.Interfaces;
using UserEntity = TheDish.User.Domain.Entities.User;

namespace TheDish.User.Application.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Response<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Response<UserDto>.FailureResult("User not found");
            }

            var userDto = MapToDto(user);
            return Response<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", request.UserId);
            return Response<UserDto>.FailureResult("An error occurred while retrieving user");
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

