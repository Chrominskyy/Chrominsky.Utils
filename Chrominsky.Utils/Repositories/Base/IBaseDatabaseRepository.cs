using Chrominsky.Utils.Models.Base;

namespace Chrominsky.Utils.Repositories.Base;

/// <summary>
/// Interface for base database repository operations.
/// </summary>
/// <typeparam name="T">Type of entity to be handled by the repository.</typeparam>
public interface IBaseDatabaseRepository<T>
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <typeparam name="T">Type of entity to be retrieved.</typeparam>
    /// <param name="id">Unique identifier of the entity.</param>
    /// <returns>The entity with the specified identifier.</returns>
    Task<T> GetByIdAsync<T>(Guid id) where T : class;

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <typeparam name="T">Type of entity to be added.</typeparam>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>The unique identifier of the newly added entity.</returns>
    Task<Guid> AddAsync<T>(T entity) where T : BaseDatabaseEntity;

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <typeparam name="T">Type of entity to be updated.</typeparam>
    /// <param name="entity">The updated entity.</param>
    /// <returns>The updated entity.</returns>
    Task<T> UpdateAsync<T>(T entity) where T : BaseDatabaseEntity;

    /// <summary>
    /// Deletes an entity from the database by its unique identifier.
    /// </summary>
    /// <typeparam name="T">Type of entity to be deleted.</typeparam>
    /// <param name="id">Unique identifier of the entity.</param>
    /// <returns>True if the entity was successfully deleted, otherwise false.</returns>
    Task<bool> DeleteAsync<T>(Guid id) where T : BaseDatabaseEntity;
}