namespace MeritJournal.Application.DTOs;

/// <summary>
/// Data Transfer Object for journal entries.
/// </summary>
public class JournalEntryDto
{
    /// <summary>
    /// The ID of the journal entry.
    /// </summary>
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
    /// </summary>
    public DateTime EntryDate { get; set; }
    
    /// <summary>
    /// The list of image IDs associated with this journal entry.
    /// </summary>
    public List<JournalImageDto>? Images { get; set; }
    
    /// <summary>
    /// The list of tags associated with this journal entry.
    /// </summary>
    public List<string>? Tags { get; set; }
}
