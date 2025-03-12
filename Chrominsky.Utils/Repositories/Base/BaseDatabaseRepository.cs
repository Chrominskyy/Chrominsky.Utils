using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Chrominsky.Utils.Enums;
using Chrominsky.Utils.Models;
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Models.Base.Interfaces;
using Chrominsky.Utils.Repositories.ObjectVersioning;
using Microsoft.EntityFrameworkCore;

namespace Chrominsky.Utils.Repositories.Base;

/// <summary>
/// Abstract class for database repository operations.
/// </summary>
/// <typeparam name="T">Type of entity to be handled by the repository.</typeparam>
public abstract class BaseDatabaseRepository<T> : IBaseDatabaseRepository<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly IObjectVersioningRepository _objectVersioningRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDatabaseRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to be used for database operations.</param>
    protected BaseDatabaseRepository(DbContext dbContext, IObjectVersioningRepository objectVersioningRepository)
    {
        _dbContext = dbContext;
        _objectVersioningRepository = objectVersioningRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        return await _dbContext.Set<T>().ToListAsync();
    }
    
    /// <inheritdoc />
    public async Task <IEnumerable<T>> GetAllActiveAsync<T>() where T : class, IBaseDatabaseEntity
    {
        return await _dbContext.Set<T>().Where(x => x.Status == DatabaseEntityStatus.Active).ToListAsync();
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
        await _objectVersioningRepository.AddAsync(new ObjectVersion()
        {
            ObjectType = typeof(T).Name,
            ObjectTenant = entity.Id,
            ObjectId = entity.Id,
            UpdatedBy = entity.CreatedBy,
            AfterValue = JsonSerializer.Serialize(entity),
        });
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task<T?> UpdateAsync<T>(T? entity) where T : class, IBaseDatabaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var existingEntity = await _dbContext.Set<T>().FindAsync(entity.Id);
        if(existingEntity == null)
            throw new KeyNotFoundException($"Entity with id {entity.Id} not found.");

        var beforeValue = JsonSerializer.Serialize(existingEntity);
        entity.UpdatedAt = DateTime.UtcNow;

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var newValue = property.GetValue(entity);
            switch (newValue)
            {
                case null:
                case DateTime time when time == DateTime.MinValue:
                case Guid guid when guid == Guid.Empty:
                case IEnumerable enumerable when !IsEnumerablePopulated(enumerable):
                    continue;
                default:
                    property.SetValue(existingEntity, newValue);
                    break;
            }
        }
        
        var afterValue = JsonSerializer.Serialize(existingEntity);
        await _dbContext.SaveChangesAsync();

        await _objectVersioningRepository.AddAsync(new ObjectVersion()
        {
            ObjectType = typeof(T).Name,
            ObjectTenant = existingEntity.Id,
            ObjectId = existingEntity.Id,
            UpdatedBy = existingEntity.UpdatedBy,
            AfterValue = afterValue,
            BeforeValue = beforeValue,
        });
        
        return existingEntity;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync<T>(Guid id) where T : class, IBaseDatabaseEntity
    {
        T entity = await _dbContext.FindAsync<T>(id);
        if (entity == null)
            return false;
        
        var existingEntityJson = JsonSerializer.Serialize(entity);
        
        entity.Status = DatabaseEntityStatus.Deleted;
        var rowsAffected = await _dbContext.SaveChangesAsync();

        await _objectVersioningRepository.AddAsync(new ObjectVersion()
        {
            ObjectType = typeof(T).Name,
            ObjectTenant = entity.Id,
            ObjectId = entity.Id,
            UpdatedBy = entity.CreatedBy,
            AfterValue = JsonSerializer.Serialize(entity),
            BeforeValue = existingEntityJson,
        });
        
        return rowsAffected > 0;
        
    }

    /// <inheritdoc />
    public async Task<List<T>> SearchAsync<T>(SearchParameterRequest request) where T : class, IBaseDatabaseEntity
    {
        var table = _dbContext.Set<T>().AsQueryable();
        var entityType = typeof(T);
        
        if(!request.IncludeNotActive)
            table = table.Where(x => x.Status == DatabaseEntityStatus.Active);

        foreach (var searchParameter in request.SearchParameters)
        {
            var propertyInfo = entityType.GetProperty(searchParameter.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo == null)
                continue;

            var propertyType = propertyInfo.PropertyType;
            
            table = searchParameter.Operator switch
            {
                SearchOperator.Contains when propertyType == typeof(string) =>
                    table.Where(x => EF.Property<string>(x, searchParameter.Key).Contains(searchParameter.Value)),

                SearchOperator.Equals => 
                    propertyType switch
                    {
                        Type t when t == typeof(bool) =>
                            bool.TryParse(searchParameter.Value, out bool boolValue) 
                                ? table.Where(x => EF.Property<bool>(x, searchParameter.Key) == boolValue)
                                : table,

                        Type t when t == typeof(int) =>
                            int.TryParse(searchParameter.Value, out int intValue)
                                ? table.Where(x => EF.Property<int>(x, searchParameter.Key) == intValue)
                                : table,

                        Type t when t == typeof(DateTime) =>
                            DateTime.TryParse(searchParameter.Value, out DateTime dateValue)
                                ? table.Where(x => EF.Property<DateTime>(x, searchParameter.Key) == dateValue)
                                : table,

                        _ => table.Where(x => EF.Property<string>(x, searchParameter.Key).Equals(searchParameter.Value))
                    },

                _ => searchParameter.Operator switch
                {
                    SearchOperator.GreaterThan =>
                        ApplyGreaterThanFilter(table, searchParameter.Key, searchParameter.Value),

                    SearchOperator.LessThan =>
                        ApplyLessThanFilter(table, searchParameter.Key, searchParameter.Value),

                    SearchOperator.GreaterOrEqualThan =>
                        ApplyGreaterOrEqualThanFilter(table, searchParameter.Key, searchParameter.Value),

                    SearchOperator.LessOrEqualThan =>
                        ApplyLessOrEqualThanFilter(table, searchParameter.Key, searchParameter.Value),

                    _ => table
                }
            };
        }

        // Execute the final query and return the result as a Task
        var itemsToSkip = (request.Page - 1) * request.PageSize;

        // Apply pagination
        var result = await table.Skip(itemsToSkip)
            .Take(request.PageSize)
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<IEnumerable<T>>> GetPaginatedAsync<T>(int page, int pageSize)
        where T : class, IBaseDatabaseEntity
    {
        var table = _dbContext.Set<T>().AsQueryable();
        
        var itemsToSkip = (page - 1) * pageSize;

        var result = await table.Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
        var count = table.Count();

        return new PaginatedResponse<IEnumerable<T>>()
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = count,
            Data = result
        };
    }

    /// <param name="tableName"></param>
    /// <inheritdoc />
    public TableColumns? GetTableColumnsAsync<T>(string? tableName = null) where T : class, IBaseDatabaseEntity
    {
        IQueryable<TableColumns?> table = _dbContext.Set<TableColumns>().AsQueryable();
        var entityType = tableName?? typeof(T).Name;
        
        return table.FirstOrDefault(c=> c.TableName == entityType) ?? null;
    }

    /// <summary>
    /// Applies a less than filter to the given query based on the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The query to apply the filter to.</param>
    /// <param name="key">The property key to filter on.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A new query with the applied less than filter.</returns>
    /// <exception cref="Exception">Thrown when the input value cannot be parsed to an integer or a DateTime.</exception>
    private IQueryable<T> ApplyLessThanFilter<T>(IQueryable<T> query, string key, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, key);

        if (int.TryParse(value, out int intValue))
        {
            var intValueConstant = Expression.Constant(intValue, typeof(int));
            var tryParseMethod =
                typeof(int).GetMethod("TryParse", new[] { typeof(string), typeof(int).MakeByRefType() });

            var outVar = Expression.Variable(typeof(int), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.LessThan(outVar, intValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        else if (DateTime.TryParse(value, out DateTime dateValue))
        {
            var dateValueConstant = Expression.Constant(dateValue, typeof(DateTime));
            var tryParseMethod =
                typeof(DateTime).GetMethod("TryParse", new[] { typeof(string), typeof(DateTime).MakeByRefType() });

            var outVar = Expression.Variable(typeof(DateTime), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.LessThan(outVar, dateValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        else
        {
            // Throw an exception if the input value cannot be parsed to an integer or a DateTime
            throw new Exception($"{nameof(ApplyLessThanFilter)} - Invalid input" );
        }
    }

    /// <summary>
    /// Applies a greater than filter to the given query based on the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The query to apply the filter to.</param>
    /// <param name="key">The property key to filter on.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A new query with the applied greater than filter.</returns>
    /// <exception cref="Exception">Thrown when the input value cannot be parsed to an integer or a DateTime.</exception>
    public IQueryable<T> ApplyGreaterThanFilter<T>(IQueryable<T> query, string key, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, key);

        if (int.TryParse(value, out int intValue))
        {
            var intValueConstant = Expression.Constant(intValue, typeof(int));
            var tryParseMethod =
                typeof(int).GetMethod("TryParse", new[] { typeof(string), typeof(int).MakeByRefType() });

            var outVar = Expression.Variable(typeof(int), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.GreaterThan(outVar, intValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        else if (DateTime.TryParse(value, out DateTime dateValue))
        {
            var dateValueConstant = Expression.Constant(dateValue, typeof(DateTime));
            var tryParseMethod =
                typeof(DateTime).GetMethod("TryParse", new[] { typeof(string), typeof(DateTime).MakeByRefType() });

            var outVar = Expression.Variable(typeof(DateTime), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.GreaterThan(outVar, dateValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        else
        {
            throw new Exception($"{nameof(ApplyGreaterThanFilter)} - Invalid input");
        }
    }
    
    /// <summary>
    /// Applies a less than or equal to filter to the given query based on the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The query to apply the filter to.</param>
    /// <param name="key">The property key to filter on.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A new query with the applied less than or equal to filter.</returns>
    /// <exception cref="Exception">Thrown when the input value cannot be parsed to an integer or a DateTime.</exception>
    private IQueryable<T> ApplyLessOrEqualThanFilter<T>(IQueryable<T> query, string key, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, key);

        if (int.TryParse(value, out int intValue))
        {
            var intValueConstant = Expression.Constant(intValue, typeof(int));
            var tryParseMethod =
                typeof(int).GetMethod("TryParse", new[] { typeof(string), typeof(int).MakeByRefType() });

            var outVar = Expression.Variable(typeof(int), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.LessThanOrEqual(outVar, intValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        else if (DateTime.TryParse(value, out DateTime dateValue))
        {
            var dateValueConstant = Expression.Constant(dateValue, typeof(DateTime));
            var tryParseMethod =
                typeof(DateTime).GetMethod("TryParse", new[] { typeof(string), typeof(DateTime).MakeByRefType() });

            var outVar = Expression.Variable(typeof(DateTime), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.LessThanOrEqual(outVar, dateValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        else
        {
            // Throw an exception if the input value cannot be parsed to an integer or a DateTime
            throw new Exception($"{nameof(ApplyLessOrEqualThanFilter)} - Invalid input" );
        }
    }

    /// <summary>
    /// Applies a greater than or equal to filter to the given query based on the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The query to apply the filter to.</param>
    /// <param name="key">The property key to filter on.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A new query with the applied greater than or equal to filter.</returns>
    /// <exception cref="Exception">Thrown when the input value cannot be parsed to an integer or a DateTime.</exception>
    private IQueryable<T> ApplyGreaterOrEqualThanFilter<T>(IQueryable<T> query, string key, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, key);

        if (int.TryParse(value, out int intValue))
        {
            var intValueConstant = Expression.Constant(intValue, typeof(int));
            var tryParseMethod =
                typeof(int).GetMethod("TryParse", new[] { typeof(string), typeof(int).MakeByRefType() });

            var outVar = Expression.Variable(typeof(int), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.GreaterThanOrEqual(outVar, intValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
        else if (DateTime.TryParse(value, out DateTime dateValue))
        {
            var dateValueConstant = Expression.Constant(dateValue, typeof(DateTime));
            var tryParseMethod =
                typeof(DateTime).GetMethod("TryParse", new[] { typeof(string), typeof(DateTime).MakeByRefType() });

            var outVar = Expression.Variable(typeof(DateTime), "outVar");
            var body = Expression.Block(
                new[] { outVar },
                Expression.AndAlso(
                    Expression.Call(tryParseMethod, property, outVar),
                    Expression.GreaterThanOrEqual(outVar, dateValueConstant)
                )
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }

        // Throw an exception if the input value cannot be parsed to an integer or a DateTime
        throw new Exception($"{nameof(ApplyGreaterThanFilter)} - Invalid input" );
    }
    
    private bool IsEnumerablePopulated(IEnumerable enumerable)
    {
        // Handle null
        if (enumerable == null)
            return false;

        // Check if it's a generic IEnumerable<T>
        var enumerableType = enumerable.GetType();
        if (enumerableType.IsGenericType &&
            enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            // Use LINQ Count() method to check population
            var countMethod = enumerableType.GetMethod("Count", Type.EmptyTypes);
            if (countMethod != null)
            {
                return (int)countMethod.Invoke(enumerable, null) > 0;
            }
        }

        // Fallback to manual enumeration
        var enumerator = enumerable.GetEnumerator();
        try
        {
            return enumerator.MoveNext();
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }
}