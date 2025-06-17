using System.Linq.Expressions;

namespace MeritJournal.Application.Interfaces;

/// <summary>
/// Generic repository interface that defines operations to be performed on entities.
/// </summary>
/// <typeparam name="T">The entity type this repository operates on</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets all entities from the repository.
    /// </summary>
    /// <returns>A queryable collection of all entities.</returns>
    IQueryable<T> GetAll();
    
    /// <summary>
    /// Gets all entities from the repository as a list asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A list of all entities.</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>A queryable collection of entities that match the condition.</returns>
    IQueryable<T> Find(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Gets entities as a list that match the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A list of entities that match the condition.</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The entity found, or null if not found.</returns>
    Task<T?> GetByIdAsync(object id);
    
    /// <summary>
    /// Gets the first entity that matches the specified predicate, or null if no entity matches.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>The first entity that matches the condition, or null if no entity matches.</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Gets the single entity that matches the specified predicate, or null if no entity matches.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The single entity that matches the condition, or null if no entity matches.</returns>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a queryable with included related entities.
    /// </summary>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A queryable collection of entities with included related entities.</returns>
    IQueryable<T> Query(params Expression<Func<T, object>>[] includes);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);
    
    /// <summary>
    /// Adds a new entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The added entity.</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a collection of entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<T> entities);
    
    /// <summary>
    /// Adds a collection of entities to the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The added entities.</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);
    
    /// <summary>
    /// Updates an existing entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);
    
    /// <summary>
    /// Removes an entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a collection of entities from the repository.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<T> entities);
    
    /// <summary>
    /// Removes a collection of entities from the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any entity satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>True if any entity satisfies the condition; otherwise, false.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Counts entities that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <returns>The number of entities that satisfy the condition.</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}
