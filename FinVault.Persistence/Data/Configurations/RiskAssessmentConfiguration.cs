using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class RiskAssessmentConfiguration : IEntityTypeConfiguration<RiskAssessment>
{
    public void Configure(EntityTypeBuilder<RiskAssessment> builder)
    {
        builder.ToTable("RiskAssessments", t =>
        {
            t.HasCheckConstraint("CK_RiskAssessments_AssessmentType", "[AssessmentType] IN ('Customer','Account','Transaction')");
            t.HasCheckConstraint("CK_RiskAssessments_RiskLevel", "[RiskLevel] IN ('Low','Medium','High','Critical')");
            t.HasCheckConstraint("CK_RiskAssessments_Decision", "[Decision] IN ('Approve','Review','Challenge','Decline')");
            t.HasCheckConstraint("CK_RiskAssessments_Subject", "[CustomerId] IS NOT NULL OR [AccountId] IS NOT NULL OR [TransactionId] IS NOT NULL");
            t.HasCheckConstraint("CK_RiskAssessments_RiskScore", "[RiskScore] BETWEEN 0 AND 100");
        });

        // Primary key
        builder.HasKey(e => e.RiskAssessmentId);

        builder.Property(e => e.RiskAssessmentId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.AssessmentType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.RiskScore)
            .HasPrecision(5, 2);
        builder.Property(e => e.RiskLevel)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Decision)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.ModelVersion)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.FactorsJson)
            .HasColumnType("nvarchar(max)");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.CustomerId, e.AssessedAt })
            .HasDatabaseName("IX_RiskAssessments_CustomerId_AssessedAt");

        builder.HasIndex(e => e.TransactionId)
            .HasDatabaseName("IX_RiskAssessments_TransactionId");

        builder.HasIndex(e => e.RiskLevel)
            .HasDatabaseName("IX_RiskAssessments_RiskLevel");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.RiskAssessments)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Account)
            .WithMany(p => p.RiskAssessments)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.RiskAssessments)
            .HasForeignKey(e => e.TransactionId)
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
