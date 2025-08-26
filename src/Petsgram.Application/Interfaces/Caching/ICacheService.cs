namespace Petsgram.Application.Interfaces.Caching;

public interface ICacheService
{
    Task<T?> ReadAsync<T>(string key, CancellationToken ct = default);
    Task WriteAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);
    Task<bool> RemoveAsync(string key, CancellationToken ct = default);
}