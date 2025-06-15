using Moq;
using Xunit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MeritJournal.Domain.Entities;
using MeritJournal.Application.Interfaces;
using MeritJournal.Application.Features.JournalEntries.Queries;

namespace MeritJournal.UnitTests.Features.JournalEntries.Queries
{
    public class GetJournalEntriesQueryTests
    {
        [Fact]
        public async Task Handle_ReturnsJournalEntriesForSpecificUser()
        {
            // Arrange
            var userId = "test-user-id";
            var otherUserId = "other-user-id";
            var cancellationToken = CancellationToken.None;
            
            // Create mock journal entries
            var journalEntries = new List<JournalEntry>
            {
                new JournalEntry
                {
                    Id = 1,
                    Title = "Test Entry 1",
                    Content = "Test Content 1",
                    UserId = userId,
                    EntryDate = System.DateTime.UtcNow.AddDays(-1),
                    CreatedAt = System.DateTime.UtcNow.AddDays(-1),
                    JournalEntryTags = new List<JournalEntryTag>()
                },
                new JournalEntry
                {
                    Id = 2,
                    Title = "Test Entry 2",
                    Content = "Test Content 2",
                    UserId = userId,
                    EntryDate = System.DateTime.UtcNow,
                    CreatedAt = System.DateTime.UtcNow,
                    JournalEntryTags = new List<JournalEntryTag>()
                },
                new JournalEntry
                {
                    Id = 3,
                    Title = "Other User Entry",
                    Content = "Other User Content",
                    UserId = otherUserId,
                    EntryDate = System.DateTime.UtcNow,
                    CreatedAt = System.DateTime.UtcNow,
                    JournalEntryTags = new List<JournalEntryTag>()
                }
            };
            
            // Set up mock DbContext
            var mockJournalEntries = MockDbSet(journalEntries);
            
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.JournalEntries).Returns(mockJournalEntries.Object);
            
            // Create the query and handler
            var query = new GetJournalEntriesQuery(userId);
            var handler = new GetJournalEntriesQueryHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(query, cancellationToken);
              // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Should only get entries for specified user
            // JournalEntryDto doesn't contain UserId, so we can't check this directly
            // Instead, ensure we have the right number of results which implies filtering worked
        }
        
        [Fact]
        public async Task Handle_ReturnsEmptyListWhenNoEntriesExist()
        {
            // Arrange
            var userId = "user-with-no-entries";
            var cancellationToken = CancellationToken.None;
            
            // Set up empty journal entries collection
            var journalEntries = new List<JournalEntry>();
            var mockJournalEntries = MockDbSet(journalEntries);
            
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.JournalEntries).Returns(mockJournalEntries.Object);
            
            // Create the query and handler
            var query = new GetJournalEntriesQuery(userId);
            var handler = new GetJournalEntriesQueryHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(query, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        private static Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());
            
            return mockSet;
        }
    }
}
