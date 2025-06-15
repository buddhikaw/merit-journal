namespace MeritJournal.Domain.Entities;

/// <summary>
/// Represents a tag that can be used to categorize journal entries.
/// </summary>
public class Tag
{
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The user ID from the external OIDC provider (e.g., 'sub' claim).
    /// Tags are specific to each user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// The collection of journal entries associated with this tag.
    /// </summary>
    public ICollection<JournalEntryTag> JournalEntryTags { get; set; } = new List<JournalEntryTag>();
}
