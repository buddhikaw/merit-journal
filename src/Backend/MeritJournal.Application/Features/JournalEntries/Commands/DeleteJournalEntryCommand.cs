using MediatR;
using MeritJournal.Application.Interfaces;

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
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteJournalEntryCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public DeleteJournalEntryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Handles the DeleteJournalEntryCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task Handle(DeleteJournalEntryCommand request, CancellationToken cancellationToken)
    {
        // Begin transaction
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Get the journal entry
            var journalEntry = await _unitOfWork.JournalEntries
                .FirstOrDefaultAsync(je => je.Id == request.JournalEntryId && je.UserId == request.UserId);
            
            if (journalEntry == null)
            {
                throw new KeyNotFoundException($"Journal entry with ID {request.JournalEntryId} not found for user {request.UserId}");
            }
            
            // Get related data to clean up
            var journalEntryTags = _unitOfWork.JournalEntryTags
                .Find(jet => jet.JournalEntryId == request.JournalEntryId)
                .ToList();
                
            var journalImages = _unitOfWork.JournalImages
                .Find(ji => ji.JournalEntryId == request.JournalEntryId)
                .ToList();
                
            // Remove related data first
            foreach (var tag in journalEntryTags)
            {
                _unitOfWork.JournalEntryTags.Remove(tag);
            }
            
            foreach (var image in journalImages)
            {
                _unitOfWork.JournalImages.Remove(image);
            }
            
            // Remove the journal entry
            _unitOfWork.JournalEntries.Remove(journalEntry);
            
            // Save changes and commit transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
