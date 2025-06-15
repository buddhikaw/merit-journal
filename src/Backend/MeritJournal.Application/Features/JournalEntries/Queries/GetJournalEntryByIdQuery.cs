using MediatR;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    private readonly IApplicationDbContext _context;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GetJournalEntryByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application DB context.</param>
    public GetJournalEntryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Handles the GetJournalEntryByIdQuery.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The journal entry as a DTO, or null if not found.</returns>
    public async Task<JournalEntryDto?> Handle(GetJournalEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var journalEntry = await _context.JournalEntries
            .Include(je => je.JournalEntryTags)
            .ThenInclude(jet => jet.Tag)
            .Include(je => je.Images)
            .FirstOrDefaultAsync(je => je.Id == request.JournalEntryId && je.UserId == request.UserId, cancellationToken);
        
        if (journalEntry == null)
        {
            return null;
        }
      // Create the DTO from the entity
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
                .Select(jet => new TagDto
                {
                    Id = jet.Tag!.Id,
                    Name = jet.Tag!.Name
                })
                .Select(tag => tag.Name)
                .ToList(),
            Images = new List<JournalImageDto>() // We'll handle images separately if needed
        };
        
        return dto;
    }
}
