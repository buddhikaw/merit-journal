using MediatR;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using System.Linq;

namespace MeritJournal.Application.Features.JournalEntries.Queries;

/// <summary>
/// Query to get a journal entry by ID.
/// </summary>
public class GetJournalEntryByIdQuery : IRequest<JournalEntryDto?>
{
    /// <summary>
    /// The ID of the journal entry to fetch.
    /// </summary>
    public int JournalEntryId { get; }
    
    /// <summary>
    /// The user ID from the external OIDC provider (e.g., 'sub' claim).
    /// </summary>
    public string UserId { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GetJournalEntryByIdQuery"/> class.
    /// </summary>
    /// <param name="journalEntryId">The ID of the journal entry to fetch.</param>
    /// <param name="userId">The user's ID.</param>
    public GetJournalEntryByIdQuery(int journalEntryId, string userId)
    {
        JournalEntryId = journalEntryId;
        UserId = userId;
    }
}

/// <summary>
/// Handler for retrieving a single journal entry.
/// </summary>
public class GetJournalEntryByIdQueryHandler : IRequestHandler<GetJournalEntryByIdQuery, JournalEntryDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GetJournalEntryByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public GetJournalEntryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Handles the GetJournalEntryByIdQuery.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The journal entry as a DTO, or null if not found.</returns>
    public async Task<JournalEntryDto?> Handle(GetJournalEntryByIdQuery request, CancellationToken cancellationToken)
    {
        // Find the journal entry
        var journalEntry = await _unitOfWork.JournalEntries
            .FirstOrDefaultAsync(je => je.Id == request.JournalEntryId && je.UserId == request.UserId);
          if (journalEntry == null)
        {
            return null;
        }
        
        // Load the related data
        var journalEntryTags = _unitOfWork.JournalEntryTags
            .Find(jet => jet.JournalEntryId == journalEntry.Id)
            .ToList();
        
        // Get all tag IDs used in this journal entry
        var tagIds = journalEntryTags.Select(jet => jet.TagId).ToList();
        
        // Load all tags for this journal entry
        var tags = _unitOfWork.Tags
            .Find(t => tagIds.Contains(t.Id))
            .ToList();
            
        // Load all images for this journal entry
        var images = _unitOfWork.JournalImages
            .Find(img => img.JournalEntryId == journalEntry.Id)
            .ToList();
            
        // Combine the data to create the journal entry DTO
        var dto = new JournalEntryDto
        {
            Id = journalEntry.Id,
            Title = journalEntry.Title,
            Content = journalEntry.Content,
            EntryDate = journalEntry.EntryDate,
            CreatedAt = journalEntry.CreatedAt,
            ModifiedAt = journalEntry.ModifiedAt,
            Tags = journalEntryTags
                .Join(
                    tags,
                    jet => jet.TagId,
                    tag => tag.Id,
                    (jet, tag) => tag.Name
                )
                .ToList(),
            Images = images.Select(image => new JournalImageDto
                {
                    Id = image.Id,
                    ImageDataBase64 = Convert.ToBase64String(image.ImageData),
                    ContentType = image.ContentType,
                    Caption = image.Caption,
                    JournalEntryId = image.JournalEntryId
                }).ToList()
        };
        
        return dto;
    }
}
