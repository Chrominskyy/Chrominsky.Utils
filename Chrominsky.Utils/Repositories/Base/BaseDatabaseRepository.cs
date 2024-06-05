using Chrominsky.Utils.Enums;
using Chrominsky.Utils.Models.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chrominsky.Utils.Repositories.Base;

/// <summary>
/// Abstract class for database repository operations.
/// </summary>
/// <typeparam name="T">Type of entity to be handled by the repository.</typeparam>
public abstract class BaseDatabaseRepository<T> : IBaseDatabaseRepository<T> where T : class
{
    private readonly DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDatabaseRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to be used for database operations.</param>
    protected BaseDatabaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<T> GetByIdAsync<T>(Guid id) where T : class
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Guid> AddAsync<T>(T entity) where T : class, IBaseDatabaseEntity
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task<T?> UpdateAsync<T>(T? entity) where T : class, IBaseDatabaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var existingEntity = await _dbContext.Set<T>().FindAsync(entity.Id);
        if(existingEntity == null)
            throw new KeyNotFoundException($"Entity with id {entity.Id} not found.");
        
        existingEntity.UpdatedAt = DateTime.UtcNow;

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var newValue = property.GetValue(entity);
            switch (newValue)
            {
                case null:
                case DateTime time when time == DateTime.MinValue:
                case Guid guid when guid == Guid.Empty:
                    continue;
                default:
                    property.SetValue(existingEntity, newValue);
                    break;
            }
        }
        
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync<T>(Guid id) where T : class, IBaseDatabaseEntity
    {
        T entity = await _dbContext.FindAsync<T>(id);
        if (entity == null)
            return false;
        entity.Status = DatabaseEntityStatus.Deleted;
        var rowsAffected = await _dbContext.SaveChangesAsync();
        return rowsAffected > 0;
        
    }
}