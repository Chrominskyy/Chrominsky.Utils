namespace Chrominsky.Utils.Mappers.Base;

/// <summary>
/// Defines a base interface for mapping between two types of objects.
/// </summary>
/// <typeparam name="T">The type of the source object.</typeparam>
/// <typeparam name="T2">The type of the destination object.</typeparam>
public interface IBaseMapper<T, T2> where T: class where T2: class
{
    /// <summary>
    /// Maps a source object to a destination object asynchronously.
    /// </summary>
    /// <param name="entity">The source object.</param>
    /// <returns>A task that represents the asynchronous operation, returning the destination object.</returns>
    T2 ToDto(T entity);
    
    /// <summary>
    /// Maps a destination object to a source object asynchronously.
    /// </summary>
    /// <param name="dto">The destination object.</param>
    /// <returns>A task that represents the asynchronous operation, returning the source object.</returns>
    T ToEntity(T2 dto);
}