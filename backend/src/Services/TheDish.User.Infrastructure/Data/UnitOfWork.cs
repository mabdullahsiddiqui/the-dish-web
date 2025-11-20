using TheDish.Common.Application.Interfaces;
using TheDish.User.Infrastructure.Data;

namespace TheDish.User.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserDbContext _context;

    public UnitOfWork(UserDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

