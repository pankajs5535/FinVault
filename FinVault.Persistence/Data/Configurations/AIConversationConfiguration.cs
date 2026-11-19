using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AIConversationConfiguration : IEntityTypeConfiguration<AIConversation>
{
    public void Configure(EntityTypeBuilder<AIConversation> builder)
    {
        builder.ToTable("AIConversations", t =>
        {
            t.HasCheckConstraint("CK_AIConversations_Channel", "[Channel] IN ('Web','Mobile','Voice')");
            t.HasCheckConstraint("CK_AIConversations_Status", "[Status] IN ('Active','Closed','Archived','Escalated')");
        });

        // Primary key
        builder.HasKey(e => e.AIConversationId);

        builder.Property(e => e.AIConversationId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.ConversationReference)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(e => e.Title)
            .HasMaxLength(200);
        builder.Property(e => e.Channel)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Language)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Summary)
            .HasMaxLength(1000);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(15);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.ConversationReference)
            .HasDatabaseName("UX_AIConversations_ConversationReference")
            .IsUnique();

        builder.HasIndex(e => new { e.UserId, e.StartedAt })
            .HasDatabaseName("IX_AIConversations_UserId_StartedAt");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_AIConversations_Status");

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(p => p.AIConversations)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
            .WithMany(p => p.AIConversations)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.EscalatedToEmployee)
            .WithMany(p => p.EscalatedConversations)
            .HasForeignKey(e => e.EscalatedToEmployeeId)
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
