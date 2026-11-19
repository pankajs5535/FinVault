using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
{
    public void Configure(EntityTypeBuilder<TransactionType> builder)
    {
        builder.ToTable("TransactionTypes", t =>
        {
            t.HasCheckConstraint("CK_TransactionTypes_Category", "[Category] IN ('Deposit','Withdrawal','Transfer','Payment','Fee','Interest','Refund','Reversal','Loan','Card')");
        });

        // Primary key
        builder.HasKey(e => e.TransactionTypeId);

        builder.Property(e => e.TransactionTypeId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Description)
            .HasMaxLength(250);
        builder.Property(e => e.ApprovalThreshold)
            .HasPrecision(18, 2);
        builder.Property(e => e.DailyLimit)
            .HasPrecision(18, 2);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.Code)
            .HasDatabaseName("UX_TransactionTypes_Code")
            .IsUnique();

        builder.HasIndex(e => e.Category)
            .HasDatabaseName("IX_TransactionTypes_Category");

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
