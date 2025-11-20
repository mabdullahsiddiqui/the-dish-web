using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TheDish.Common.Application.Interfaces;
using TheDish.User.Application.Interfaces;
using TheDish.User.Domain.Enums;
using UserEntity = TheDish.User.Domain.Entities.User;
using TheDish.User.Infrastructure.Data;

namespace TheDish.User.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserEntity>> FindAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Users.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<UserEntity?> FirstOrDefaultAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<UserEntity> AddAsync(UserEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(UserEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(UserEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<UserEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _context.Users.CountAsync(cancellationToken);
        return await _context.Users.CountAsync(predicate, cancellationToken);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserEntity?> GetByExternalProviderAsync(string providerId, ExternalProvider provider, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => 
                u.ExternalProviderId == providerId && 
                u.ExternalProvider == provider, 
                cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}

