using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class FeeConfiguration : IEntityTypeConfiguration<Fee>
{
    public void Configure(EntityTypeBuilder<Fee> builder)
    {
        builder.ToTable("Fees", t =>
        {
            t.HasCheckConstraint("CK_Fees_CalculationMethod", "[CalculationMethod] IN ('Flat','Percentage','Slab')");
            t.HasCheckConstraint("CK_Fees_Frequency", "[Frequency] IN ('PerTransaction','Monthly','Quarterly','Yearly','OneTime')");
        });

        // Primary key
        builder.HasKey(e => e.FeeId);

        builder.Property(e => e.FeeId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.CalculationMethod)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.Frequency)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.FlatAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.Percentage)
            .HasPrecision(9, 4);
        builder.Property(e => e.MinimumAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.MaximumAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.GstRate)
            .HasPrecision(5, 2);
        builder.Property(e => e.EffectiveFrom)
            .HasColumnType("date");
        builder.Property(e => e.EffectiveTo)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.Code)
            .HasDatabaseName("UX_Fees_Code")
            .IsUnique();

        builder.HasIndex(e => new { e.IsActive, e.EffectiveFrom })
            .HasDatabaseName("IX_Fees_IsActive_EffectiveFrom");

        // Relationships
        builder.HasOne(e => e.TransactionType)
            .WithMany(p => p.Fees)
            .HasForeignKey(e => e.TransactionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PaymentMethod)
            .WithMany(p => p.Fees)
            .HasForeignKey(e => e.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AccountType)
            .WithMany(p => p.Fees)
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
