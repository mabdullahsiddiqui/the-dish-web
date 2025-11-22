using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Common.Application.Interfaces;
using TheDish.User.Application.Interfaces;
using UserEntity = TheDish.User.Domain.Entities.User;

namespace TheDish.User.Application.Commands;

public class UpdateReputationCommandHandler : IRequestHandler<UpdateReputationCommand, Response<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateReputationCommandHandler> _logger;

    public UpdateReputationCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateReputationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<bool>> Handle(UpdateReputationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Response<bool>.FailureResult("User not found");
            }

            user.UpdateReputation(request.Points);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reputation updated for user {UserId}: {Points} points (new total: {Reputation})", 
                request.UserId, request.Points, user.Reputation);

            return Response<bool>.SuccessResult(true, "Reputation updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reputation for user: {UserId}", request.UserId);
            return Response<bool>.FailureResult("An error occurred while updating reputation");
        }
    }
}


