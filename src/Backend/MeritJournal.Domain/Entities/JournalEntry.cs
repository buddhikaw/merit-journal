namespace MeritJournal.Domain.Entities;

/// <summary>
/// Represents a journal entry in the Merit Journal application.
/// </summary>
public class JournalEntry
{
    public int Id { get; set; }
    
    /// <summary>
    /// The title of the journal entry.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The content of the journal entry in HTML format.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// The date when the journal entry was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The date when the journal entry was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
    
    /// <summary>
    /// The date to which this journal entry pertains. 
    /// This represents the day for which meritorious acts are being recorded.
    /// </summary>
    public DateTime EntryDate { get; set; }
    
    /// <summary>
    /// The user ID from the external OIDC provider (e.g., 'sub' claim).
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// The collection of images associated with this journal entry.
    /// </summary>
    public ICollection<JournalImage> Images { get; set; } = new List<JournalImage>();
    
    /// <summary>
    /// The collection of tags associated with this journal entry.
    /// </summary>
    public ICollection<JournalEntryTag> JournalEntryTags { get; set; } = new List<JournalEntryTag>();
}
