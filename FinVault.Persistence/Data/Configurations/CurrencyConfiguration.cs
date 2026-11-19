using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");

        // Primary key
        builder.HasKey(e => e.CurrencyId);

        builder.Property(e => e.CurrencyId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);
        builder.Property(e => e.NumericCode)
            .IsRequired()
            .HasMaxLength(3);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Symbol)
            .IsRequired()
            .HasMaxLength(5);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.CurrencyCode)
            .HasDatabaseName("UX_Currencies_CurrencyCode")
            .IsUnique();

        builder.HasIndex(e => e.NumericCode)
            .HasDatabaseName("UX_Currencies_NumericCode")
            .IsUnique();

        // Relationships
        builder.HasOne(e => e.Country)
            .WithMany(p => p.Currencies)
            .HasForeignKey(e => e.CountryId)
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
