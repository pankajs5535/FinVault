using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t =>
        {
            t.HasCheckConstraint("CK_Users_UserType", "[UserType] IN ('Customer','Employee','Admin','System')");
        });

        // Primary key
        builder.HasKey(e => e.UserId);

        builder.Property(e => e.UserId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15);
        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);
        builder.Property(e => e.SecurityStamp)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(e => e.UserType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.TwoFactorSecretEncrypted)
            .HasMaxLength(512);
        builder.Property(e => e.LastLoginIp)
            .HasMaxLength(45);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.Username)
            .HasDatabaseName("UX_Users_Username")
            .IsUnique();

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("UX_Users_Email")
            .IsUnique();

        builder.HasIndex(e => e.PhoneNumber)
            .HasDatabaseName("IX_Users_PhoneNumber");

        // Relationships
        builder.HasOne(e => e.Role)
            .WithMany(p => p.Users)
            .HasForeignKey(e => e.RoleId)
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
