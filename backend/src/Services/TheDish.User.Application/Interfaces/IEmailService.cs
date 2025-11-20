namespace TheDish.User.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetCodeAsync(string email, string code);
}

