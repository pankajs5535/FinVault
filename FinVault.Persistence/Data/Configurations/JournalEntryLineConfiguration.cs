using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines", t =>
        {
            t.HasCheckConstraint("CK_JournalEntryLines_OneSided", "([DebitAmount] > 0 AND [CreditAmount] = 0) OR ([CreditAmount] > 0 AND [DebitAmount] = 0)");
        });

        // Primary key
        builder.HasKey(e => e.JournalEntryLineId);

        builder.Property(e => e.JournalEntryLineId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.DebitAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.CreditAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.Narration)
            .HasMaxLength(250);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.JournalEntryId, e.LineNumber })
            .HasDatabaseName("UX_JournalEntryLines_JournalEntryId_LineNumber")
            .IsUnique();

        // Relationships
        builder.HasOne(e => e.JournalEntry)
            .WithMany(p => p.Lines)
            .HasForeignKey(e => e.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Account)
            .WithMany(p => p.JournalEntryLines)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
