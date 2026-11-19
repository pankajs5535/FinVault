using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses", t =>
        {
            t.HasCheckConstraint("CK_Addresses_AddressType", "[AddressType] IN ('Residential','Permanent','Correspondence','Office','Registered')");
            t.HasCheckConstraint("CK_Addresses_PinCode", "[PinCode] LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]'");
        });

        // Primary key
        builder.HasKey(e => e.AddressId);

        builder.Property(e => e.AddressId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.AddressType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.AddressLine2)
            .HasMaxLength(200);
        builder.Property(e => e.Landmark)
            .HasMaxLength(100);
        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.District)
            .HasMaxLength(100);
        builder.Property(e => e.State)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.PinCode)
            .IsRequired()
            .HasMaxLength(6);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.CustomerId, e.AddressType })
            .HasDatabaseName("IX_Addresses_CustomerId_AddressType");

        builder.HasIndex(e => e.PinCode)
            .HasDatabaseName("IX_Addresses_PinCode");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.Addresses)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Country)
            .WithMany(p => p.Addresses)
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
