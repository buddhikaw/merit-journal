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
    public class UpdateJournalEntryCommandTests
    {
        [Fact]
        public async Task Handle_UpdatesBasicProperties_WhenEntryExists()
        {
            // Arrange
            var userId = "test-user-id";
            var entryId = 1;
            var originalTitle = "Original Title";
            var originalContent = "Original Content";
            var originalDate = DateTime.UtcNow.AddDays(-2);
            
            var updatedTitle = "Updated Title";
            var updatedContent = "Updated Content";
            var updatedDate = DateTime.UtcNow.Date;
            
            var cancellationToken = CancellationToken.None;
            
            // Create the original entry
            var existingEntry = new JournalEntry
            {
                Id = entryId,
                Title = originalTitle,
                Content = originalContent,
                EntryDate = originalDate,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UserId = userId,
                JournalEntryTags = new List<JournalEntryTag>(),
                Images = new List<JournalImage>()
            };
            
            // Setup mock DbContext
            var mockContext = SetupMockDbContext(new List<JournalEntry> { existingEntry });
            
            // Create the command
            var command = new UpdateJournalEntryCommand
            {
                Id = entryId,
                Title = updatedTitle,
                Content = updatedContent,
                EntryDate = updatedDate,
                UserId = userId,
                Tags = null,
                Images = null
            };
            
            var handler = new UpdateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(entryId, result.Id);
            Assert.Equal(updatedTitle, result.Title);
            Assert.Equal(updatedContent, result.Content);
            Assert.Equal(updatedDate, result.EntryDate);
            
            // Verify the journal entry was saved to the database
            mockContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.AtLeastOnce());
            
            // Verify that the original entry was updated
            Assert.Equal(updatedTitle, existingEntry.Title);
            Assert.Equal(updatedContent, existingEntry.Content);
            Assert.Equal(updatedDate, existingEntry.EntryDate);
            Assert.NotNull(existingEntry.ModifiedAt);
        }
        
        [Fact]
        public async Task Handle_UpdatesTags_RemovesOldAddNewTags()
        {
            // Arrange
            var userId = "test-user-id";
            var entryId = 1;
            
            // Create existing tags
            var existingTag1 = new Tag { Id = 1, Name = "tag1", UserId = userId };
            var existingTag2 = new Tag { Id = 2, Name = "tag2", UserId = userId };
            var existingTag3 = new Tag { Id = 3, Name = "tag3", UserId = userId };
            
            var allTags = new List<Tag> { existingTag1, existingTag2, existingTag3 };
            
            // Create original entry with tag1 and tag2
            var existingEntry = new JournalEntry
            {
                Id = entryId,
                Title = "Test Entry",
                Content = "Test Content",
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                JournalEntryTags = new List<JournalEntryTag>
                {
                    new JournalEntryTag { JournalEntryId = entryId, TagId = 1, Tag = existingTag1 },
                    new JournalEntryTag { JournalEntryId = entryId, TagId = 2, Tag = existingTag2 }
                },
                Images = new List<JournalImage>()
            };
            
            var cancellationToken = CancellationToken.None;
            
            // Setup mock DbContext
            var mockContext = SetupMockDbContext(
                new List<JournalEntry> { existingEntry },
                allTags);
                
            // Create an instance of the captured JournalEntryTags collection to check later
            var capturedJournalEntryTags = new List<JournalEntryTag>();
            var mockJournalEntryTagsDbSet = new Mock<DbSet<JournalEntryTag>>();
            mockJournalEntryTagsDbSet.Setup(m => m.Add(It.IsAny<JournalEntryTag>()))
                .Callback<JournalEntryTag>(tag => capturedJournalEntryTags.Add(tag))
                .Returns((JournalEntryTag tag) => tag);
                
            mockContext.Setup(c => c.JournalEntryTags).Returns(mockJournalEntryTagsDbSet.Object);
            
            // Create the command with updated tags (tag2 and tag3)
            var command = new UpdateJournalEntryCommand
            {
                Id = entryId,
                Title = "Updated Test Entry",
                Content = "Updated Test Content",
                EntryDate = DateTime.UtcNow.Date,
                UserId = userId,
                Tags = new List<string> { "tag2", "tag3", "tag4" }, // Keep tag2, remove tag1, add tag3 and new tag4
                Images = null
            };
            
            var handler = new UpdateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Tags);
            
            // Verify command removed old tags
            mockContext.Verify(c => c.JournalEntryTags.RemoveRange(existingEntry.JournalEntryTags), Times.Once());
            
            // Verify the updated tags in the result
            Assert.Equal(3, result.Tags.Count);
            Assert.Contains("tag2", result.Tags);
            Assert.Contains("tag3", result.Tags);
            Assert.Contains("tag4", result.Tags);
            Assert.DoesNotContain("tag1", result.Tags);
        }
        
        [Fact]
        public async Task Handle_ThrowsKeyNotFoundException_WhenEntryNotFound()
        {
            // Arrange
            var userId = "test-user-id";
            var nonExistentEntryId = 999;
            var cancellationToken = CancellationToken.None;
            
            // Setup mock DbContext with empty entries collection
            var mockContext = SetupMockDbContext(new List<JournalEntry>());
            
            // Create the command
            var command = new UpdateJournalEntryCommand
            {
                Id = nonExistentEntryId,
                Title = "Updated Title",
                Content = "Updated Content",
                EntryDate = DateTime.UtcNow,
                UserId = userId
            };
            
            var handler = new UpdateJournalEntryCommandHandler(mockContext.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                handler.Handle(command, cancellationToken));
        }
        
        [Fact]
        public async Task Handle_FiltersDuplicateAndEmptyTags()
        {
            // Arrange
            var userId = "test-user-id";
            var entryId = 1;
            
            // Create existing entry
            var existingEntry = new JournalEntry
            {
                Id = entryId,
                Title = "Test Entry",
                Content = "Test Content",
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                JournalEntryTags = new List<JournalEntryTag>(),
                Images = new List<JournalImage>()
            };
            
            var cancellationToken = CancellationToken.None;
            
            // Setup mock DbContext
            var mockContext = SetupMockDbContext(new List<JournalEntry> { existingEntry });
            
            // Create the command with duplicate and empty tags
            var command = new UpdateJournalEntryCommand
            {
                Id = entryId,
                Title = "Updated Title",
                Content = "Updated Content",
                EntryDate = DateTime.UtcNow,
                UserId = userId,
                Tags = new List<string> { "tag1", "tag1", "", " tag2 ", string.Empty, "  " },
                Images = null
            };
            
            var handler = new UpdateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Tags);
            
            // Verify duplicate and empty tags are filtered
            Assert.Equal(2, result.Tags.Count);
            Assert.Contains("tag1", result.Tags);
            Assert.Contains("tag2", result.Tags);
        }

        [Fact]
        public async Task Handle_UpdatesImages_RemovesOldAddsNewUpdatesExisting()
        {
            // Arrange
            var userId = "test-user-id";
            var entryId = 1;
            
            // Create existing images
            var existingImage1 = new JournalImage 
            { 
                Id = 1, 
                ImageData = Convert.FromBase64String("dGVzdGltYWdlZGF0YTE="),  // "testimagedata1" in base64 
                ContentType = "image/jpeg",
                Caption = "Original Caption 1",
                JournalEntryId = entryId
            };
            
            var existingImage2 = new JournalImage 
            { 
                Id = 2, 
                ImageData = Convert.FromBase64String("dGVzdGltYWdlZGF0YTI="),  // "testimagedata2" in base64
                ContentType = "image/png", 
                Caption = "Original Caption 2",
                JournalEntryId = entryId
            };
            
            // Create original entry with two images
            var existingEntry = new JournalEntry
            {
                Id = entryId,
                Title = "Test Entry",
                Content = "Test Content",
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                JournalEntryTags = new List<JournalEntryTag>(),
                Images = new List<JournalImage> { existingImage1, existingImage2 }
            };
            
            var cancellationToken = CancellationToken.None;
            
            // Setup mock DbContext
            var mockContext = SetupMockDbContext(new List<JournalEntry> { existingEntry });
            
            // Mock JournalImages DbSet for remove operations tracking
            var removedImages = new List<JournalImage>();
            var mockJournalImagesDbSet = new Mock<DbSet<JournalImage>>();
            mockJournalImagesDbSet.Setup(m => m.Remove(It.IsAny<JournalImage>()))
                .Callback<JournalImage>(image => removedImages.Add(image))
                .Returns((JournalImage image) => image);
                
            mockContext.Setup(c => c.JournalImages).Returns(mockJournalImagesDbSet.Object);
            
            // Create the command with updated images (update image1, remove image2, add a new image3)
            var command = new UpdateJournalEntryCommand
            {
                Id = entryId,
                Title = "Updated Title",
                Content = "Updated Content",
                EntryDate = DateTime.UtcNow.Date,
                UserId = userId,
                Tags = null,
                Images = new List<JournalImageDto>
                {
                    // Update existing image 1 with new caption
                    new JournalImageDto 
                    { 
                        Id = 1,
                        ImageDataBase64 = "", // Empty means don't update the binary data
                        ContentType = "image/jpeg",
                        Caption = "Updated Caption 1",
                        JournalEntryId = entryId
                    },
                    // Add new image
                    new JournalImageDto
                    {
                        Id = 0, // New image has Id = 0
                        ImageDataBase64 = "bmV3aW1hZ2VkYXRhMw==", // "newimagedata3" in base64
                        ContentType = "image/gif",
                        Caption = "New Image 3",
                        JournalEntryId = entryId
                    }
                    // Note: Image 2 is not included, so it should be removed
                }
            };
            
            var handler = new UpdateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Images);
            
            // Should have 2 images in the result
            Assert.Equal(2, result.Images.Count);
            
            // Check that image1 was updated properly
            var updatedImage1 = existingEntry.Images.FirstOrDefault(i => i.Id == 1);
            Assert.NotNull(updatedImage1);
            Assert.Equal("Updated Caption 1", updatedImage1.Caption);
            Assert.Equal("image/jpeg", updatedImage1.ContentType);
            Assert.Equal(Convert.ToBase64String(existingImage1.ImageData), Convert.ToBase64String(updatedImage1.ImageData));
            
            // Verify that image2 was removed
            Assert.Contains(removedImages, i => i.Id == 2);
            Assert.DoesNotContain(existingEntry.Images, i => i.Id == 2);
            
            // Verify a new image was added
            var newImage = existingEntry.Images.FirstOrDefault(i => i.Id != 1 && i.Id != 2);
            Assert.NotNull(newImage);
            Assert.Equal("New Image 3", newImage.Caption);
            Assert.Equal("image/gif", newImage.ContentType);
            Assert.Equal("bmV3aW1hZ2VkYXRhMw==", Convert.ToBase64String(newImage.ImageData));
            
            // Verify the database was updated
            mockContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.AtLeastOnce());
        }
        
        [Fact]
        public async Task Handle_UpdatesEntryWithNoImagesOrTags()
        {
            // Arrange
            var userId = "test-user-id";
            var entryId = 1;
            
            // Create existing tags and images
            var existingTag = new Tag { Id = 1, Name = "oldtag", UserId = userId };
            var existingImage = new JournalImage 
            { 
                Id = 1, 
                ImageData = Convert.FromBase64String("dGVzdGltYWdlZGF0YQ=="),  // "testimagedata" in base64 
                ContentType = "image/jpeg",
                Caption = "Original Caption",
                JournalEntryId = entryId
            };
            
            // Create original entry with one tag and one image
            var existingEntry = new JournalEntry
            {
                Id = entryId,
                Title = "Test Entry",
                Content = "Test Content",
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                JournalEntryTags = new List<JournalEntryTag> 
                { 
                    new JournalEntryTag { JournalEntryId = entryId, TagId = 1, Tag = existingTag }
                },
                Images = new List<JournalImage> { existingImage }
            };
            
            var cancellationToken = CancellationToken.None;
            
            // Setup mock DbContext
            var mockContext = SetupMockDbContext(new List<JournalEntry> { existingEntry });
            
            // Mock JournalImages DbSet for remove operations tracking
            var removedImages = new List<JournalImage>();
            var mockJournalImagesDbSet = new Mock<DbSet<JournalImage>>();
            mockJournalImagesDbSet.Setup(m => m.Remove(It.IsAny<JournalImage>()))
                .Callback<JournalImage>(image => removedImages.Add(image))
                .Returns((JournalImage image) => image);
                
            mockContext.Setup(c => c.JournalImages).Returns(mockJournalImagesDbSet.Object);
            
            // Create the command with empty lists for tags and images
            var command = new UpdateJournalEntryCommand
            {
                Id = entryId,
                Title = "Updated Title",
                Content = "Updated Content",
                EntryDate = DateTime.UtcNow.Date,
                UserId = userId,
                Tags = new List<string>(), // Empty list should remove all tags
                Images = new List<JournalImageDto>() // Empty list should remove all images
            };
            
            var handler = new UpdateJournalEntryCommandHandler(mockContext.Object);
            
            // Act
            var result = await handler.Handle(command, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
              // Verify tags collection is either empty or null 
            Assert.True(result.Tags == null || result.Tags.Count == 0);
            
            // Verify images collection is either empty or null
            Assert.True(result.Images == null || result.Images.Count == 0);
            
            // Verify the journal entry was saved to the database
            mockContext.Verify(c => c.SaveChangesAsync(cancellationToken), Times.AtLeastOnce());
            
            // Verify the original entry was updated
            Assert.Equal("Updated Title", existingEntry.Title);
            Assert.Equal("Updated Content", existingEntry.Content);
        }
        
        [Fact]
        public async Task Handle_EnsuresSameUser_WhenUpdatingEntry()
        {
            // Arrange
            var userId = "test-user-id";
            var differentUserId = "different-user-id";
            var entryId = 1;
            
            // Create the original entry with one userId
            var existingEntry = new JournalEntry
            {
                Id = entryId,
                Title = "Original Title",
                Content = "Original Content",
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                JournalEntryTags = new List<JournalEntryTag>(),
                Images = new List<JournalImage>()
            };
            
            var cancellationToken = CancellationToken.None;
            
            // Setup mock DbContext
            var mockContext = SetupMockDbContext(new List<JournalEntry> { existingEntry });
            
            // Create the command with a different userId
            var command = new UpdateJournalEntryCommand
            {
                Id = entryId,
                Title = "Updated Title",
                Content = "Updated Content",
                EntryDate = DateTime.UtcNow.Date,
                UserId = differentUserId, // Different user ID
                Tags = null,
                Images = null
            };
            
            var handler = new UpdateJournalEntryCommandHandler(mockContext.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                handler.Handle(command, cancellationToken));
            
            // Verify the entry was not updated
            Assert.Equal("Original Title", existingEntry.Title);
            Assert.Equal("Original Content", existingEntry.Content);
        }

        private Mock<IApplicationDbContext> SetupMockDbContext(
            List<JournalEntry> journalEntries,
            List<Tag>? existingTags = null)
        {
            var mockContext = new Mock<IApplicationDbContext>();
            
            // Mock JournalEntries DbSet
            var mockJournalEntriesDbSet = MockDbSet(journalEntries);
            mockContext.Setup(c => c.JournalEntries).Returns(mockJournalEntriesDbSet.Object);
            
            // Mock JournalEntryTags DbSet with empty collection
            var mockJournalEntryTagsDbSet = new Mock<DbSet<JournalEntryTag>>();
            mockContext.Setup(c => c.JournalEntryTags).Returns(mockJournalEntryTagsDbSet.Object);
            
            // Mock Tags DbSet
            existingTags ??= new List<Tag>();
            var mockTagsDbSet = MockDbSet(existingTags);
            mockContext.Setup(c => c.Tags).Returns(mockTagsDbSet.Object);
            
            // Setup Tags.Add to capture added tags
            mockTagsDbSet.Setup(m => m.Add(It.IsAny<Tag>()))
                .Callback<Tag>(tag => {
                    tag.Id = existingTags.Count + 1;
                    existingTags.Add(tag);
                })
                .Returns((Tag tag) => tag);
            
            // Setup SaveChangesAsync
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
                
            return mockContext;
        }
        
        private static Mock<DbSet<T>> MockDbSet<T>(List<T> data) where T : class
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
