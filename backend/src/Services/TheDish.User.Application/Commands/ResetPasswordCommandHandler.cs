using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Common.Application.Interfaces;
using TheDish.User.Application.Interfaces;
using UserEntity = TheDish.User.Domain.Entities.User;

namespace TheDish.User.Application.Commands;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Response<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Response<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate code format (6 digits)
            if (string.IsNullOrEmpty(request.Code) || request.Code.Length != 6 || !request.Code.All(char.IsDigit))
            {
                return Response<bool>.FailureResult("Invalid reset code format. Code must be 6 digits.");
            }

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                return Response<bool>.FailureResult("Invalid email or reset code.");
            }

            // Validate reset code
            if (!user.IsPasswordResetCodeValid(request.Code))
            {
                _logger.LogWarning("Invalid or expired password reset code for user: {Email}", request.Email);
                return Response<bool>.FailureResult("Invalid or expired reset code. Please request a new code.");
            }

            // Validate password strength (basic validation)
            if (string.IsNullOrEmpty(request.NewPassword) || request.NewPassword.Length < 8)
            {
                return Response<bool>.FailureResult("Password must be at least 8 characters long.");
            }

            // Hash new password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Update password
            user.UpdatePasswordHash(passwordHash);
            user.ClearPasswordResetCode();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Password reset successful for user: {Email}", user.Email);

            return Response<bool>.SuccessResult(true, "Password has been reset successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset: {Email}", request.Email);
            return Response<bool>.FailureResult("An error occurred while resetting the password.");
        }
    }
}

