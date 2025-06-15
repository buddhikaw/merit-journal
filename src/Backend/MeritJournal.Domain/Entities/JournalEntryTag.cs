namespace MeritJournal.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between JournalEntry and Tag entities.
/// </summary>
public class JournalEntryTag
{
    /// <summary>
    /// The ID of the journal entry.
    /// </summary>
    public int JournalEntryId { get; set; }
    
    /// <summary>
    /// The journal entry associated with this relationship.
    /// </summary>
    public JournalEntry? JournalEntry { get; set; }
    
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    public int TagId { get; set; }
    
    /// <summary>
    /// The tag associated with this relationship.
    /// </summary>
    public Tag? Tag { get; set; }
}
