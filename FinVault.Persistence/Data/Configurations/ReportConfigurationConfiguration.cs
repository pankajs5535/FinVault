using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class ReportConfigurationConfiguration : IEntityTypeConfiguration<ReportConfiguration>
{
    public void Configure(EntityTypeBuilder<ReportConfiguration> builder)
    {
        builder.ToTable("ReportConfigurations", t =>
        {
            t.HasCheckConstraint("CK_ReportConfigurations_ReportType", "[ReportType] IN ('Statement','Regulatory','Operational','Financial','Analytics')");
            t.HasCheckConstraint("CK_ReportConfigurations_OutputFormat", "[OutputFormat] IN ('Pdf','Excel','Csv','Json')");
        });

        // Primary key
        builder.HasKey(e => e.ReportConfigurationId);

        builder.Property(e => e.ReportConfigurationId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.ReportCode)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(e => e.Description)
            .HasMaxLength(500);
        builder.Property(e => e.ReportType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.OutputFormat)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.QueryDefinition)
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.ParametersJson)
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.ScheduleCron)
            .HasMaxLength(50);
        builder.Property(e => e.Recipients)
            .HasMaxLength(1000);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.ReportCode)
            .HasDatabaseName("UX_ReportConfigurations_ReportCode")
            .IsUnique();

        builder.HasIndex(e => new { e.IsScheduled, e.NextRunAt })
            .HasDatabaseName("IX_ReportConfigurations_IsScheduled_NextRunAt");

        // Relationships
        builder.HasOne(e => e.OwnerUser)
            .WithMany(p => p.OwnedReportConfigurations)
            .HasForeignKey(e => e.OwnerUserId)
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
