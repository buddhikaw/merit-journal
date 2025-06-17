using MeritJournal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeritJournal.Infrastructure.Persistence;

/// <summary>
/// Implementation of the application's database context.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor for the ApplicationDbContext.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet for JournalEntry entities.
    /// </summary>
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    
    /// <summary>
    /// DbSet for JournalImage entities.
    /// </summary>
    public DbSet<JournalImage> JournalImages => Set<JournalImage>();
    
    /// <summary>
    /// DbSet for Tag entities.
    /// </summary>
    public DbSet<Tag> Tags => Set<Tag>();
    
    /// <summary>
    /// DbSet for JournalEntryTag entities.
    /// </summary>    public DbSet<JournalEntryTag> JournalEntryTags => Set<JournalEntryTag>();

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the JournalEntryTag entity as a many-to-many relationship
        modelBuilder.Entity<JournalEntryTag>()
            .HasKey(jet => new { jet.JournalEntryId, jet.TagId });
        
        modelBuilder.Entity<JournalEntryTag>()
            .HasOne(jet => jet.JournalEntry)
            .WithMany(j => j.JournalEntryTags)
            .HasForeignKey(jet => jet.JournalEntryId);
        
        modelBuilder.Entity<JournalEntryTag>()
            .HasOne(jet => jet.Tag)
            .WithMany(t => t.JournalEntryTags)
            .HasForeignKey(jet => jet.TagId);
        
        // Configure the JournalImage entity to have a one-to-many relationship with JournalEntry
        modelBuilder.Entity<JournalImage>()
            .HasOne(ji => ji.JournalEntry)
            .WithMany(j => j.Images)
            .HasForeignKey(ji => ji.JournalEntryId);
        
        base.OnModelCreating(modelBuilder);
    }
}
