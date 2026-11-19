using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        // Primary key
        builder.HasKey(e => e.RefreshTokenId);

        builder.Property(e => e.RefreshTokenId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.TokenHash)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(e => e.JwtId)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(e => e.RevocationReason)
            .HasMaxLength(250);
        builder.Property(e => e.RevokedByIp)
            .HasMaxLength(45);
        builder.Property(e => e.CreatedByIp)
            .IsRequired()
            .HasMaxLength(45);
        builder.Property(e => e.UserAgent)
            .HasMaxLength(512);
        builder.Property(e => e.DeviceName)
            .HasMaxLength(100);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.TokenHash)
            .HasDatabaseName("UX_RefreshTokens_TokenHash")
            .IsUnique();

        builder.HasIndex(e => e.JwtId)
            .HasDatabaseName("IX_RefreshTokens_JwtId");

        builder.HasIndex(e => new { e.UserId, e.ExpiresAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAt");

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(p => p.RefreshTokens)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.ReplacedByToken)
            .WithMany(p => p.ReplacedTokens)
            .HasForeignKey(e => e.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
