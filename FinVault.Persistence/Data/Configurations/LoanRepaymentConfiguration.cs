using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class LoanRepaymentConfiguration : IEntityTypeConfiguration<LoanRepayment>
{
    public void Configure(EntityTypeBuilder<LoanRepayment> builder)
    {
        builder.ToTable("LoanRepayments", t =>
        {
            t.HasCheckConstraint("CK_LoanRepayments_RepaymentType", "[RepaymentType] IN ('Emi','PartPayment','ForeClosure','Penalty')");
            t.HasCheckConstraint("CK_LoanRepayments_PaymentMode", "[PaymentMode] IN ('AutoDebit','Nach','Online','Branch')");
            t.HasCheckConstraint("CK_LoanRepayments_Status", "[Status] IN ('Pending','Successful','Failed','Reversed')");
            t.HasCheckConstraint("CK_LoanRepayments_Components", "[AmountPaid] = [PrincipalComponent] + [InterestComponent] + [PenaltyComponent]");
            t.HasCheckConstraint("CK_LoanRepayments_AmountPaid", "[AmountPaid] > 0");
        });

        // Primary key
        builder.HasKey(e => e.LoanRepaymentId);

        builder.Property(e => e.LoanRepaymentId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.RepaymentReference)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(e => e.RepaymentType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.PaymentMode)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.AmountPaid)
            .HasPrecision(18, 2);
        builder.Property(e => e.PrincipalComponent)
            .HasPrecision(18, 2);
        builder.Property(e => e.InterestComponent)
            .HasPrecision(18, 2);
        builder.Property(e => e.PenaltyComponent)
            .HasPrecision(18, 2);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.FailureReason)
            .HasMaxLength(250);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.RepaymentReference)
            .HasDatabaseName("UX_LoanRepayments_RepaymentReference")
            .IsUnique();

        builder.HasIndex(e => new { e.LoanId, e.PaidAt })
            .HasDatabaseName("IX_LoanRepayments_LoanId_PaidAt");

        // Relationships
        builder.HasOne(e => e.Loan)
            .WithMany(p => p.LoanRepayments)
            .HasForeignKey(e => e.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.LoanSchedule)
            .WithMany(p => p.LoanRepayments)
            .HasForeignKey(e => e.LoanScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.LoanRepayments)
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
