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
    /// Retrieves all entities of type T and DatabaseEntityStatus of Active from the database.
    /// </summary>
    /// <typeparam name="T">Type of entity to be retrieved.</typeparam>
    /// <returns>An enumerable collection of entities of type T.</returns>
    Task<IEnumerable<T>> GetAllActiveAsync<T>() where T : class, IBaseDatabaseEntity;
    
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
    
    /// <summary>
    /// Retrieves a paginated list of entities of type T from the database.
    /// </summary>
    /// <typeparam name="T">Type of entity to be retrieved.</typeparam>
    /// <param name="page">The page number to retrieve. Pages are 1-indexed.</param>
    /// <param name="pageSize">The number of entities to include in each page.</param>
    /// <returns>A PaginatedResponse containing the requested page of entities of type T.</returns>
    /// <remarks>
    /// The PaginatedResponse class contains properties for the total number of entities,
    /// the current page number, the page size, and the collection of entities.
    /// If no entities are found for the specified page and page size, the collection will be empty.
    /// </remarks>
    Task<PaginatedResponse<IEnumerable<T>>> GetPaginatedAsync<T>(int page, int pageSize) where T : class, IBaseDatabaseEntity;

    /// <summary>
    /// Retrieves the columns of a table for the specified entity type.
    /// </summary>
    /// <param name="tableName">Optional parameter to pass if table name is different than dbContext entity name.</param>
    /// <typeparam name="T">Type of entity whose table columns are to be retrieved.</typeparam>
    /// <returns>A <see cref="TableColumns"/> containing the table name and columns.</returns>
    TableColumns? GetTableColumnsAsync<T>(string? tableName = null) where T : class, IBaseDatabaseEntity;
}