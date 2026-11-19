using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class InterestRateConfiguration : IEntityTypeConfiguration<InterestRate>
{
    public void Configure(EntityTypeBuilder<InterestRate> builder)
    {
        builder.ToTable("InterestRates", t =>
        {
            t.HasCheckConstraint("CK_InterestRates_ProductCategory", "[ProductCategory] IN ('Savings','Current','FixedDeposit','RecurringDeposit','Loan')");
            t.HasCheckConstraint("CK_InterestRates_CompoundingFrequency", "[CompoundingFrequency] IN ('Simple','Daily','Monthly','Quarterly','HalfYearly','Yearly')");
            t.HasCheckConstraint("CK_InterestRates_AnnualRate", "[AnnualRate] >= 0");
        });

        // Primary key
        builder.HasKey(e => e.InterestRateId);

        builder.Property(e => e.InterestRateId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.RateName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.ProductCategory)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.LoanType)
            .HasMaxLength(20);
        builder.Property(e => e.AnnualRate)
            .HasPrecision(9, 4);
        builder.Property(e => e.SeniorCitizenAdditionalRate)
            .HasPrecision(9, 4);
        builder.Property(e => e.CompoundingFrequency)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.MinAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.MaxAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.BenchmarkName)
            .HasMaxLength(50);
        builder.Property(e => e.SpreadOverBenchmark)
            .HasPrecision(9, 4);
        builder.Property(e => e.EffectiveFrom)
            .HasColumnType("date");
        builder.Property(e => e.EffectiveTo)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.AccountTypeId, e.EffectiveFrom })
            .HasDatabaseName("IX_InterestRates_AccountTypeId_EffectiveFrom");

        builder.HasIndex(e => new { e.ProductCategory, e.IsActive })
            .HasDatabaseName("IX_InterestRates_ProductCategory_IsActive");

        // Relationships
        builder.HasOne(e => e.AccountType)
            .WithMany(p => p.InterestRates)
            .HasForeignKey(e => e.AccountTypeId)
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
