namespace TheDish.User.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Domain.Entities.User user);
}

