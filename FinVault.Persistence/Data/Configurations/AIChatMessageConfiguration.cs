using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AIChatMessageConfiguration : IEntityTypeConfiguration<AIChatMessage>
{
    public void Configure(EntityTypeBuilder<AIChatMessage> builder)
    {
        builder.ToTable("AIChatMessages", t =>
        {
            t.HasCheckConstraint("CK_AIChatMessages_SenderRole", "[SenderRole] IN ('System','User','Assistant','Tool')");
        });

        // Primary key
        builder.HasKey(e => e.AIChatMessageId);

        builder.Property(e => e.AIChatMessageId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.SenderRole)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.ModelName)
            .HasMaxLength(100);
        builder.Property(e => e.FinishReason)
            .HasMaxLength(30);
        builder.Property(e => e.FlagReason)
            .HasMaxLength(250);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.AIConversationId, e.SequenceNumber })
            .HasDatabaseName("UX_AIChatMessages_AIConversationId_SequenceNumber")
            .IsUnique();

        // Relationships
        builder.HasOne(e => e.AIConversation)
            .WithMany(p => p.AIChatMessages)
            .HasForeignKey(e => e.AIConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
