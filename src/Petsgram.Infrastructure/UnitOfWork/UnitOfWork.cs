using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Infrastructure.DbContexts;

namespace Petsgram.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly PetsgramDbContext _context;
    private bool _disposed;

    public UnitOfWork(PetsgramDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}