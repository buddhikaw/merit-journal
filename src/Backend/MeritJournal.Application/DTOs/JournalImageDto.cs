namespace MeritJournal.Application.DTOs;

/// <summary>
/// Data Transfer Object for journal images.
/// </summary>
public class JournalImageDto
{
    /// <summary>
    /// The ID of the image.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The binary data of the image encoded as a Base64 string.
    /// </summary>
    public string ImageDataBase64 { get; set; } = string.Empty;
    
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
}
