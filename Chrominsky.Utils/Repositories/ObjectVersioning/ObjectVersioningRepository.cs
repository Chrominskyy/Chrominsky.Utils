using Chrominsky.Utils.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Chrominsky.Utils.Repositories.ObjectVersioning;

/// <inheritdoc />
public class ObjectVersioningRepository : IObjectVersioningRepository
{
    private readonly DbContext _dbContext;
    
    public ObjectVersioningRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<ObjectVersion?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<ObjectVersion>().FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ObjectVersion>> GetByObjectAsync(string objectType, Guid objectTenant, Guid objectId)
    {
        if (_dbContext.Set<ObjectVersion>() != null)
            return await _dbContext.Set<ObjectVersion>()
                .Where(
                    x => x.ObjectType == objectType
                         && x.ObjectTenant == objectTenant
                         && x.ObjectId == objectId
                )
                .OrderByDescending(x => x.UpdatedOn)
                .ToListAsync();
        return new List<ObjectVersion>();
    }

    /// <inheritdoc />
    public async Task<Guid> AddAsync(ObjectVersion entity)
    {
        entity.Id = Guid.NewGuid();
        entity.UpdatedOn = DateTime.UtcNow;
        await _dbContext.Set<ObjectVersion>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task<ObjectVersion> UpdateAsync(ObjectVersion entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ObjectVersion>> GetAllAsync()
    {
        return await _dbContext.Set<ObjectVersion>()
            .OrderByDescending(x => x.UpdatedOn)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ObjectVersion>> GetByObjectIdAsync(Guid objectId)
    {
        return await _dbContext.Set<ObjectVersion>()
            .Where(x => x.ObjectId == objectId)
            .OrderByDescending(x => x.UpdatedOn)
            .ToListAsync();
    }
}