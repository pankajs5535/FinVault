using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class ComplianceRecordConfiguration : IEntityTypeConfiguration<ComplianceRecord>
{
    public void Configure(EntityTypeBuilder<ComplianceRecord> builder)
    {
        builder.ToTable("ComplianceRecords", t =>
        {
            t.HasCheckConstraint("CK_ComplianceRecords_RecordType", "[RecordType] IN ('Str','Ctr','Ccr','Ntr','AmlScreening','PepCheck','SanctionsScreening','PeriodicKycReview','FatcaCrs')");
            t.HasCheckConstraint("CK_ComplianceRecords_RiskLevel", "[RiskLevel] IN ('Low','Medium','High','Critical')");
            t.HasCheckConstraint("CK_ComplianceRecords_Status", "[Status] IN ('Open','UnderReview','Filed','Cleared','Closed')");
        });

        // Primary key
        builder.HasKey(e => e.ComplianceRecordId);

        builder.Property(e => e.ComplianceRecordId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(e => e.RecordType)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(e => e.RiskLevel)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.ReportingAuthority)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.FilingReference)
            .HasMaxLength(50);
        builder.Property(e => e.DueDate)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.ReferenceNumber)
            .HasDatabaseName("UX_ComplianceRecords_ReferenceNumber")
            .IsUnique();

        builder.HasIndex(e => new { e.RecordType, e.Status })
            .HasDatabaseName("IX_ComplianceRecords_RecordType_Status");

        builder.HasIndex(e => e.DueDate)
            .HasDatabaseName("IX_ComplianceRecords_DueDate");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.ComplianceRecords)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Account)
            .WithMany(p => p.ComplianceRecords)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.ComplianceRecords)
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.FraudAlert)
            .WithMany(p => p.ComplianceRecords)
            .HasForeignKey(e => e.FraudAlertId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.FiledByEmployee)
            .WithMany(p => p.FiledComplianceRecords)
            .HasForeignKey(e => e.FiledByEmployeeId)
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
