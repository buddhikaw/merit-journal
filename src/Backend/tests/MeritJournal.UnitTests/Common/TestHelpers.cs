using System.Linq.Expressions;
using MeritJournal.Application.Interfaces;
using Moq;

namespace MeritJournal.UnitTests.Common;

/// <summary>
/// Helper methods for setting up test mocks.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Creates a mock repository with common setups.
    /// </summary>
    /// <typeparam name="T">The entity type for the repository.</typeparam>
    /// <param name="data">The data to be returned by the repository.</param>
    /// <returns>A configured mock repository.</returns>
    public static Mock<IRepository<T>> MockRepository<T>(IEnumerable<T> data) where T : class
    {
        var repository = new Mock<IRepository<T>>();
        
        var queryableData = data.AsQueryable();
        
        // Setup GetAll
        repository.Setup(r => r.GetAll())
            .Returns(queryableData);
            
        // Setup GetAllAsync
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken token) => data.ToList() as IReadOnlyList<T>);
            
        // Setup Find
        repository.Setup(r => r.Find(It.IsAny<Expression<Func<T, bool>>>()))
            .Returns((Expression<Func<T, bool>> predicate) => 
                queryableData.Where(predicate));
                
        // Setup FindAsync
        repository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>> predicate, CancellationToken token) => 
                queryableData.Where(predicate.Compile()).ToList() as IReadOnlyList<T>);
                
        // Setup FirstOrDefaultAsync
        repository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<T, bool>>>()))
            .ReturnsAsync((Expression<Func<T, bool>> predicate) => 
                queryableData.FirstOrDefault(predicate.Compile()));
                
        // Setup SingleOrDefaultAsync
        repository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>> predicate, CancellationToken token) => 
                queryableData.SingleOrDefault(predicate.Compile()));
                
        // Setup AnyAsync
        repository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<T, bool>>>()))
            .ReturnsAsync((Expression<Func<T, bool>> predicate) => 
                queryableData.Any(predicate.Compile()));
                
        // Setup CountAsync
        repository.Setup(r => r.CountAsync(It.IsAny<Expression<Func<T, bool>>>()))
            .ReturnsAsync((Expression<Func<T, bool>> predicate) => 
                queryableData.Count(predicate.Compile()));
                
        // Add, Update, Remove are void methods, so no need to set up return values
        
        return repository;
    }

    /// <summary>
    /// Creates a mock unit of work with configured repositories.
    /// </summary>
    /// <param name="journalEntries">Mock journal entries repository</param>
    /// <param name="journalImages">Mock journal images repository</param>
    /// <param name="tags">Mock tags repository</param>
    /// <param name="journalEntryTags">Mock journal entry tags repository</param>
    /// <returns>A configured mock unit of work.</returns>
    public static Mock<IUnitOfWork> MockUnitOfWork(
        Mock<IRepository<MeritJournal.Domain.Entities.JournalEntry>>? journalEntries = null,
        Mock<IRepository<MeritJournal.Domain.Entities.JournalImage>>? journalImages = null,
        Mock<IRepository<MeritJournal.Domain.Entities.Tag>>? tags = null,
        Mock<IRepository<MeritJournal.Domain.Entities.JournalEntryTag>>? journalEntryTags = null)
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        
        if (journalEntries != null)
        {
            unitOfWork.Setup(u => u.JournalEntries).Returns(journalEntries.Object);
        }
        
        if (journalImages != null)
        {
            unitOfWork.Setup(u => u.JournalImages).Returns(journalImages.Object);
        }
        
        if (tags != null)
        {
            unitOfWork.Setup(u => u.Tags).Returns(tags.Object);
        }
        
        if (journalEntryTags != null)
        {
            unitOfWork.Setup(u => u.JournalEntryTags).Returns(journalEntryTags.Object);
        }
        
        // Setup SaveChangesAsync to return 1 (indicating one row was affected)
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
            
        return unitOfWork;
    }
}
