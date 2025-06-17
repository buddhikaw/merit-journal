using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Features.JournalEntries.Commands;
using MeritJournal.Application.Interfaces;
using MeritJournal.Domain.Entities;
using MeritJournal.UnitTests.Common;
using Moq;
using Xunit;

namespace MeritJournal.UnitTests.Features.JournalEntries.Commands;

public class CreateJournalEntryCommandTests
{
    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateJournalEntry()
    {
        // Arrange
        var journalEntries = new List<JournalEntry>();
        var tags = new List<Tag>();
        var journalEntryTags = new List<JournalEntryTag>();
        var images = new List<JournalImage>();
        
        var journalEntriesRepo = TestHelpers.MockRepository(journalEntries);
        journalEntriesRepo.Setup(r => r.Add(It.IsAny<JournalEntry>()))
            .Callback<JournalEntry>(je => 
            { 
                je.Id = 1; // Simulate auto-generated ID
                journalEntries.Add(je);
            });
            
        var tagsRepo = TestHelpers.MockRepository(tags);
        tagsRepo.Setup(r => r.Add(It.IsAny<Tag>()))
            .Callback<Tag>(t => 
            { 
                t.Id = tags.Count + 1;
                tags.Add(t);
            });
            
        var journalEntryTagsRepo = TestHelpers.MockRepository(journalEntryTags);
        journalEntryTagsRepo.Setup(r => r.Add(It.IsAny<JournalEntryTag>()))
            .Callback<JournalEntryTag>(jet => journalEntryTags.Add(jet));
            
        var journalImagesRepo = TestHelpers.MockRepository(images);
        journalImagesRepo.Setup(r => r.Add(It.IsAny<JournalImage>()))
            .Callback<JournalImage>(img =>
            {
                img.Id = images.Count + 1;
                images.Add(img);
            });
        
        var unitOfWork = TestHelpers.MockUnitOfWork(
            journalEntriesRepo,
            journalImagesRepo,
            tagsRepo,
            journalEntryTagsRepo);
            
        var handler = new CreateJournalEntryCommandHandler(unitOfWork.Object);
        
        // Create a test command
        var command = new CreateJournalEntryCommand
        {
            Title = "Test Journal Entry",
            Content = "<p>Test content</p>",
            EntryDate = DateTime.UtcNow.Date,
            UserId = "user123",
            Tags = new List<string> { "tag1", "tag2" },
            Images = new List<JournalImageDto>
            {
                new JournalImageDto
                {
                    ImageDataBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
                    ContentType = "image/jpeg",
                    Caption = "Test Image"
                }
            }
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
          // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Content, result.Content);
        Assert.Equal(command.EntryDate.Date, result.EntryDate.Date);
        
        // Check nullable collections
        Assert.NotNull(result.Tags);
        Assert.Equal(2, result.Tags!.Count);
        Assert.Contains("tag1", result.Tags);
        Assert.Contains("tag2", result.Tags);
        
        Assert.NotNull(result.Images);
        Assert.Single(result.Images!);
        
        // Verify that the unit of work was called to save changes
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    }
    
    [Fact]
    public async Task Handle_WithExistingTags_ShouldReuseExistingTags()
    {
        // Arrange
        var journalEntries = new List<JournalEntry>();
        var tags = new List<Tag>
        {
            new Tag { Id = 1, Name = "tag1", UserId = "user123" }
        };
        var journalEntryTags = new List<JournalEntryTag>();
        var images = new List<JournalImage>();
        
        var journalEntriesRepo = TestHelpers.MockRepository(journalEntries);
        journalEntriesRepo.Setup(r => r.Add(It.IsAny<JournalEntry>()))
            .Callback<JournalEntry>(je => 
            { 
                je.Id = 1; // Simulate auto-generated ID
                journalEntries.Add(je);
            });
            
        var tagsRepo = TestHelpers.MockRepository(tags);
        tagsRepo.Setup(r => r.Add(It.IsAny<Tag>()))
            .Callback<Tag>(t => 
            { 
                t.Id = tags.Count + 1;
                tags.Add(t);
            });
            
        var journalEntryTagsRepo = TestHelpers.MockRepository(journalEntryTags);
        journalEntryTagsRepo.Setup(r => r.Add(It.IsAny<JournalEntryTag>()))
            .Callback<JournalEntryTag>(jet => journalEntryTags.Add(jet));
            
        var unitOfWork = TestHelpers.MockUnitOfWork(
            journalEntriesRepo, 
            null, 
            tagsRepo, 
            journalEntryTagsRepo);
            
        var handler = new CreateJournalEntryCommandHandler(unitOfWork.Object);
        
        // Create a test command with one existing tag and one new tag
        var command = new CreateJournalEntryCommand
        {
            Title = "Test Journal Entry",
            Content = "<p>Test content</p>",
            EntryDate = DateTime.UtcNow.Date,
            UserId = "user123",
            Tags = new List<string> { "tag1", "tag2" }
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
          // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Tags);
        Assert.Equal(2, result.Tags!.Count);
        
        // Verify that only one new tag was created
        Assert.Equal(2, tags.Count); // Original tag + one new tag
        
        // Verify that both tags were linked to the journal entry
        Assert.Equal(2, journalEntryTags.Count);
    }
}
