using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", t =>
        {
            t.HasCheckConstraint("CK_AuditLogs_Outcome", "[Outcome] IN ('Success','Failure')");
        });

        // Primary key
        builder.HasKey(e => e.AuditLogId);

        builder.Property(e => e.AuditLogId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.EntityName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Outcome)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.OldValuesMasked)
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.NewValuesMasked)
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.IpAddress)
            .HasMaxLength(45);
        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);
        builder.Property(e => e.CorrelationId)
            .HasMaxLength(64);
        builder.Property(e => e.PreviousHash)
            .HasMaxLength(64);
        builder.Property(e => e.EntryHash)
            .HasMaxLength(64);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.EntityName, e.EntityId })
            .HasDatabaseName("IX_AuditLogs_EntityName_EntityId");

        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .HasDatabaseName("IX_AuditLogs_UserId_CreatedAt");

        builder.HasIndex(e => e.CorrelationId)
            .HasDatabaseName("IX_AuditLogs_CorrelationId");

        builder.HasIndex(e => new { e.IsSecuritySensitive, e.CreatedAt })
            .HasDatabaseName("IX_AuditLogs_IsSecuritySensitive_CreatedAt");

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(p => p.AuditLogs)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
