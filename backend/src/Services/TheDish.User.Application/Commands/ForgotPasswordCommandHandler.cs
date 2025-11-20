using System.Security.Cryptography;
using MediatR;
using Microsoft.Extensions.Logging;
using TheDish.Common.Application.Common;
using TheDish.Common.Application.Interfaces;
using TheDish.User.Application.Interfaces;
using UserEntity = TheDish.User.Domain.Entities.User;

namespace TheDish.User.Application.Commands;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Response<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Response<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            // Security: Always return success even if email not found (prevent email enumeration)
            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                return Response<bool>.SuccessResult(true, "If the email exists, a password reset code has been sent.");
            }

            // Allow password reset for social login accounts (they can set a password for the first time)
            // No need to check if PasswordHash is null - we'll allow setting password for social accounts

            // Generate 6-digit code using cryptographically secure random number generator
            var codeBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(codeBytes);
            }
            var code = (Math.Abs(BitConverter.ToInt32(codeBytes, 0)) % 900000 + 100000).ToString();

            // Set expiry to 15 minutes from now
            var expiry = DateTime.UtcNow.AddMinutes(15);

            // Save code to user
            user.SetPasswordResetCode(code, expiry);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send email with code
            await _emailService.SendPasswordResetCodeAsync(user.Email, code);

            _logger.LogInformation("Password reset code generated for user: {Email}", user.Email);

            return Response<bool>.SuccessResult(true, "If the email exists, a password reset code has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password: {Email}", request.Email);
            // Still return success for security (prevent email enumeration)
            return Response<bool>.SuccessResult(true, "If the email exists, a password reset code has been sent.");
        }
    }
}

