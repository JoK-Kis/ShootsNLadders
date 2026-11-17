using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShootsNLadders.Core.Entities;

namespace ShootsNLadders.Infrastructure.Data.Configurations;

/// <summary>
/// Describes the Entries table: keys, the one-entry-per-day rule and the rating range check.
/// </summary>
public sealed class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        // Database safety net for the 1–10 rule (the DTO validation is the first check).
        builder.ToTable("Entries", table => table.HasCheckConstraint(
            "CK_Entries_Ratings",
            $"Physical BETWEEN {Entry.MinRating} AND {Entry.MaxRating} " +
            $"AND Mental BETWEEN {Entry.MinRating} AND {Entry.MaxRating} " +
            $"AND Emotional BETWEEN {Entry.MinRating} AND {Entry.MaxRating}"));

        builder.HasKey(e => e.Id);

        // One entry per day. Becomes (UserId, Date) once accounts exist in step 5.
        builder.HasIndex(e => e.Date).IsUnique();

        // Note: SQLite does not enforce this length (see redflags.md); the DTO validation does.
        builder.Property(e => e.Note).HasMaxLength(Entry.NoteMaxLength);

        // Computed in C#, never stored.
        builder.Ignore(e => e.Total);
    }
}
