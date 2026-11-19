using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class CardTransactionConfiguration : IEntityTypeConfiguration<CardTransaction>
{
    public void Configure(EntityTypeBuilder<CardTransaction> builder)
    {
        builder.ToTable("CardTransactions", t =>
        {
            t.HasCheckConstraint("CK_CardTransactions_AuthorizationStatus", "[AuthorizationStatus] IN ('Authorized','Declined','Reversed','Settled','Expired')");
            t.HasCheckConstraint("CK_CardTransactions_Channel", "[Channel] IN ('POS','ATM','Ecommerce','Contactless','Recurring')");
            t.HasCheckConstraint("CK_CardTransactions_Amount", "[Amount] > 0");
        });

        // Primary key
        builder.HasKey(e => e.CardTransactionId);

        builder.Property(e => e.CardTransactionId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.AuthorizationCode)
            .HasMaxLength(20);
        builder.Property(e => e.RetrievalReferenceNumber)
            .HasMaxLength(12);
        builder.Property(e => e.AuthorizationStatus)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.DeclineReason)
            .HasMaxLength(100);
        builder.Property(e => e.MerchantName)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(e => e.MerchantCategoryCode)
            .IsRequired()
            .HasMaxLength(4);
        builder.Property(e => e.MerchantCity)
            .HasMaxLength(100);
        builder.Property(e => e.TerminalId)
            .HasMaxLength(20);
        builder.Property(e => e.Channel)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2);
        builder.Property(e => e.SettlementAmount)
            .HasPrecision(18, 2);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.CardId, e.AuthorizedAt })
            .HasDatabaseName("IX_CardTransactions_CardId_AuthorizedAt");

        builder.HasIndex(e => e.RetrievalReferenceNumber)
            .HasDatabaseName("UX_CardTransactions_RetrievalReferenceNumber")
            .IsUnique()
            .HasFilter("[RetrievalReferenceNumber] IS NOT NULL");

        builder.HasIndex(e => e.MerchantCategoryCode)
            .HasDatabaseName("IX_CardTransactions_MerchantCategoryCode");

        // Relationships
        builder.HasOne(e => e.Card)
            .WithMany(p => p.CardTransactions)
            .HasForeignKey(e => e.CardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany(p => p.CardTransactions)
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId)
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
