using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class InterestCalculationConfiguration : IEntityTypeConfiguration<InterestCalculation>
{
    public void Configure(EntityTypeBuilder<InterestCalculation> builder)
    {
        builder.ToTable("InterestCalculations", t =>
        {
            t.HasCheckConstraint("CK_InterestCalculations_CalculationType", "[CalculationType] IN ('Accrual','Payout','Capitalisation')");
            t.HasCheckConstraint("CK_InterestCalculations_Status", "[Status] IN ('Calculated','Posted','Reversed')");
            t.HasCheckConstraint("CK_InterestCalculations_Subject", "([AccountId] IS NOT NULL AND [LoanId] IS NULL) OR ([AccountId] IS NULL AND [LoanId] IS NOT NULL)");
            t.HasCheckConstraint("CK_InterestCalculations_Period", "[PeriodEnd] >= [PeriodStart]");
        });

        // Primary key
        builder.HasKey(e => e.InterestCalculationId);

        builder.Property(e => e.InterestCalculationId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.CalculationType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.PeriodStart)
            .HasColumnType("date");
        builder.Property(e => e.PeriodEnd)
            .HasColumnType("date");
        builder.Property(e => e.PrincipalBase)
            .HasPrecision(18, 2);
        builder.Property(e => e.AppliedRate)
            .HasPrecision(9, 4);
        builder.Property(e => e.InterestAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.TdsAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.AccountId, e.PeriodStart, e.PeriodEnd, e.CalculationType })
            .HasDatabaseName("UX_InterestCalculations_Account_Period")
            .IsUnique()
            .HasFilter("[AccountId] IS NOT NULL AND [Status] <> 'Reversed'");

        builder.HasIndex(e => new { e.LoanId, e.PeriodStart, e.PeriodEnd, e.CalculationType })
            .HasDatabaseName("UX_InterestCalculations_Loan_Period")
            .IsUnique()
            .HasFilter("[LoanId] IS NOT NULL AND [Status] <> 'Reversed'");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_InterestCalculations_Status");

        // Relationships
        builder.HasOne(e => e.Account)
            .WithMany(p => p.InterestCalculations)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Loan)
            .WithMany(p => p.InterestCalculations)
            .HasForeignKey(e => e.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InterestRate)
            .WithMany(p => p.InterestCalculations)
            .HasForeignKey(e => e.InterestRateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.JournalEntry)
            .WithMany(p => p.InterestCalculations)
            .HasForeignKey(e => e.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.InterestCalculations)
            .HasForeignKey(e => e.TransactionId)
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
