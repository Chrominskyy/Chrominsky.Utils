using Chrominsky.Utils.Models.Base;

namespace Chrominsky.Utils.Repositories.ObjectVersioning;

/// <summary>
/// Interface for ObjectVersioningRepository.
/// This interface defines the methods for interacting with the ObjectVersioning entity in the database.
/// </summary>
public interface IObjectVersioningRepository
{
    /// <summary>
    /// Retrieves an ObjectVersioning entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ObjectVersioning entity.</param>
    /// <returns>A Task that represents the asynchronous operation, returning the retrieved ObjectVersioning entity.</returns>
    Task<ObjectVersion?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Retrieves an ObjectVersioning entity by its associated object type, tenant, and id.
    /// </summary>
    /// <param name="objectType">The type of the object associated with the ObjectVersioning entity.</param>
    /// <param name="objectTenant">The tenant of the object associated with the ObjectVersioning entity.</param>
    /// <param name="objectId">The unique identifier of the object associated with the ObjectVersioning entity.</param>
    /// <returns>A Task that represents the asynchronous operation, returning the retrieved ObjectVersioning entity.</returns>
    Task<IEnumerable<ObjectVersion>> GetByObjectAsync(string objectType, Guid objectTenant, Guid objectId);

    /// <summary>
    /// Adds a new ObjectVersioning entity to the database.
    /// </summary>
    /// <param name="entity">The ObjectVersioning entity to be added.</param>
    /// <returns>A Task that represents the asynchronous operation, returning the unique identifier of the newly added entity.</returns>
    Task<Guid> AddAsync(ObjectVersion entity);

    /// <summary>
    /// Updates an existing ObjectVersioning entity in the database.
    /// </summary>
    /// <param name="entity">The updated ObjectVersioning entity.</param>
    /// <returns>A Task that represents the asynchronous operation, returning the updated ObjectVersioning entity.</returns>
    Task<ObjectVersion> UpdateAsync(ObjectVersion entity);

    /// <summary>
    /// Deletes an ObjectVersioning entity from the database by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ObjectVersioning entity to be deleted.</param>
    /// <returns>A Task that represents the asynchronous operation, returning a boolean indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(Guid id);
    
    
    /// <summary>
    /// Retrieves all ObjectVersioning entities from the database.
    /// </summary>
    /// <returns>
    /// A Task that represents the asynchronous operation, returning an IEnumerable of ObjectVersioning entities.
    /// </returns>
    Task<IEnumerable<ObjectVersion>> GetAllAsync();

    /// <summary>
    /// Retrieves all ObjectVersioning entities from the database that have the specified objectId.
    /// </summary>
    /// <param name="objectId">The unique identifier of the object associated with the ObjectVersioning entities.</param>
    /// <returns>
    /// A Task that represents the asynchronous operation, returning an IEnumerable of ObjectVersioning entities.
    /// If no ObjectVersioning entities are found with the specified objectId, an empty IEnumerable is returned.
    /// </returns>
    Task<IEnumerable<ObjectVersion>> GetByObjectIdAsync(Guid objectId);
}