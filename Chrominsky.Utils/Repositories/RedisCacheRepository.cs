using System.Text.Json;
using StackExchange.Redis;

namespace Chrominsky.Utils.Repositories;

/// <summary>
/// A class representing a Redis cache repository.
/// Implements the ICacheRepository interface.
/// </summary>
public class RedisCacheRepository : ICacheRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheRepository"/> class.
    /// </summary>
    /// <param name="redis">The Redis connection multiplexer.</param>
    public RedisCacheRepository(IConnectionMultiplexer redis)
    {
        _redis = redis?? throw new ArgumentNullException(nameof(redis));
        _database = _redis.GetDatabase();
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(value!);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serializedValue, expiry);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
}