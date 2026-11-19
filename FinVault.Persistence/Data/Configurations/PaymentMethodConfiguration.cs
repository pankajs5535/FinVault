using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods", t =>
        {
            t.HasCheckConstraint("CK_PaymentMethods_Rail", "[Rail] IN ('UPI','NEFT','RTGS','IMPS','NACH','Cheque','Card','Internal')");
            t.HasCheckConstraint("CK_PaymentMethods_SettlementType", "[SettlementType] IN ('RealTime','DeferredNet','Batch')");
        });

        // Primary key
        builder.HasKey(e => e.PaymentMethodId);

        builder.Property(e => e.PaymentMethodId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Rail)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Description)
            .HasMaxLength(250);
        builder.Property(e => e.MinAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.MaxAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.DailyLimit)
            .HasPrecision(18, 2);
        builder.Property(e => e.SettlementType)
            .IsRequired()
            .HasMaxLength(20);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.Code)
            .HasDatabaseName("UX_PaymentMethods_Code")
            .IsUnique();

        builder.HasIndex(e => e.Rail)
            .HasDatabaseName("IX_PaymentMethods_Rail");

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
