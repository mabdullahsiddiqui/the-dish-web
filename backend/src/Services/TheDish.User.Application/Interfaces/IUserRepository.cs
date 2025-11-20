using TheDish.Common.Application.Interfaces;
using TheDish.User.Domain.Enums;
using UserEntity = TheDish.User.Domain.Entities.User;

namespace TheDish.User.Application.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByExternalProviderAsync(string providerId, ExternalProvider provider, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

