namespace Petsgram.Application.Interfaces.Shared;

public interface IGenericRepository<T> where T : class
{
    Task<T?> FindAsync(int id, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(int id, CancellationToken cancellationToken = default);
}
