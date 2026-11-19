using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class KYCDetailsConfiguration : IEntityTypeConfiguration<KYCDetails>
{
    public void Configure(EntityTypeBuilder<KYCDetails> builder)
    {
        builder.ToTable("KYCDetails", t =>
        {
            t.HasCheckConstraint("CK_KYCDetails_KycLevel", "[KycLevel] IN ('Minimum','Full','Video','EKyc')");
            t.HasCheckConstraint("CK_KYCDetails_VerificationStatus", "[VerificationStatus] IN ('Pending','InProgress','Verified','Rejected','Expired')");
            t.HasCheckConstraint("CK_KYCDetails_AadhaarVerificationMethod", "[AadhaarVerificationMethod] IN ('OtpEkyc','BiometricEkyc','OfflineXml','VirtualId')");
            t.HasCheckConstraint("CK_KYCDetails_CkycStatus", "[CkycStatus] IN ('NotInitiated','Pending','Uploaded','Verified','Rejected')");
        });

        // Primary key
        builder.HasKey(e => e.KYCDetailsId);

        builder.Property(e => e.KYCDetailsId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.KycLevel)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.VerificationStatus)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500);
        builder.Property(e => e.NextReKycDueDate)
            .HasColumnType("date");
        builder.Property(e => e.PanEncrypted)
            .HasMaxLength(512);
        builder.Property(e => e.PanHash)
            .HasMaxLength(64);
        builder.Property(e => e.PanMasked)
            .HasMaxLength(10);
        builder.Property(e => e.AadhaarVaultReference)
            .HasMaxLength(128);
        builder.Property(e => e.AadhaarLast4)
            .HasMaxLength(4);
        builder.Property(e => e.AadhaarVerificationMethod)
            .HasMaxLength(20);
        builder.Property(e => e.CkycNumber)
            .HasMaxLength(14);
        builder.Property(e => e.CkycStatus)
            .IsRequired()
            .HasMaxLength(20);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.CustomerId)
            .HasDatabaseName("UX_KYCDetails_CustomerId_Current")
            .IsUnique()
            .HasFilter("[IsCurrent] = 1");

        builder.HasIndex(e => e.PanHash)
            .HasDatabaseName("UX_KYCDetails_PanHash_Current")
            .IsUnique()
            .HasFilter("[IsCurrent] = 1 AND [PanHash] IS NOT NULL");

        builder.HasIndex(e => e.CkycNumber)
            .HasDatabaseName("IX_KYCDetails_CkycNumber");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.KYCDetailsHistory)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.VerifiedByEmployee)
            .WithMany(p => p.VerifiedKYCRecords)
            .HasForeignKey(e => e.VerifiedByEmployeeId)
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
