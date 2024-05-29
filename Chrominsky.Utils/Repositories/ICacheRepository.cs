namespace Chrominsky.Utils.Repositories;

/// <summary>
/// Interface for cache repository operations.
/// </summary>
public interface ICacheRepository
{
    /// <summary>
    /// Retrieves an item from the cache asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the item to retrieve.</typeparam>
    /// <param name="key">The unique identifier for the item in the cache.</param>
    /// <returns>The retrieved item, or null if not found.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Stores an item in the cache asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the item to store.</typeparam>
    /// <param name="key">The unique identifier for the item in the cache.</param>
    /// <param name="value">The item to store.</param>
    /// <param name="expiry">The optional expiry time for the item in the cache. If not provided, the default expiry time will be used.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    /// Removes an item from the cache asynchronously.
    /// </summary>
    /// <param name="key">The unique identifier for the item in the cache.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    Task<bool> RemoveAsync(string key);

    /// <summary>
    /// Checks if an item exists in the cache asynchronously.
    /// </summary>
    /// <param name="key">The unique identifier for the item in the cache.</param>
    /// <returns>True if the item exists in the cache; otherwise, false.</returns>
    Task<bool> ExistsAsync(string key);
}