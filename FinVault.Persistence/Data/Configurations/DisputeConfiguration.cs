using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.ToTable("Disputes", t =>
        {
            t.HasCheckConstraint("CK_Disputes_DisputeType", "[DisputeType] IN ('UnauthorizedTransaction','DuplicateCharge','WrongAmount','ServiceNotReceived','AtmCashNotDispensed','FailedTransactionDebited','Other')");
            t.HasCheckConstraint("CK_Disputes_Status", "[Status] IN ('Open','UnderInvestigation','ProvisionalCreditGiven','Resolved','Rejected','Escalated','Closed')");
            t.HasCheckConstraint("CK_Disputes_Priority", "[Priority] IN ('Low','Medium','High','Critical')");
            t.HasCheckConstraint("CK_Disputes_Resolution", "[Resolution] IN ('InFavourOfCustomer','InFavourOfBank','Withdrawn')");
            t.HasCheckConstraint("CK_Disputes_DisputedAmount", "[DisputedAmount] > 0");
        });

        // Primary key
        builder.HasKey(e => e.DisputeId);

        builder.Property(e => e.DisputeId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.DisputeNumber)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.DisputeType)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(e => e.DisputedAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Priority)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Resolution)
            .HasMaxLength(30);
        builder.Property(e => e.ResolutionNotes)
            .HasMaxLength(1000);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.DisputeNumber)
            .HasDatabaseName("UX_Disputes_DisputeNumber")
            .IsUnique();

        builder.HasIndex(e => new { e.CustomerId, e.Status })
            .HasDatabaseName("IX_Disputes_CustomerId_Status");

        builder.HasIndex(e => new { e.Status, e.DueBy })
            .HasDatabaseName("IX_Disputes_Status_DueBy");

        // Relationships
        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.Disputes)
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CardTransaction)
            .WithMany(p => p.Disputes)
            .HasForeignKey(e => e.CardTransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
            .WithMany(p => p.Disputes)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AssignedToEmployee)
            .WithMany(p => p.AssignedDisputes)
            .HasForeignKey(e => e.AssignedToEmployeeId)
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
