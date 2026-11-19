using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class LoanScheduleConfiguration : IEntityTypeConfiguration<LoanSchedule>
{
    public void Configure(EntityTypeBuilder<LoanSchedule> builder)
    {
        builder.ToTable("LoanSchedules", t =>
        {
            t.HasCheckConstraint("CK_LoanSchedules_Status", "[Status] IN ('Pending','PartiallyPaid','Paid','Overdue','Waived')");
            t.HasCheckConstraint("CK_LoanSchedules_Amounts", "[PrincipalDue] >= 0 AND [InterestDue] >= 0 AND [PrincipalPaid] >= 0 AND [InterestPaid] >= 0");
        });

        // Primary key
        builder.HasKey(e => e.LoanScheduleId);

        builder.Property(e => e.LoanScheduleId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.DueDate)
            .HasColumnType("date");
        builder.Property(e => e.OpeningBalance)
            .HasPrecision(18, 2);
        builder.Property(e => e.ClosingBalance)
            .HasPrecision(18, 2);
        builder.Property(e => e.PrincipalDue)
            .HasPrecision(18, 2);
        builder.Property(e => e.InterestDue)
            .HasPrecision(18, 2);
        builder.Property(e => e.PenaltyAccrued)
            .HasPrecision(18, 2);
        builder.Property(e => e.PrincipalPaid)
            .HasPrecision(18, 2);
        builder.Property(e => e.InterestPaid)
            .HasPrecision(18, 2);
        builder.Property(e => e.PenaltyPaid)
            .HasPrecision(18, 2);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.PaidOn)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => new { e.LoanId, e.InstallmentNumber })
            .HasDatabaseName("UX_LoanSchedules_LoanId_InstallmentNumber")
            .IsUnique();

        builder.HasIndex(e => new { e.DueDate, e.Status })
            .HasDatabaseName("IX_LoanSchedules_DueDate_Status");

        // Relationships
        builder.HasOne(e => e.Loan)
            .WithMany(p => p.LoanSchedules)
            .HasForeignKey(e => e.LoanId)
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
