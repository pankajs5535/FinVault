using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        // Primary key
        builder.HasKey(e => e.PermissionId);

        builder.Property(e => e.PermissionId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.ModuleName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Description)
            .HasMaxLength(250);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.Code)
            .HasDatabaseName("UX_Permissions_Code")
            .IsUnique();

        builder.HasIndex(e => e.ModuleName)
            .HasDatabaseName("IX_Permissions_ModuleName");

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
