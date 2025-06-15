using MediatR;
using MeritJournal.Application.DTOs;
using MeritJournal.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Constructor for GetJournalEntriesQueryHandler.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public GetJournalEntriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the GetJournalEntriesQuery.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of journal entries as DTOs.</returns>
    public async Task<List<JournalEntryDto>> Handle(GetJournalEntriesQuery request, CancellationToken cancellationToken)
    {
        // Get all journal entries for the current user
        var journalEntries = await _context.JournalEntries
            .Include(j => j.Images)
            .Include(j => j.JournalEntryTags)
                .ThenInclude(jet => jet.Tag)
            .Where(j => j.UserId == request.UserId)
            .OrderByDescending(j => j.EntryDate)
            .ToListAsync(cancellationToken);

        // Convert to DTOs
        var result = journalEntries.Select(entry => new JournalEntryDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.Content,
            CreatedAt = entry.CreatedAt,
            ModifiedAt = entry.ModifiedAt,
            EntryDate = entry.EntryDate,
            Images = entry.Images.Select(image => new JournalImageDto
            {
                Id = image.Id,
                ImageDataBase64 = Convert.ToBase64String(image.ImageData),
                ContentType = image.ContentType,
                Caption = image.Caption,
                JournalEntryId = image.JournalEntryId
            }).ToList(),
            Tags = entry.JournalEntryTags.Select(jet => jet.Tag!.Name).ToList()
        }).ToList();

        return result;
    }
}
