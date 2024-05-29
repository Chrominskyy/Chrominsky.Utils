namespace Chrominsky.Utils.Services;

/// <summary>
/// Interface for cache service.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets the value associated with the specified key, or adds it to the cache if it doesn't exist.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <param name="failover">A function to retrieve the value if it doesn't exist in the cache.</param>
    /// <param name="expiry">The expiration time of the value in the cache. If not provided, the default expiration time will be used.</param>
    /// <returns>The value associated with the specified key.</returns>
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> failover, TimeSpan? expiry = null);
    
    /// <summary>
    /// Removes the value associated with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>True if the value was successfully removed; otherwise, false.</returns>
    Task<bool> RemoveAsync(string key);
    
    /// <summary>
    /// Checks if the cache contains a value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to check.</param>
    /// <returns>True if the cache contains a value associated with the specified key; otherwise, false.</returns>
    Task<bool> ExistsAsync(string key);
}