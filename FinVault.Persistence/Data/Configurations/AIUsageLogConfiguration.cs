using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AIUsageLogConfiguration : IEntityTypeConfiguration<AIUsageLog>
{
    public void Configure(EntityTypeBuilder<AIUsageLog> builder)
    {
        builder.ToTable("AIUsageLogs");

        // Primary key
        builder.HasKey(e => e.AIUsageLogId);

        builder.Property(e => e.AIUsageLogId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.FeatureName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Provider)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.ModelName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.RequestId)
            .HasMaxLength(64);
        builder.Property(e => e.ErrorCode)
            .HasMaxLength(50);
        builder.Property(e => e.EstimatedCostInr)
            .HasPrecision(18, 6);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .HasDatabaseName("IX_AIUsageLogs_UserId_CreatedAt");

        builder.HasIndex(e => new { e.FeatureName, e.CreatedAt })
            .HasDatabaseName("IX_AIUsageLogs_FeatureName_CreatedAt");

        builder.HasIndex(e => e.RequestId)
            .HasDatabaseName("IX_AIUsageLogs_RequestId");

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(p => p.AIUsageLogs)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AIConversation)
            .WithMany(p => p.AIUsageLogs)
            .HasForeignKey(e => e.AIConversationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AIChatMessage)
            .WithMany(p => p.AIUsageLogs)
            .HasForeignKey(e => e.AIChatMessageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
