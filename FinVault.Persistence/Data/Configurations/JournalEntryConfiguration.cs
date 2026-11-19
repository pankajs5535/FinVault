using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries", t =>
        {
            t.HasCheckConstraint("CK_JournalEntries_JournalType", "[JournalType] IN ('Transaction','Interest','Fee','Loan','Adjustment','Reversal')");
            t.HasCheckConstraint("CK_JournalEntries_Status", "[Status] IN ('Draft','Posted','Reversed')");
            t.HasCheckConstraint("CK_JournalEntries_Balanced", "[Status] <> 'Posted' OR ([TotalDebit] = [TotalCredit] AND [TotalDebit] > 0)");
        });

        // Primary key
        builder.HasKey(e => e.JournalEntryId);

        builder.Property(e => e.JournalEntryId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.JournalNumber)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.JournalType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.EntryDate)
            .HasColumnType("date");
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.TotalDebit)
            .HasPrecision(18, 2);
        builder.Property(e => e.TotalCredit)
            .HasPrecision(18, 2);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.JournalNumber)
            .HasDatabaseName("UX_JournalEntries_JournalNumber")
            .IsUnique();

        builder.HasIndex(e => new { e.EntryDate, e.Status })
            .HasDatabaseName("IX_JournalEntries_EntryDate_Status");

        // Relationships
        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.JournalEntries)
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PostedByUser)
            .WithMany()
            .HasForeignKey(e => e.PostedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ReversalOfJournalEntry)
            .WithMany(p => p.ReversalEntries)
            .HasForeignKey(e => e.ReversalOfJournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UpdatedByUser)
            .WithMany()
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
