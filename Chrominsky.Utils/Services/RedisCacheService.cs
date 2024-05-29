using Chrominsky.Utils.Repositories;

namespace Chrominsky.Utils.Services;

/// <summary>
/// Implements the caching service using Redis as the storage backend.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly ICacheRepository _cacheRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
    /// </summary>
    /// <param name="cacheRepository">The cache repository.</param>
    public RedisCacheService(ICacheRepository cacheRepository)
    {
        _cacheRepository = cacheRepository;
    }

    /// <inheritdoc />
    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> failover, TimeSpan? expiry = null)
    {
        var value = await _cacheRepository.GetAsync<T>(key);
        if (value == null || value.Equals(default(T)))
        {
            value = await failover();
            if (value!= null &&!value.Equals(default(T)))
            {
                await _cacheRepository.SetAsync(key, value, expiry);
            }
        }
        return value;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveAsync(string key)
    {
        return await _cacheRepository.RemoveAsync(key);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key)
    {
        return await _cacheRepository.ExistsAsync(key);
    }
}