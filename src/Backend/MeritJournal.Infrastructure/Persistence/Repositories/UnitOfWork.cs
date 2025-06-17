using MeritJournal.Application.Interfaces;
using MeritJournal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MeritJournal.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of the Unit of Work pattern, coordinating the work of multiple repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    // Repositories
    private IRepository<JournalEntry>? _journalEntryRepository;
    private IRepository<JournalImage>? _journalImageRepository;
    private IRepository<Tag>? _tagRepository;
    private IRepository<JournalEntryTag>? _journalEntryTagRepository;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class.
    /// </summary>
    /// <param name="dbContext">The database context</param>
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Gets the repository for JournalEntry entities.
    /// </summary>
    public IRepository<JournalEntry> JournalEntries => 
        _journalEntryRepository ??= new Repository<JournalEntry>(_dbContext);

    /// <summary>
    /// Gets the repository for JournalImage entities.
    /// </summary>
    public IRepository<JournalImage> JournalImages => 
        _journalImageRepository ??= new Repository<JournalImage>(_dbContext);

    /// <summary>
    /// Gets the repository for Tag entities.
    /// </summary>
    public IRepository<Tag> Tags => 
        _tagRepository ??= new Repository<Tag>(_dbContext);

    /// <summary>
    /// Gets the repository for JournalEntryTag entities.
    /// </summary>
    public IRepository<JournalEntryTag> JournalEntryTags => 
        _journalEntryTagRepository ??= new Repository<JournalEntryTag>(_dbContext);

    /// <summary>
    /// Saves all changes made through this unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The number of objects written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress");
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// Disposes the current transaction and database context.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the current transaction and database context.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _currentTransaction?.Dispose();
            _dbContext.Dispose();
        }
        _disposed = true;
    }
}
