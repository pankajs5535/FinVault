using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class ReconciliationRecordConfiguration : IEntityTypeConfiguration<ReconciliationRecord>
{
    public void Configure(EntityTypeBuilder<ReconciliationRecord> builder)
    {
        builder.ToTable("ReconciliationRecords", t =>
        {
            t.HasCheckConstraint("CK_ReconciliationRecords_MatchStatus", "[MatchStatus] IN ('Matched','Unmatched','AmountMismatch','MissingInBank','MissingInExternal','Manual')");
        });

        // Primary key
        builder.HasKey(e => e.ReconciliationRecordId);

        builder.Property(e => e.ReconciliationRecordId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.ExternalReference)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.ExternalAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.ExternalDate)
            .HasColumnType("date");
        builder.Property(e => e.InternalAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.DifferenceAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.MatchStatus)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.ExceptionReason)
            .HasMaxLength(250);
        builder.Property(e => e.ResolutionNotes)
            .HasMaxLength(500);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.ReconciliationBatchId, e.MatchStatus })
            .HasDatabaseName("IX_ReconciliationRecords_ReconciliationBatchId_MatchStatus");

        builder.HasIndex(e => e.ExternalReference)
            .HasDatabaseName("IX_ReconciliationRecords_ExternalReference");

        // Relationships
        builder.HasOne(e => e.ReconciliationBatch)
            .WithMany(p => p.ReconciliationRecords)
            .HasForeignKey(e => e.ReconciliationBatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.ReconciliationRecords)
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ResolvedByUser)
            .WithMany()
            .HasForeignKey(e => e.ResolvedByUserId)
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
