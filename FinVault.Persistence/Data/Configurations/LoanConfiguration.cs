using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans", t =>
        {
            t.HasCheckConstraint("CK_Loans_LoanType", "[LoanType] IN ('Personal','Home','Vehicle','Education','Business','Gold','AgainstDeposit')");
            t.HasCheckConstraint("CK_Loans_Status", "[Status] IN ('Applied','Approved','Rejected','Disbursed','Active','Closed','WrittenOff')");
            t.HasCheckConstraint("CK_Loans_InterestType", "[InterestType] IN ('Fixed','Floating')");
            t.HasCheckConstraint("CK_Loans_AssetClassification", "[AssetClassification] IN ('Standard','SMA0','SMA1','SMA2','SubStandard','Doubtful','Loss')");
            t.HasCheckConstraint("CK_Loans_Amounts", "[RequestedAmount] > 0 AND [DisbursedAmount] >= 0 AND [OutstandingPrincipal] >= 0");
        });

        // Primary key
        builder.HasKey(e => e.LoanId);

        builder.Property(e => e.LoanId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.LoanNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.LoanType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Purpose)
            .HasMaxLength(250);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.RequestedAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.SanctionedAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.DisbursedAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.OutstandingPrincipal)
            .HasPrecision(18, 2);
        builder.Property(e => e.OutstandingInterest)
            .HasPrecision(18, 2);
        builder.Property(e => e.EmiAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.AnnualInterestRate)
            .HasPrecision(9, 4);
        builder.Property(e => e.InterestType)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.ApplicationDate)
            .HasColumnType("date");
        builder.Property(e => e.SanctionDate)
            .HasColumnType("date");
        builder.Property(e => e.DisbursementDate)
            .HasColumnType("date");
        builder.Property(e => e.FirstEmiDate)
            .HasColumnType("date");
        builder.Property(e => e.MaturityDate)
            .HasColumnType("date");
        builder.Property(e => e.ClosedOn)
            .HasColumnType("date");
        builder.Property(e => e.AssetClassification)
            .IsRequired()
            .HasMaxLength(15);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.LoanNumber)
            .HasDatabaseName("UX_Loans_LoanNumber")
            .IsUnique();

        builder.HasIndex(e => new { e.CustomerId, e.Status })
            .HasDatabaseName("IX_Loans_CustomerId_Status");

        builder.HasIndex(e => e.AssetClassification)
            .HasDatabaseName("IX_Loans_AssetClassification");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.Loans)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Branch)
            .WithMany(p => p.Loans)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DisbursementAccount)
            .WithMany(p => p.DisbursementLoans)
            .HasForeignKey(e => e.DisbursementAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RepaymentAccount)
            .WithMany(p => p.RepaymentLoans)
            .HasForeignKey(e => e.RepaymentAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InterestRate)
            .WithMany(p => p.Loans)
            .HasForeignKey(e => e.InterestRateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ApprovedByEmployee)
            .WithMany(p => p.ApprovedLoans)
            .HasForeignKey(e => e.ApprovedByEmployeeId)
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
