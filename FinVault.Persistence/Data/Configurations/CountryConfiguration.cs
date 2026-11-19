using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");

        // Primary key
        builder.HasKey(e => e.CountryId);

        builder.Property(e => e.CountryId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.IsoAlpha2Code)
            .IsRequired()
            .HasMaxLength(2);
        builder.Property(e => e.IsoAlpha3Code)
            .IsRequired()
            .HasMaxLength(3);
        builder.Property(e => e.NumericCode)
            .IsRequired()
            .HasMaxLength(3);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.PhoneCode)
            .HasMaxLength(6);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.IsoAlpha2Code)
            .HasDatabaseName("UX_Countries_IsoAlpha2Code")
            .IsUnique();

        builder.HasIndex(e => e.IsoAlpha3Code)
            .HasDatabaseName("UX_Countries_IsoAlpha3Code")
            .IsUnique();

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("UX_Countries_Name")
            .IsUnique();

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
