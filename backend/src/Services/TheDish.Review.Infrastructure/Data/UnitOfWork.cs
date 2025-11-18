using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ReviewDbContext _context;

    public UnitOfWork(ReviewDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}








