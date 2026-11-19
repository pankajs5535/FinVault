using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions", t =>
        {
            t.HasCheckConstraint("CK_Transactions_Status", "[Status] IN ('Initiated','Pending','Processing','Posted','Failed','Reversed','Cancelled','OnHold')");
            t.HasCheckConstraint("CK_Transactions_Channel", "[Channel] IN ('Branch','ATM','NetBanking','MobileApp','UPI','POS','Api','Batch')");
            t.HasCheckConstraint("CK_Transactions_Amount", "[Amount] > 0");
            t.HasCheckConstraint("CK_Transactions_Fees", "[FeeAmount] >= 0 AND [TaxAmount] >= 0");
            t.HasCheckConstraint("CK_Transactions_Accounts", "[SourceAccountId] IS NOT NULL OR [DestinationAccountId] IS NOT NULL");
        });

        // Primary key
        builder.HasKey(e => e.TransactionId);

        builder.Property(e => e.TransactionId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.TransactionReference)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(e => e.IdempotencyKey)
            .HasMaxLength(64);
        builder.Property(e => e.ExternalReference)
            .HasMaxLength(50);
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2);
        builder.Property(e => e.FeeAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.TaxAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.SourceBalanceAfter)
            .HasPrecision(18, 2);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Channel)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.ValueDate)
            .HasColumnType("date");
        builder.Property(e => e.FailureReason)
            .HasMaxLength(250);
        builder.Property(e => e.Description)
            .HasMaxLength(250);
        builder.Property(e => e.Remarks)
            .HasMaxLength(250);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.TransactionReference)
            .HasDatabaseName("UX_Transactions_TransactionReference")
            .IsUnique();

        builder.HasIndex(e => e.IdempotencyKey)
            .HasDatabaseName("UX_Transactions_IdempotencyKey")
            .IsUnique()
            .HasFilter("[IdempotencyKey] IS NOT NULL");

        builder.HasIndex(e => new { e.SourceAccountId, e.TransactionAt })
            .HasDatabaseName("IX_Transactions_SourceAccountId_TransactionAt");

        builder.HasIndex(e => new { e.DestinationAccountId, e.TransactionAt })
            .HasDatabaseName("IX_Transactions_DestinationAccountId_TransactionAt");

        builder.HasIndex(e => new { e.Status, e.TransactionAt })
            .HasDatabaseName("IX_Transactions_Status_TransactionAt");

        builder.HasIndex(e => e.ExternalReference)
            .HasDatabaseName("IX_Transactions_ExternalReference");

        // Relationships
        builder.HasOne(e => e.TransactionType)
            .WithMany(p => p.Transactions)
            .HasForeignKey(e => e.TransactionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SourceAccount)
            .WithMany(p => p.OutgoingTransactions)
            .HasForeignKey(e => e.SourceAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DestinationAccount)
            .WithMany(p => p.IncomingTransactions)
            .HasForeignKey(e => e.DestinationAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Currency)
            .WithMany(p => p.Transactions)
            .HasForeignKey(e => e.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Fee)
            .WithMany(p => p.ChargedTransactions)
            .HasForeignKey(e => e.FeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InitiatedByUser)
            .WithMany(p => p.InitiatedTransactions)
            .HasForeignKey(e => e.InitiatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ApprovedByUser)
            .WithMany()
            .HasForeignKey(e => e.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ReversalOfTransaction)
            .WithMany(p => p.ReversalTransactions)
            .HasForeignKey(e => e.ReversalOfTransactionId)
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
