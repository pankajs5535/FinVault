using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class ReconciliationBatchConfiguration : IEntityTypeConfiguration<ReconciliationBatch>
{
    public void Configure(EntityTypeBuilder<ReconciliationBatch> builder)
    {
        builder.ToTable("ReconciliationBatches", t =>
        {
            t.HasCheckConstraint("CK_ReconciliationBatches_ReconciliationType", "[ReconciliationType] IN ('Upi','Neft','Rtgs','Imps','CardSettlement','AtmSwitch','GeneralLedger')");
            t.HasCheckConstraint("CK_ReconciliationBatches_Status", "[Status] IN ('Pending','InProgress','Completed','CompletedWithExceptions','Failed')");
        });

        // Primary key
        builder.HasKey(e => e.ReconciliationBatchId);

        builder.Property(e => e.ReconciliationBatchId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.BatchNumber)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.ReconciliationType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.BusinessDate)
            .HasColumnType("date");
        builder.Property(e => e.SourceName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.FileName)
            .HasMaxLength(260);
        builder.Property(e => e.FileHash)
            .HasMaxLength(64);
        builder.Property(e => e.TotalAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.MatchedAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.DifferenceAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(30);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.BatchNumber)
            .HasDatabaseName("UX_ReconciliationBatches_BatchNumber")
            .IsUnique();

        builder.HasIndex(e => e.FileHash)
            .HasDatabaseName("UX_ReconciliationBatches_FileHash")
            .IsUnique()
            .HasFilter("[FileHash] IS NOT NULL");

        builder.HasIndex(e => new { e.ReconciliationType, e.BusinessDate })
            .HasDatabaseName("IX_ReconciliationBatches_ReconciliationType_BusinessDate");

        // Relationships
        builder.HasOne(e => e.PaymentMethod)
            .WithMany(p => p.ReconciliationBatches)
            .HasForeignKey(e => e.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InitiatedByUser)
            .WithMany()
            .HasForeignKey(e => e.InitiatedByUserId)
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
