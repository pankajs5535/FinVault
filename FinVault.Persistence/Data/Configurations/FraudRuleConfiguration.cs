using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class FraudRuleConfiguration : IEntityTypeConfiguration<FraudRule>
{
    public void Configure(EntityTypeBuilder<FraudRule> builder)
    {
        builder.ToTable("FraudRules", t =>
        {
            t.HasCheckConstraint("CK_FraudRules_RuleType", "[RuleType] IN ('Velocity','Amount','Geo','Device','Behavioural','Blacklist','Pattern')");
            t.HasCheckConstraint("CK_FraudRules_Severity", "[Severity] IN ('Low','Medium','High','Critical')");
            t.HasCheckConstraint("CK_FraudRules_ActionOnMatch", "[ActionOnMatch] IN ('Alert','Block','StepUpAuth','Review')");
        });

        // Primary key
        builder.HasKey(e => e.FraudRuleId);

        builder.Property(e => e.FraudRuleId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.RuleCode)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Description)
            .HasMaxLength(500);
        builder.Property(e => e.RuleType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Severity)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.RuleDefinitionJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.ThresholdAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.ActionOnMatch)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.EffectiveFrom)
            .HasColumnType("date");
        builder.Property(e => e.EffectiveTo)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.RuleCode)
            .HasDatabaseName("UX_FraudRules_RuleCode")
            .IsUnique();

        builder.HasIndex(e => new { e.IsActive, e.Priority })
            .HasDatabaseName("IX_FraudRules_IsActive_Priority");

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
