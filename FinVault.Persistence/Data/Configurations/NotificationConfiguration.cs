using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", t =>
        {
            t.HasCheckConstraint("CK_Notifications_Channel", "[Channel] IN ('Sms','Email','Push','InApp','WhatsApp')");
            t.HasCheckConstraint("CK_Notifications_Priority", "[Priority] IN ('Low','Normal','High','Urgent')");
            t.HasCheckConstraint("CK_Notifications_Status", "[Status] IN ('Pending','Queued','Sent','Delivered','Failed','Read')");
            t.HasCheckConstraint("CK_Notifications_Recipient", "[CustomerId] IS NOT NULL OR [UserId] IS NOT NULL");
        });

        // Primary key
        builder.HasKey(e => e.NotificationId);

        builder.Property(e => e.NotificationId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Channel)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.Priority)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.RecipientAddress)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(e => e.Subject)
            .HasMaxLength(200);
        builder.Property(e => e.Body)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.ProviderMessageId)
            .HasMaxLength(100);
        builder.Property(e => e.FailureReason)
            .HasMaxLength(500);
        builder.Property(e => e.RelatedEntityType)
            .HasMaxLength(50);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.CustomerId, e.CreatedAt })
            .HasDatabaseName("IX_Notifications_CustomerId_CreatedAt");

        builder.HasIndex(e => new { e.UserId, e.Status })
            .HasDatabaseName("IX_Notifications_UserId_Status");

        builder.HasIndex(e => new { e.Status, e.ScheduledAt })
            .HasDatabaseName("IX_Notifications_Status_ScheduledAt");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.Notifications)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User)
            .WithMany(p => p.Notifications)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.NotificationTemplate)
            .WithMany(p => p.Notifications)
            .HasForeignKey(e => e.NotificationTemplateId)
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
