using MediatR;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using MeritJournal.Domain.Entities;
using System.Linq;

namespace MeritJournal.Application.Features.JournalEntries.Commands;

/// <summary>
/// Command to update an existing journal entry.
/// </summary>
public class UpdateJournalEntryCommand : IRequest<JournalEntryDto>
{
    /// <summary>
    /// The ID of the journal entry to update.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The title of the journal entry.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// The content of the journal entry in HTML format.
    /// </summary>
    public required string Content { get; init; }
    
    /// <summary>
    /// The date to which this journal entry pertains.
    /// </summary>
    public DateTime EntryDate { get; init; }
    
    /// <summary>
    /// The images associated with this journal entry.
    /// </summary>
    public List<JournalImageDto>? Images { get; init; }
    
    /// <summary>
    /// The tags associated with this journal entry.
    /// </summary>
    public List<string>? Tags { get; init; }
    
    /// <summary>
    /// The user ID from the external OIDC provider (e.g., 'sub' claim).
    /// </summary>
    public required string UserId { get; init; }
}

/// <summary>
/// Handler for updating a journal entry.
/// </summary>
public class UpdateJournalEntryCommandHandler : IRequestHandler<UpdateJournalEntryCommand, JournalEntryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateJournalEntryCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public UpdateJournalEntryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Handles the UpdateJournalEntryCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated journal entry as a DTO.</returns>
    public async Task<JournalEntryDto> Handle(UpdateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        // Begin transaction
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Get the existing journal entry
            var journalEntry = await _unitOfWork.JournalEntries
                .FirstOrDefaultAsync(je => je.Id == request.Id && je.UserId == request.UserId);
          if (journalEntry == null)
        {
            throw new KeyNotFoundException($"Journal entry with ID {request.Id} not found for user {request.UserId}");
        }
        
        // Get all related data
        var journalEntryTags = _unitOfWork.JournalEntryTags
            .Find(jet => jet.JournalEntryId == journalEntry.Id)
            .ToList();
            
        var images = _unitOfWork.JournalImages
            .Find(img => img.JournalEntryId == journalEntry.Id)
            .ToList();
            
        // Update the basic properties
        journalEntry.Title = request.Title;
        journalEntry.Content = request.Content;
        journalEntry.EntryDate = DateTime.SpecifyKind(request.EntryDate.Date, DateTimeKind.Utc);
        journalEntry.ModifiedAt = DateTime.UtcNow;
        journalEntry.JournalEntryTags = journalEntryTags;
        journalEntry.Images = images;
        
        // Process tags if provided
        if (request.Tags != null && request.Tags.Any())
        {
            // Remove existing tags
            foreach (var tag in journalEntryTags.ToList())
            {
                _unitOfWork.JournalEntryTags.Remove(tag);
            }
            journalEntry.JournalEntryTags.Clear();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Add the new tags
            // Process only non-empty tags
            var uniqueTagNames = request.Tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct()
                .ToList();
            
            foreach (var tagName in uniqueTagNames)
            {
                // Check if the tag already exists for this user
                var tag = await _unitOfWork.Tags
                    .FirstOrDefaultAsync(t => t.Name == tagName && t.UserId == request.UserId);
                
                if (tag == null)
                {
                    // Create new tag
                    tag = new Tag
                    {
                        Name = tagName,
                        UserId = request.UserId
                    };
                    _unitOfWork.Tags.Add(tag);
                    
                    // Save changes to ensure the tag is created with an ID
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                
                // Create journal entry tag relationship
                var journalEntryTag = new JournalEntryTag
                {
                    TagId = tag.Id,
                    JournalEntryId = journalEntry.Id
                };
                _unitOfWork.JournalEntryTags.Add(journalEntryTag);
                journalEntry.JournalEntryTags.Add(journalEntryTag);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // If no tags are provided, remove all existing tags
            foreach (var tag in journalEntryTags.ToList())
            {
                _unitOfWork.JournalEntryTags.Remove(tag);
            }
            journalEntry.JournalEntryTags.Clear();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
          // Process images if provided
        if (request.Images != null && request.Images.Any())
        {
            // Remove images that are not in the update request
            var requestImageIds = request.Images.Where(i => i.Id > 0).Select(i => i.Id).ToList();
            var imagesToRemove = journalEntry.Images.Where(i => !requestImageIds.Contains(i.Id)).ToList();
            
            foreach (var image in imagesToRemove)
            {
                _unitOfWork.JournalImages.Remove(image);
                journalEntry.Images.Remove(image);
            }
            
            // Update existing images and add new ones
            foreach (var imageDto in request.Images)
            {
                if (imageDto.Id > 0)
                {
                    // Update existing image
                    var existingImage = journalEntry.Images.FirstOrDefault(i => i.Id == imageDto.Id);
                    if (existingImage != null)
                    {
                        // Update image data if provided
                        if (!string.IsNullOrEmpty(imageDto.ImageDataBase64))
                        {
                            existingImage.ImageData = Convert.FromBase64String(imageDto.ImageDataBase64);
                        }
                        existingImage.ContentType = imageDto.ContentType;
                        existingImage.Caption = imageDto.Caption;
                        
                        _unitOfWork.JournalImages.Update(existingImage);
                    }
                }
                else
                {
                    // Add new image
                    var newImage = new JournalImage
                    {
                        ImageData = Convert.FromBase64String(imageDto.ImageDataBase64),
                        ContentType = imageDto.ContentType,
                        Caption = imageDto.Caption,
                        JournalEntryId = journalEntry.Id
                    };
                    _unitOfWork.JournalImages.Add(newImage);
                    journalEntry.Images.Add(newImage);
                }
            }
        }
        
        // Update the journal entry
        _unitOfWork.JournalEntries.Update(journalEntry);
        
        // Save changes to the database
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        
        // Refresh data for the DTO
        var refreshedTags = _unitOfWork.JournalEntryTags
            .Find(jet => jet.JournalEntryId == journalEntry.Id)
            .ToList();
            
        var tagIds = refreshedTags.Select(t => t.TagId).ToList();
        var tags = _unitOfWork.Tags
            .Find(t => tagIds.Contains(t.Id))
            .ToList();
            
        var refreshedImages = _unitOfWork.JournalImages
            .Find(i => i.JournalEntryId == journalEntry.Id)
            .ToList();
        
        // Prepare DTO for response
        var dto = new JournalEntryDto
        {
            Id = journalEntry.Id,
            Title = journalEntry.Title,
            Content = journalEntry.Content,
            EntryDate = journalEntry.EntryDate,
            CreatedAt = journalEntry.CreatedAt,
            ModifiedAt = journalEntry.ModifiedAt,
            Tags = refreshedTags
                .Join(
                    tags,
                    jet => jet.TagId,
                    tag => tag.Id,
                    (_, tag) => tag.Name
                )
                .ToList(),
            Images = refreshedImages
                .Select(i => new JournalImageDto
                {
                    Id = i.Id,
                    ImageDataBase64 = Convert.ToBase64String(i.ImageData),
                    ContentType = i.ContentType,
                    Caption = i.Caption,
                    JournalEntryId = i.JournalEntryId
                })
                .ToList()
        };
        
        return dto;
        
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
