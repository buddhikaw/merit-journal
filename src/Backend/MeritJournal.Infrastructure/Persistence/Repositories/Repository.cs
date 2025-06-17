using System.Linq.Expressions;
using MeritJournal.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeritJournal.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic repository implementation that provides CRUD operations for entities.
/// </summary>
/// <typeparam name="T">The entity type this repository operates on</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    /// <param name="dbContext">The database context</param>
    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = dbContext.Set<T>();
    }

    /// <summary>
    /// Gets all entities from the repository.
    /// </summary>
    /// <returns>A queryable collection of all entities.</returns>
    public IQueryable<T> GetAll()
    {
        return _dbSet;
    }
    
    /// <summary>
    /// Gets all entities from the repository as a list asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A list of all entities.</returns>
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>A queryable collection of entities that match the condition.</returns>
    public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }
    
    /// <summary>
    /// Gets entities as a list that match the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A list of entities that match the condition.</returns>
    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a single entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The entity found, or null if not found.</returns>
    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Gets the first entity that matches the specified predicate, or null if no entity matches.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>The first entity that matches the condition, or null if no entity matches.</returns>
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
    
    /// <summary>
    /// Gets the single entity that matches the specified predicate, or null if no entity matches.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The single entity that matches the condition, or null if no entity matches.</returns>
    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }
    
    /// <summary>
    /// Gets a queryable with included related entities.
    /// </summary>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A queryable collection of entities with included related entities.</returns>
    public IQueryable<T> Query(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return query;
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }
    
    /// <summary>
    /// Adds a new entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The added entity.</returns>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Adds a collection of entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }
    
    /// <summary>
    /// Adds a collection of entities to the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The added entities.</returns>
    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToList();
        await _dbSet.AddRangeAsync(entitiesList, cancellationToken);
        return entitiesList;
    }

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    
    /// <summary>
    /// Updates an existing entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
    
    /// <summary>
    /// Removes an entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a collection of entities from the repository.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    
    /// <summary>
    /// Removes a collection of entities from the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if any entity satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>True if any entity satisfies the condition; otherwise, false.</returns>
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    /// <summary>
    /// Counts entities that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>The number of entities that satisfy the condition.</returns>
    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }
}
