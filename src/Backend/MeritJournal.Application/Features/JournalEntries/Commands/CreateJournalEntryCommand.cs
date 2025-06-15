using MediatR;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using MeritJournal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeritJournal.Application.Features.JournalEntries.Commands;

/// <summary>
/// Command to create a new journal entry.
/// </summary>
public record CreateJournalEntryCommand : IRequest<JournalEntryDto>
{
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
/// Handler for CreateJournalEntryCommand.
/// </summary>
public class CreateJournalEntryCommandHandler : IRequestHandler<CreateJournalEntryCommand, JournalEntryDto>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Constructor for CreateJournalEntryCommandHandler.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public CreateJournalEntryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the CreateJournalEntryCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created journal entry as a DTO.</returns>
    public async Task<JournalEntryDto> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
    {        // Create a new journal entry with explicit UTC dates for PostgreSQL
        var journalEntry = new JournalEntry
        {
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow, // Already UTC
            // Ensure the date portion also has UTC kind
            EntryDate = DateTime.SpecifyKind(request.EntryDate.Date, DateTimeKind.Utc),
            UserId = request.UserId
        };

        // Add the journal entry to the context
        _context.JournalEntries.Add(journalEntry);
        await _context.SaveChangesAsync(cancellationToken);

        // Process tags
        if (request.Tags != null && request.Tags.Any())
        {
            foreach (var tagName in request.Tags)
            {
                // Check if the tag already exists for this user
                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name == tagName && t.UserId == request.UserId, cancellationToken);

                // If not, create it
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName,
                        UserId = request.UserId
                    };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Create the journal entry tag relationship
                var journalEntryTag = new JournalEntryTag
                {
                    JournalEntryId = journalEntry.Id,
                    TagId = tag.Id
                };
                _context.JournalEntryTags.Add(journalEntryTag);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Process images
        if (request.Images != null && request.Images.Any())
        {
            foreach (var imageDto in request.Images)
            {
                byte[] imageData;
                try
                {
                    imageData = Convert.FromBase64String(imageDto.ImageDataBase64);
                }
                catch
                {
                    // Skip invalid images
                    continue;
                }

                var journalImage = new JournalImage
                {
                    ImageData = imageData,
                    ContentType = imageDto.ContentType,
                    Caption = imageDto.Caption,
                    JournalEntryId = journalEntry.Id
                };
                _context.JournalImages.Add(journalImage);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Reload the journal entry with all related data
        journalEntry = await _context.JournalEntries
            .Include(j => j.Images)
            .Include(j => j.JournalEntryTags)
                .ThenInclude(jet => jet.Tag)
            .FirstAsync(j => j.Id == journalEntry.Id, cancellationToken);

        // Create the result DTO
        var result = new JournalEntryDto
        {
            Id = journalEntry.Id,
            Title = journalEntry.Title,
            Content = journalEntry.Content,
            CreatedAt = journalEntry.CreatedAt,
            ModifiedAt = journalEntry.ModifiedAt,
            EntryDate = journalEntry.EntryDate,
            Images = journalEntry.Images.Select(image => new JournalImageDto
            {
                Id = image.Id,
                ImageDataBase64 = Convert.ToBase64String(image.ImageData),
                ContentType = image.ContentType,
                Caption = image.Caption,
                JournalEntryId = image.JournalEntryId
            }).ToList(),
            Tags = journalEntry.JournalEntryTags.Select(jet => jet.Tag!.Name).ToList()
        };

        return result;
    }
}
