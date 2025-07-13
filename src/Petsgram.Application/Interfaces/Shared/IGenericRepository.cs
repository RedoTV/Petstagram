namespace Petsgram.Application.Interfaces.Shared;

public interface IGenericRepository<T> where T : class
{
    Task<T?> FindAsync(int id);
    Task<T> AddAsync(T entity);
    Task RemoveAsync(int id);
    Task UpdateAsync(T entity);
}
