using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Petsgram.Application.Interfaces.Caching;

namespace Petsgram.Infrastructure.Services.RedisCache;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _defaultExpiration;
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public RedisCacheService(IDistributedCache cache, TimeSpan defaultExpiration)
    {
        _cache = cache;
        _defaultExpiration = defaultExpiration;
    }

    public async Task<T?> ReadAsync<T>(string key, CancellationToken ct = default)
    {
        var json = await _cache.GetStringAsync(key, ct);
        if (string.IsNullOrEmpty(json)) return default;
        return JsonSerializer.Deserialize<T>(json, Json);
    }

    public async Task WriteAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };
        var json = JsonSerializer.Serialize(value, Json);
        await _cache.SetStringAsync(key, json, options, ct);
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(key, ct);
        return true;
    }
}