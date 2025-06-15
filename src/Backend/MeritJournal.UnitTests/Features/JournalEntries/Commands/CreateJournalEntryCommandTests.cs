using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MeritJournal.Domain.Entities;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using MeritJournal.Application.Features.JournalEntries.Commands;

namespace MeritJournal.UnitTests.Features.JournalEntries.Commands
{
    public class CreateJournalEntryCommandTests
    {
        [Fact]
        public async Task Handle_CreatesJournalEntry_WithBasicProperties()
        {
            // Arrange
            var userId = "test-user-id";
            var title = "Test Journal Entry";
            var content = "Test Content";
            var entryDate = DateTime.UtcNow.Date;
            var cancellationToken = CancellationToken.None;
            
            var command = new CreateJournalEntryCommand
            {
                Title = title,
                Content = content,
                EntryDate = entryDate,
                UserId = userId,
                Tags = null,
                Images = null
            };
            
            var (mockContext, capturedJournalEntry) = SetupMockContext();
            var handler = new CreateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(title, result.Title);
            Assert.Equal(content, result.Content);
            Assert.Equal(entryDate, result.EntryDate);
            
            // Verify the journal entry was saved to the database
            mockContext.Verify(c => c.JournalEntries.Add(It.IsAny<JournalEntry>()), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.Once);
            
            // Verify properties were set correctly
            Assert.NotNull(capturedJournalEntry);
            Assert.Equal(title, capturedJournalEntry.Title);
            Assert.Equal(content, capturedJournalEntry.Content);
            Assert.Equal(entryDate, capturedJournalEntry.EntryDate);
            Assert.Equal(userId, capturedJournalEntry.UserId);
        }
        
        [Fact]
        public async Task Handle_CreatesJournalEntry_WithTags()
        {
            // Arrange
            var userId = "test-user-id";
            var title = "Test Journal Entry";
            var content = "Test Content";
            var entryDate = DateTime.UtcNow.Date;
            var tags = new List<string> { "tag1", "tag2", "tag3" };
            var cancellationToken = CancellationToken.None;
            
            var command = new CreateJournalEntryCommand
            {
                Title = title,
                Content = content,
                EntryDate = entryDate,
                UserId = userId,
                Tags = tags,
                Images = null
            };
            
            // Create existing tag for tag1
            var existingTag = new Tag { Id = 1, Name = "tag1", UserId = userId };
            
            var (mockContext, capturedJournalEntry) = SetupMockContext(
                existingTags: new List<Tag> { existingTag });
                
            // Setup Tags DbSet to return existingTag when queried
            var mockTagsDbSet = new Mock<DbSet<Tag>>();
            var queryableTags = new List<Tag> { existingTag }.AsQueryable();
            
            mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.Provider).Returns(queryableTags.Provider);
            mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.Expression).Returns(queryableTags.Expression);
            mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.ElementType).Returns(queryableTags.ElementType);
            mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.GetEnumerator()).Returns(() => queryableTags.GetEnumerator());
            
            mockContext.Setup(c => c.Tags).Returns(mockTagsDbSet.Object);
            
            // Capture added tags
            var capturedTags = new List<Tag>();
            mockTagsDbSet.Setup(m => m.Add(It.IsAny<Tag>()))
                .Callback<Tag>(tag => capturedTags.Add(tag))
                .Returns((Tag tag) => tag);
            
            var handler = new CreateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(title, result.Title);
            
            // Verify tags were processed and saved
            mockContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.AtLeast(2));
            
            // Verify new tags were created (tag2, tag3)
            Assert.Equal(2, capturedTags.Count);
            Assert.Contains(capturedTags, t => t.Name == "tag2" && t.UserId == userId);
            Assert.Contains(capturedTags, t => t.Name == "tag3" && t.UserId == userId);
            
            // Verify result has the tags
            Assert.NotNull(result.Tags);
            Assert.Equal(3, result.Tags.Count);
            Assert.Contains("tag1", result.Tags);
            Assert.Contains("tag2", result.Tags);
            Assert.Contains("tag3", result.Tags);
        }
        
        [Fact]
        public async Task Handle_CreatesJournalEntry_WithoutEmptyOrDuplicateTags()
        {
            // Arrange
            var userId = "test-user-id";
            var tags = new List<string> { "tag1", "", "tag1", " tag2 ", string.Empty, "  ", "tag2" };
            var cancellationToken = CancellationToken.None;
            
            var command = new CreateJournalEntryCommand
            {
                Title = "Test Title",
                Content = "Test Content",
                EntryDate = DateTime.UtcNow.Date,
                UserId = userId,
                Tags = tags,
                Images = null
            };
            
            var (mockContext, capturedJournalEntry) = SetupMockContext();
            var handler = new CreateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Tags);
            
            // Should only have two unique non-empty tags: tag1 and tag2 (with trimmed spaces)
            Assert.Equal(2, result.Tags.Count);
            Assert.Contains("tag1", result.Tags);
            Assert.Contains("tag2", result.Tags);
        }

        // Return both the mock context and the captured journal entry
        private (Mock<IApplicationDbContext>, JournalEntry) SetupMockContext(List<Tag>? existingTags = null)
        {
            var mockContext = new Mock<IApplicationDbContext>();
            
            // Create a journal entry to capture
            var capturedEntry = new JournalEntry();
            
            // Mock JournalEntries DbSet
            var mockJournalEntriesDbSet = new Mock<DbSet<JournalEntry>>();
            mockJournalEntriesDbSet.Setup(m => m.Add(It.IsAny<JournalEntry>()))
                .Callback<JournalEntry>(entry => { 
                    // Set an ID for the entry as EF would
                    entry.Id = 1;
                    // Copy all properties to our captured entry
                    capturedEntry.Id = entry.Id;
                    capturedEntry.Title = entry.Title;
                    capturedEntry.Content = entry.Content;
                    capturedEntry.EntryDate = entry.EntryDate;
                    capturedEntry.UserId = entry.UserId;
                    capturedEntry.CreatedAt = entry.CreatedAt;
                    capturedEntry.JournalEntryTags = entry.JournalEntryTags;
                    capturedEntry.Images = entry.Images;
                })
                .Returns((JournalEntry entry) => entry);
                
            mockContext.Setup(c => c.JournalEntries).Returns(mockJournalEntriesDbSet.Object);
            
            // Mock JournalEntryTags DbSet
            var mockJournalEntryTagsDbSet = new Mock<DbSet<JournalEntryTag>>();
            mockContext.Setup(c => c.JournalEntryTags).Returns(mockJournalEntryTagsDbSet.Object);
            
            // Mock Tags DbSet if not provided
            if (existingTags == null)
            {
                var mockTagsDbSet = new Mock<DbSet<Tag>>();
                var emptyTags = new List<Tag>().AsQueryable();
                
                mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.Provider).Returns(emptyTags.Provider);
                mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.Expression).Returns(emptyTags.Expression);
                mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.ElementType).Returns(emptyTags.ElementType);
                mockTagsDbSet.As<IQueryable<Tag>>().Setup(m => m.GetEnumerator()).Returns(() => emptyTags.GetEnumerator());
                
                mockTagsDbSet.Setup(m => m.Add(It.IsAny<Tag>()))
                    .Callback<Tag>(tag => { tag.Id = existingTags?.Count + 1 ?? 1; })
                    .Returns((Tag tag) => tag);
                    
                mockContext.Setup(c => c.Tags).Returns(mockTagsDbSet.Object);
            }
            
            // Setup SaveChangesAsync
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
                
            return (mockContext, capturedEntry);
        }
    }
}
