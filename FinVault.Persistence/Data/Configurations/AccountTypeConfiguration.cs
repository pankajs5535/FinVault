using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.ToTable("AccountTypes", t =>
        {
            t.HasCheckConstraint("CK_AccountTypes_Category", "[Category] IN ('Savings','Current','FixedDeposit','RecurringDeposit','Internal')");
            t.HasCheckConstraint("CK_AccountTypes_InterestPayoutFrequency", "[InterestPayoutFrequency] IN ('Monthly','Quarterly','HalfYearly','Yearly','AtMaturity')");
        });

        // Primary key
        builder.HasKey(e => e.AccountTypeId);

        builder.Property(e => e.AccountTypeId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Description)
            .HasMaxLength(500);
        builder.Property(e => e.MinimumBalance)
            .HasPrecision(18, 2);
        builder.Property(e => e.MinimumAverageBalance)
            .HasPrecision(18, 2);
        builder.Property(e => e.DailyTransactionLimit)
            .HasPrecision(18, 2);
        builder.Property(e => e.InterestPayoutFrequency)
            .HasMaxLength(20);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.Code)
            .HasDatabaseName("UX_AccountTypes_Code")
            .IsUnique();

        builder.HasIndex(e => e.Category)
            .HasDatabaseName("IX_AccountTypes_Category");

        // Relationships
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
