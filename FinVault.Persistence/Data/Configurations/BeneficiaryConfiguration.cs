using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        builder.ToTable("Beneficiaries", t =>
        {
            t.HasCheckConstraint("CK_Beneficiaries_BeneficiaryType", "[BeneficiaryType] IN ('IntraBank','InterBank','UPI')");
            t.HasCheckConstraint("CK_Beneficiaries_Status", "[Status] IN ('Pending','Active','Blocked','Removed')");
            t.HasCheckConstraint("CK_Beneficiaries_PayeeIdentifier", "([BeneficiaryType] = 'UPI' AND [UpiVpa] IS NOT NULL) OR ([BeneficiaryType] <> 'UPI' AND [AccountNumber] IS NOT NULL AND [IfscCode] IS NOT NULL)");
        });

        // Primary key
        builder.HasKey(e => e.BeneficiaryId);

        builder.Property(e => e.BeneficiaryId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.BeneficiaryName)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(e => e.Nickname)
            .HasMaxLength(50);
        builder.Property(e => e.BeneficiaryType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.AccountNumber)
            .HasMaxLength(20);
        builder.Property(e => e.IfscCode)
            .HasMaxLength(11);
        builder.Property(e => e.BankName)
            .HasMaxLength(150);
        builder.Property(e => e.BranchName)
            .HasMaxLength(150);
        builder.Property(e => e.UpiVpa)
            .HasMaxLength(100);
        builder.Property(e => e.MobileNumber)
            .HasMaxLength(15);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.PerTransactionLimit)
            .HasPrecision(18, 2);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => new { e.CustomerId, e.Status })
            .HasDatabaseName("IX_Beneficiaries_CustomerId_Status");

        builder.HasIndex(e => new { e.CustomerId, e.AccountNumber, e.IfscCode })
            .HasDatabaseName("UX_Beneficiaries_Customer_Account_Ifsc")
            .IsUnique()
            .HasFilter("[Status] <> 'Removed' AND [AccountNumber] IS NOT NULL AND [IfscCode] IS NOT NULL");

        builder.HasIndex(e => new { e.CustomerId, e.UpiVpa })
            .HasDatabaseName("UX_Beneficiaries_Customer_Vpa")
            .IsUnique()
            .HasFilter("[Status] <> 'Removed' AND [UpiVpa] IS NOT NULL");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.Beneficiaries)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InternalAccount)
            .WithMany(p => p.BeneficiaryReferences)
            .HasForeignKey(e => e.InternalAccountId)
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
