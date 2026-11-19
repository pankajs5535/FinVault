using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates", t =>
        {
            t.HasCheckConstraint("CK_NotificationTemplates_Channel", "[Channel] IN ('Sms','Email','Push','InApp','WhatsApp')");
            t.HasCheckConstraint("CK_NotificationTemplates_Category", "[Category] IN ('Transactional','Security','Promotional','Regulatory','Reminder')");
        });

        // Primary key
        builder.HasKey(e => e.NotificationTemplateId);

        builder.Property(e => e.NotificationTemplateId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.TemplateCode)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Channel)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.Language)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Subject)
            .HasMaxLength(200);
        builder.Property(e => e.BodyTemplate)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.DltTemplateId)
            .HasMaxLength(30);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.TemplateCode, e.Channel, e.Language })
            .HasDatabaseName("UX_NotificationTemplates_TemplateCode_Channel_Language")
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
