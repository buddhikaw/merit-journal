using MeritJournal.Domain.Entities;

namespace MeritJournal.Application.Interfaces;

/// <summary>
/// Represents a unit of work that coordinates repository operations
/// and provides transaction management.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the repository for JournalEntry entities.
    /// </summary>
    IRepository<JournalEntry> JournalEntries { get; }
    
    /// <summary>
    /// Gets the repository for JournalImage entities.
    /// </summary>
    IRepository<JournalImage> JournalImages { get; }
    
    /// <summary>
    /// Gets the repository for Tag entities.
    /// </summary>
    IRepository<Tag> Tags { get; }
    
    /// <summary>
    /// Gets the repository for JournalEntryTag entities.
    /// </summary>
    IRepository<JournalEntryTag> JournalEntryTags { get; }
    
    /// <summary>
    /// Saves all changes made through this unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The number of objects written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
