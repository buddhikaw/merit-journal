namespace MeritJournal.Domain.Entities;

/// <summary>
/// Represents an image associated with a journal entry.
/// </summary>
public class JournalImage
{
    public int Id { get; set; }
    
    /// <summary>
    /// The binary data of the image.
    /// </summary>
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    
    /// <summary>
    /// The MIME type of the image (e.g., "image/jpeg", "image/png").
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>
    /// An optional caption or description for the image.
    /// </summary>
    public string? Caption { get; set; }
    
    /// <summary>
    /// The ID of the journal entry this image is associated with.
    /// </summary>
    public int JournalEntryId { get; set; }
    
    /// <summary>
    /// The journal entry this image is associated with.
    /// </summary>
    public JournalEntry? JournalEntry { get; set; }
}
