using MediatR;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using System.Linq;

namespace MeritJournal.Application.Features.JournalEntries.Queries;

/// <summary>
/// Query to get all journal entries for the current user.
/// </summary>
public record GetJournalEntriesQuery(string UserId) : IRequest<List<JournalEntryDto>>;

/// <summary>
/// Handler for GetJournalEntriesQuery.
/// </summary>
public class GetJournalEntriesQueryHandler : IRequestHandler<GetJournalEntriesQuery, List<JournalEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Constructor for GetJournalEntriesQueryHandler.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public GetJournalEntriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }    /// <summary>
    /// Handles the GetJournalEntriesQuery.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of journal entries as DTOs.</returns>
    public Task<List<JournalEntryDto>> Handle(GetJournalEntriesQuery request, CancellationToken cancellationToken)
    {
        // Get all journal entries for the current user
        var journalEntries = _unitOfWork.JournalEntries
            .Find(j => j.UserId == request.UserId)
            .OrderByDescending(j => j.EntryDate)
            .ToList();        // Load related images for all journal entries
        var journalEntryIds = journalEntries.Select(j => j.Id).ToList();
        
        // Fetch all related data in batch operations
        var images = _unitOfWork.JournalImages
            .Find(img => journalEntryIds.Contains(img.JournalEntryId))
            .ToList();
            
        var journalEntryTags = _unitOfWork.JournalEntryTags
            .Find(jet => journalEntryIds.Contains(jet.JournalEntryId))
            .ToList();
            
        // Get all tag IDs and fetch the tags
        var tagIds = journalEntryTags.Select(jet => jet.TagId).Distinct().ToList();
        var tags = _unitOfWork.Tags
            .Find(t => tagIds.Contains(t.Id))
            .ToList();
            
        // Group images by journal entry ID for easy lookup
        var imagesByJournalEntry = images
            .GroupBy(img => img.JournalEntryId)
            .ToDictionary(g => g.Key, g => g.ToList());
            
        // Group journal entry tags by journal entry ID for easy lookup
        var tagsByJournalEntry = journalEntryTags
            .GroupBy(jet => jet.JournalEntryId)
            .ToDictionary(g => g.Key, g => g.ToList());
            
        // Convert to DTOs
        var result = journalEntries.Select(entry => new JournalEntryDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.Content,
            CreatedAt = entry.CreatedAt,
            ModifiedAt = entry.ModifiedAt,
            EntryDate = entry.EntryDate,
            Images = imagesByJournalEntry.TryGetValue(entry.Id, out var entryImages) 
                ? entryImages.Select(image => new JournalImageDto
                {
                    Id = image.Id,
                    ImageDataBase64 = Convert.ToBase64String(image.ImageData),
                    ContentType = image.ContentType,
                    Caption = image.Caption,
                    JournalEntryId = image.JournalEntryId
                }).ToList() 
                : new List<JournalImageDto>(),
            Tags = tagsByJournalEntry.TryGetValue(entry.Id, out var entryTags)
                ? entryTags
                    .Select(jet => tags.FirstOrDefault(t => t.Id == jet.TagId)?.Name)
                    .Where(name => name != null)
                    .Cast<string>()
                    .ToList()
                : new List<string>()        }).ToList();

        return Task.FromResult(result);
    }
}
