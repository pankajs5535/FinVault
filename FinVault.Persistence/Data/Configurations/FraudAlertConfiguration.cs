using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class FraudAlertConfiguration : IEntityTypeConfiguration<FraudAlert>
{
    public void Configure(EntityTypeBuilder<FraudAlert> builder)
    {
        builder.ToTable("FraudAlerts", t =>
        {
            t.HasCheckConstraint("CK_FraudAlerts_Severity", "[Severity] IN ('Low','Medium','High','Critical')");
            t.HasCheckConstraint("CK_FraudAlerts_Status", "[Status] IN ('Open','UnderReview','ConfirmedFraud','FalsePositive','Closed')");
            t.HasCheckConstraint("CK_FraudAlerts_Subject", "[TransactionId] IS NOT NULL OR [CustomerId] IS NOT NULL OR [AccountId] IS NOT NULL OR [CardId] IS NOT NULL");
        });

        // Primary key
        builder.HasKey(e => e.FraudAlertId);

        builder.Property(e => e.FraudAlertId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.AlertNumber)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Severity)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Score)
            .HasPrecision(5, 2);
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.ReviewNotes)
            .HasMaxLength(1000);
        builder.Property(e => e.ActionTaken)
            .HasMaxLength(100);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.AlertNumber)
            .HasDatabaseName("UX_FraudAlerts_AlertNumber")
            .IsUnique();

        builder.HasIndex(e => new { e.Status, e.RaisedAt })
            .HasDatabaseName("IX_FraudAlerts_Status_RaisedAt");

        builder.HasIndex(e => new { e.CustomerId, e.Status })
            .HasDatabaseName("IX_FraudAlerts_CustomerId_Status");

        // Relationships
        builder.HasOne(e => e.FraudRule)
            .WithMany(p => p.FraudAlerts)
            .HasForeignKey(e => e.FraudRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.FraudAlerts)
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
            .WithMany(p => p.FraudAlerts)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Account)
            .WithMany(p => p.FraudAlerts)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Card)
            .WithMany(p => p.FraudAlerts)
            .HasForeignKey(e => e.CardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RiskAssessment)
            .WithMany(p => p.FraudAlerts)
            .HasForeignKey(e => e.RiskAssessmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ReviewedByEmployee)
            .WithMany(p => p.ReviewedFraudAlerts)
            .HasForeignKey(e => e.ReviewedByEmployeeId)
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
