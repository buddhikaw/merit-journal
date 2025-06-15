using MediatR;
using Microsoft.EntityFrameworkCore;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using MeritJournal.Domain.Entities;

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
    private readonly IApplicationDbContext _context;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateJournalEntryCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application DB context.</param>
    public UpdateJournalEntryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Handles the UpdateJournalEntryCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated journal entry as a DTO.</returns>
    public async Task<JournalEntryDto> Handle(UpdateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        // Get the existing journal entry
        var journalEntry = await _context.JournalEntries
            .Include(je => je.JournalEntryTags)
            .ThenInclude(jet => jet.Tag)
            .Include(je => je.Images)
            .FirstOrDefaultAsync(je => je.Id == request.Id && je.UserId == request.UserId, cancellationToken);
        
        if (journalEntry == null)
        {
            throw new KeyNotFoundException($"Journal entry with ID {request.Id} not found for user {request.UserId}");
        }
        
        // Update the basic properties
        journalEntry.Title = request.Title;
        journalEntry.Content = request.Content;
        journalEntry.EntryDate = DateTime.SpecifyKind(request.EntryDate.Date, DateTimeKind.Utc);
        journalEntry.ModifiedAt = DateTime.UtcNow;
          // Process tags if provided
        if (request.Tags != null && request.Tags.Any())
        {
            // Remove existing tags
            _context.JournalEntryTags.RemoveRange(journalEntry.JournalEntryTags);
            journalEntry.JournalEntryTags.Clear();
            
            // Add the new tags            // Process only non-empty tags
            var uniqueTagNames = request.Tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct()
                .ToList();
            
            foreach (var tagName in uniqueTagNames)
            {
                // Check if the tag already exists for this user
                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name == tagName && t.UserId == request.UserId, cancellationToken);
                
                if (tag == null)
                {
                    // Create new tag
                    tag = new Tag
                    {
                        Name = tagName,
                        UserId = request.UserId
                    };
                    _context.Tags.Add(tag);
                    
                    // Save changes to ensure the tag is created with an ID
                    await _context.SaveChangesAsync(cancellationToken);
                }
                
                // Create journal entry tag relationship
                journalEntry.JournalEntryTags.Add(new JournalEntryTag
                {
                    TagId = tag.Id,
                    JournalEntryId = journalEntry.Id
                });
            }
        }
        else
        {
            // If no tags are provided, remove all existing tags
            _context.JournalEntryTags.RemoveRange(journalEntry.JournalEntryTags);
            journalEntry.JournalEntryTags.Clear();
        }
        
        // Process images if provided
        if (request.Images != null && request.Images.Any())
        {
            // Remove images that are not in the update request
            var requestImageIds = request.Images.Where(i => i.Id > 0).Select(i => i.Id).ToList();
            var imagesToRemove = journalEntry.Images.Where(i => !requestImageIds.Contains(i.Id)).ToList();
            
            foreach (var image in imagesToRemove)
            {
                _context.JournalImages.Remove(image);
                journalEntry.Images.Remove(image);
            }
            
            // Update existing images and add new ones
            foreach (var imageDto in request.Images)
            {
                if (imageDto.Id > 0)
                {                    // Update existing image
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
                    }
                }
                else
                {                    // Add new image
                    journalEntry.Images.Add(new JournalImage
                    {
                        ImageData = Convert.FromBase64String(imageDto.ImageDataBase64),
                        ContentType = imageDto.ContentType,
                        Caption = imageDto.Caption
                    });
                }
            }
        }
        
        // Save changes to the database
        await _context.SaveChangesAsync(cancellationToken);
          // Prepare DTO for response
        var dto = new JournalEntryDto
        {
            Id = journalEntry.Id,
            Title = journalEntry.Title,
            Content = journalEntry.Content,
            EntryDate = journalEntry.EntryDate,
            CreatedAt = journalEntry.CreatedAt,
            ModifiedAt = journalEntry.ModifiedAt,
            Tags = journalEntry.JournalEntryTags
                .Where(jet => jet.Tag != null)
                .Select(jet => jet.Tag!.Name)
                .ToList(),
            Images = journalEntry.Images
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
}
