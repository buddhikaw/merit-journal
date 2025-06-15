using MeritJournal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeritJournal.Application.Interfaces;

/// <summary>
/// Interface for the application's database context.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// DbSet for JournalEntry entities.
    /// </summary>
    DbSet<JournalEntry> JournalEntries { get; }
    
    /// <summary>
    /// DbSet for JournalImage entities.
    /// </summary>
    DbSet<JournalImage> JournalImages { get; }
    
    /// <summary>
    /// DbSet for Tag entities.
    /// </summary>
    DbSet<Tag> Tags { get; }
    
    /// <summary>
    /// DbSet for JournalEntryTag entities.
    /// </summary>
    DbSet<JournalEntryTag> JournalEntryTags { get; }
    
    /// <summary>
    /// Saves all changes made to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of objects written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
