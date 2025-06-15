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
    public class GetJournalEntryByIdQueryTests
    {
        [Fact]
        public async Task Handle_ReturnsJournalEntryWhenFound()
        {
            // Arrange
            var userId = "test-user-id";
            var entryId = 1;
            var cancellationToken = CancellationToken.None;
            
            var tag = new Tag { Id = 1, Name = "TestTag", UserId = userId };
            
            var journalEntry = new JournalEntry
            {
                Id = entryId,
                Title = "Test Entry",
                Content = "Test Content",
                UserId = userId,
                EntryDate = System.DateTime.UtcNow,
                CreatedAt = System.DateTime.UtcNow,
                JournalEntryTags = new List<JournalEntryTag>
                {
                    new JournalEntryTag { JournalEntryId = entryId, TagId = tag.Id, Tag = tag }
                },
                Images = new List<JournalImage>()
            };
            
            var journalEntries = new List<JournalEntry> { journalEntry };
            var mockJournalEntries = MockDbSet(journalEntries);
            
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.JournalEntries).Returns(mockJournalEntries.Object);
            
            var query = new GetJournalEntryByIdQuery(entryId, userId);
            var handler = new GetJournalEntryByIdQueryHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(query, cancellationToken);
              // Assert            
            Assert.NotNull(result);
            Assert.Equal(entryId, result.Id);
            Assert.Equal("Test Entry", result.Title);
            Assert.Equal("Test Content", result.Content);
            Assert.NotNull(result.Tags);
            if (result.Tags != null)
            {
                Assert.Contains("TestTag", result.Tags);
            }
        }
        
        [Fact]
        public async Task Handle_ReturnsNullWhenEntryNotFound()
        {
            // Arrange
            var userId = "test-user-id";
            var nonExistentEntryId = 999;
            var cancellationToken = CancellationToken.None;
            
            var journalEntries = new List<JournalEntry>
            {
                new JournalEntry
                {
                    Id = 1,
                    Title = "Test Entry",
                    Content = "Test Content",
                    UserId = userId,
                    EntryDate = System.DateTime.UtcNow,
                    CreatedAt = System.DateTime.UtcNow
                }
            };
            
            var mockJournalEntries = MockDbSet(journalEntries);
            
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.JournalEntries).Returns(mockJournalEntries.Object);
            
            var query = new GetJournalEntryByIdQuery(nonExistentEntryId, userId);
            var handler = new GetJournalEntryByIdQueryHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(query, cancellationToken);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Handle_ReturnsNullWhenEntryBelongsToAnotherUser()
        {
            // Arrange
            var userId = "test-user-id";
            var otherUserId = "other-user-id";
            var entryId = 1;
            var cancellationToken = CancellationToken.None;
            
            var journalEntries = new List<JournalEntry>
            {
                new JournalEntry
                {
                    Id = entryId,
                    Title = "Test Entry",
                    Content = "Test Content",
                    UserId = otherUserId, // Entry belongs to a different user
                    EntryDate = System.DateTime.UtcNow,
                    CreatedAt = System.DateTime.UtcNow
                }
            };
            
            var mockJournalEntries = MockDbSet(journalEntries);
            
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.JournalEntries).Returns(mockJournalEntries.Object);
            
            var query = new GetJournalEntryByIdQuery(entryId, userId);
            var handler = new GetJournalEntryByIdQueryHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(query, cancellationToken);
            
            // Assert
            Assert.Null(result);
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
