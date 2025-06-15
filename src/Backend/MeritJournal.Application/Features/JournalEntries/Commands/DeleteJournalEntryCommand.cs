using MediatR;
using MeritJournal.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeritJournal.Application.Features.JournalEntries.Commands;

/// <summary>
/// Command to delete a journal entry.
/// </summary>
public class DeleteJournalEntryCommand : IRequest
{
    /// <summary>
    /// The ID of the journal entry to delete.
    /// </summary>
    public int JournalEntryId { get; }
    
    /// <summary>
    /// The user ID from the external OIDC provider (e.g., 'sub' claim).
    /// </summary>
    public string UserId { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteJournalEntryCommand"/> class.
    /// </summary>
    /// <param name="journalEntryId">The ID of the journal entry to delete.</param>
    /// <param name="userId">The user's ID.</param>
    public DeleteJournalEntryCommand(int journalEntryId, string userId)
    {
        JournalEntryId = journalEntryId;
        UserId = userId;
    }
}

/// <summary>
/// Handler for deleting a journal entry.
/// </summary>
public class DeleteJournalEntryCommandHandler : IRequestHandler<DeleteJournalEntryCommand>
{
    private readonly IApplicationDbContext _context;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteJournalEntryCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The application DB context.</param>
    public DeleteJournalEntryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Handles the DeleteJournalEntryCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task Handle(DeleteJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var journalEntry = await _context.JournalEntries
            .FirstOrDefaultAsync(je => je.Id == request.JournalEntryId && je.UserId == request.UserId, cancellationToken);
        
        if (journalEntry == null)
        {
            throw new KeyNotFoundException($"Journal entry with ID {request.JournalEntryId} not found for user {request.UserId}");
        }
        
        _context.JournalEntries.Remove(journalEntry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
