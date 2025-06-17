using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeritJournal.Application.Features.JournalEntries.Queries;
using MeritJournal.Domain.Entities;
using MeritJournal.UnitTests.Common;
using Xunit;

namespace MeritJournal.UnitTests.Features.JournalEntries.Queries;

public class GetJournalEntryByIdQueryTests
{
    [Fact]
    public async Task Handle_ExistingJournalEntry_ReturnsJournalEntryDto()
    {
        // Arrange
        var userId = "user123";
        var journalEntryId = 1;
        
        // Create test data
        var journalEntries = new List<JournalEntry>
        {
            new JournalEntry
            {
                Id = journalEntryId,
                Title = "Test Journal Entry",
                Content = "<p>Test content</p>",
                EntryDate = DateTime.UtcNow.Date,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = null,
                UserId = userId
            }
        };
        
        var tags = new List<Tag>
        {
            new Tag { Id = 1, Name = "tag1", UserId = userId },
            new Tag { Id = 2, Name = "tag2", UserId = userId }
        };
        
        var journalEntryTags = new List<JournalEntryTag>
        {
            new JournalEntryTag { JournalEntryId = journalEntryId, TagId = 1 },
            new JournalEntryTag { JournalEntryId = journalEntryId, TagId = 2 }
        };
        
        var images = new List<JournalImage>
        {
            new JournalImage
            {
                Id = 1,
                JournalEntryId = journalEntryId,
                ImageData = new byte[] { 1, 2, 3 },
                ContentType = "image/jpeg",
                Caption = "Test Image"
            }
        };
        
        // Setup repositories
        var journalEntriesRepo = TestHelpers.MockRepository(journalEntries);
        var tagsRepo = TestHelpers.MockRepository(tags);
        var journalEntryTagsRepo = TestHelpers.MockRepository(journalEntryTags);
        var journalImagesRepo = TestHelpers.MockRepository(images);
        
        // Setup unit of work
        var unitOfWork = TestHelpers.MockUnitOfWork(
            journalEntriesRepo, 
            journalImagesRepo, 
            tagsRepo, 
            journalEntryTagsRepo);
            
        var query = new GetJournalEntryByIdQuery(journalEntryId, userId);
        var handler = new GetJournalEntryByIdQueryHandler(unitOfWork.Object);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
          // Assert
        Assert.NotNull(result);
        Assert.Equal(journalEntryId, result.Id);
        Assert.Equal("Test Journal Entry", result.Title);
        Assert.Equal("<p>Test content</p>", result.Content);
        
        // Check nullable collections
        Assert.NotNull(result.Tags);
        Assert.Equal(2, result.Tags!.Count);
        Assert.Contains("tag1", result.Tags);
        Assert.Contains("tag2", result.Tags);
        
        Assert.NotNull(result.Images);
        Assert.Single(result.Images!);
    }
    
    [Fact]
    public async Task Handle_NonExistingJournalEntry_ReturnsNull()
    {
        // Arrange
        var userId = "user123";
        var nonExistingEntryId = 999;
        
        // Setup empty repositories
        var journalEntriesRepo = TestHelpers.MockRepository(new List<JournalEntry>());
        var tagsRepo = TestHelpers.MockRepository(new List<Tag>());
        var journalEntryTagsRepo = TestHelpers.MockRepository(new List<JournalEntryTag>());
        var journalImagesRepo = TestHelpers.MockRepository(new List<JournalImage>());
        
        // Setup unit of work
        var unitOfWork = TestHelpers.MockUnitOfWork(
            journalEntriesRepo, 
            journalImagesRepo, 
            tagsRepo, 
            journalEntryTagsRepo);
            
        var query = new GetJournalEntryByIdQuery(nonExistingEntryId, userId);
        var handler = new GetJournalEntryByIdQueryHandler(unitOfWork.Object);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Handle_JournalEntryBelongingToAnotherUser_ReturnsNull()
    {
        // Arrange
        var userId = "user123";
        var anotherUserId = "user456";
        var journalEntryId = 1;
        
        // Create test data for another user
        var journalEntries = new List<JournalEntry>
        {
            new JournalEntry
            {
                Id = journalEntryId,
                Title = "Test Journal Entry",
                Content = "<p>Test content</p>",
                EntryDate = DateTime.UtcNow.Date,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = null,
                UserId = anotherUserId // Different user ID
            }
        };
        
        // Setup repository
        var journalEntriesRepo = TestHelpers.MockRepository(journalEntries);
        
        // Setup unit of work
        var unitOfWork = TestHelpers.MockUnitOfWork(journalEntriesRepo);
            
        var query = new GetJournalEntryByIdQuery(journalEntryId, userId);
        var handler = new GetJournalEntryByIdQueryHandler(unitOfWork.Object);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
    }
}
