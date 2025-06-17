// This interface is being deprecated in favor of using IRepository<T> and IUnitOfWork.
// It's kept here temporarily for backward compatibility while refactoring.
// Once refactoring is complete, this interface can be removed.

using MeritJournal.Domain.Entities;

namespace MeritJournal.Application.Interfaces;

/// <summary>
/// @Deprecated: Use IUnitOfWork and IRepository<T> interfaces instead.
/// Interface for the application's database context.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// @Deprecated: Use IUnitOfWork.JournalEntries repository instead.
    /// </summary>
    IQueryable<JournalEntry> JournalEntries { get; }
    
    /// <summary>
    /// @Deprecated: Use IUnitOfWork.JournalImages repository instead.
    /// </summary>
    IQueryable<JournalImage> JournalImages { get; }
    
    /// <summary>
    /// @Deprecated: Use IUnitOfWork.Tags repository instead.
    /// </summary>
    IQueryable<Tag> Tags { get; }
    
    /// <summary>
    /// @Deprecated: Use IUnitOfWork.JournalEntryTags repository instead.
    /// </summary>
    IQueryable<JournalEntryTag> JournalEntryTags { get; }
    
    /// <summary>
    /// @Deprecated: Use IUnitOfWork.SaveChangesAsync() instead.
    /// Saves all changes made to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of objects written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
