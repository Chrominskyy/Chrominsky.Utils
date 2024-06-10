using Chrominsky.Utils.Models;
using Chrominsky.Utils.Models.Base.Interfaces;

namespace Chrominsky.Utils.Repositories.Base;

/// <summary>
/// Interface for base database repository operations.
/// </summary>
/// <typeparam name="T">Type of entity to be handled by the repository.</typeparam>
public interface IBaseDatabaseRepository<T>
{
    
    /// <summary>
    /// Retrieves all entities of type T from the database.
    /// </summary>
    /// <typeparam name="T">Type of entity to be retrieved.</typeparam>
    /// <returns>An enumerable collection of entities of type T.</returns>
    Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
    
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
    Task<Guid> AddAsync<T>(T entity) where T : class, IBaseDatabaseEntity;

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <typeparam name="T">Type of entity to be updated.</typeparam>
    /// <param name="entity">The updated entity.</param>
    /// <returns>The updated entity.</returns>
    Task<T?> UpdateAsync<T>(T? entity) where T : class, IBaseDatabaseEntity;

    /// <summary>
    /// Deletes an entity from the database by its unique identifier.
    /// </summary>
    /// <typeparam name="T">Type of entity to be deleted.</typeparam>
    /// <param name="id">Unique identifier of the entity.</param>
    /// <returns>True if the entity was successfully deleted, otherwise false.</returns>
    Task<bool> DeleteAsync<T>(Guid id) where T : class, IBaseDatabaseEntity;

    /// <summary>
    /// Performs a search operation on the database based on the provided search parameters.
    /// </summary>
    /// <typeparam name="T">Type of entity to be searched.</typeparam>
    /// <param name="request">The search parameters and criteria.</param>
    /// <returns>An enumerable collection of entities of type T that match the search criteria.</returns>
    /// <remarks>
    /// This method is intended to be used for searching entities based on specific conditions.
    /// The search parameters are defined in the <see cref="SearchParameterRequest"/> class.
    /// The returned collection may be empty if no matching entities are found.
    /// </remarks>
    Task<List<T>> SearchAsync<T>(SearchParameterRequest request) where T : class, IBaseDatabaseEntity;
}