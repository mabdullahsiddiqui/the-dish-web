using TheDish.Place.Application.Interfaces;

namespace TheDish.Place.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly PlaceDbContext _context;

    public UnitOfWork(PlaceDbContext context)
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










