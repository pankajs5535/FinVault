using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds", t =>
        {
            t.HasCheckConstraint("CK_Refunds_RefundType", "[RefundType] IN ('Full','Partial')");
            t.HasCheckConstraint("CK_Refunds_Status", "[Status] IN ('Requested','Approved','Rejected','Processed','Failed')");
            t.HasCheckConstraint("CK_Refunds_Amount", "[Amount] > 0");
        });

        // Primary key
        builder.HasKey(e => e.RefundId);

        builder.Property(e => e.RefundId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.RefundReference)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(e => e.RefundType)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2);
        builder.Property(e => e.Reason)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.RejectionReason)
            .HasMaxLength(250);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.RefundReference)
            .HasDatabaseName("UX_Refunds_RefundReference")
            .IsUnique();

        builder.HasIndex(e => new { e.OriginalTransactionId, e.Status })
            .HasDatabaseName("IX_Refunds_OriginalTransactionId_Status");

        // Relationships
        builder.HasOne(e => e.OriginalTransaction)
            .WithMany(p => p.Refunds)
            .HasForeignKey(e => e.OriginalTransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RefundTransaction)
            .WithOne(p => p.SourceRefund)
            .HasForeignKey<Refund>(e => e.RefundTransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Dispute)
            .WithMany(p => p.Refunds)
            .HasForeignKey(e => e.DisputeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RequestedByUser)
            .WithMany()
            .HasForeignKey(e => e.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ApprovedByUser)
            .WithMany()
            .HasForeignKey(e => e.ApprovedByUserId)
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
