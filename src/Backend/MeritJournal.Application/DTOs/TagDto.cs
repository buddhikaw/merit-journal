namespace MeritJournal.Application.DTOs;

/// <summary>
/// Data Transfer Object for tags.
/// </summary>
public class TagDto
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
