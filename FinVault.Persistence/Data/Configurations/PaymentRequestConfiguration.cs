using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
{
    public void Configure(EntityTypeBuilder<PaymentRequest> builder)
    {
        builder.ToTable("PaymentRequests", t =>
        {
            t.HasCheckConstraint("CK_PaymentRequests_Channel", "[Channel] IN ('Branch','NetBanking','MobileApp','UPI','Api')");
            t.HasCheckConstraint("CK_PaymentRequests_Status", "[Status] IN ('Created','Authorized','Submitted','Processing','Completed','Failed','Rejected','Expired','Cancelled')");
            t.HasCheckConstraint("CK_PaymentRequests_Amount", "[Amount] > 0");
            t.HasCheckConstraint("CK_PaymentRequests_Payee", "[BeneficiaryId] IS NOT NULL OR [PayeeAccountNumber] IS NOT NULL OR [PayeeVpa] IS NOT NULL");
        });

        // Primary key
        builder.HasKey(e => e.PaymentRequestId);

        builder.Property(e => e.PaymentRequestId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.RequestReference)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(e => e.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(e => e.UtrNumber)
            .HasMaxLength(30);
        builder.Property(e => e.PayeeName)
            .HasMaxLength(150);
        builder.Property(e => e.PayeeAccountNumber)
            .HasMaxLength(20);
        builder.Property(e => e.PayeeIfsc)
            .HasMaxLength(11);
        builder.Property(e => e.PayeeVpa)
            .HasMaxLength(100);
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2);
        builder.Property(e => e.Purpose)
            .HasMaxLength(250);
        builder.Property(e => e.Remarks)
            .HasMaxLength(250);
        builder.Property(e => e.Channel)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.FailureCode)
            .HasMaxLength(30);
        builder.Property(e => e.FailureReason)
            .HasMaxLength(250);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.RequestReference)
            .HasDatabaseName("UX_PaymentRequests_RequestReference")
            .IsUnique();

        builder.HasIndex(e => e.IdempotencyKey)
            .HasDatabaseName("UX_PaymentRequests_IdempotencyKey")
            .IsUnique();

        builder.HasIndex(e => new { e.CustomerId, e.RequestedAt })
            .HasDatabaseName("IX_PaymentRequests_CustomerId_RequestedAt");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_PaymentRequests_Status");

        builder.HasIndex(e => e.UtrNumber)
            .HasDatabaseName("IX_PaymentRequests_UtrNumber");

        // Relationships
        builder.HasOne(e => e.PaymentMethod)
            .WithMany(p => p.PaymentRequests)
            .HasForeignKey(e => e.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
            .WithMany(p => p.PaymentRequests)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SourceAccount)
            .WithMany(p => p.PaymentRequests)
            .HasForeignKey(e => e.SourceAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Beneficiary)
            .WithMany(p => p.PaymentRequests)
            .HasForeignKey(e => e.BeneficiaryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithOne(p => p.PaymentRequest)
            .HasForeignKey<PaymentRequest>(e => e.TransactionId)
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
