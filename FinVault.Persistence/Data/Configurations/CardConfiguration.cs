using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards", t =>
        {
            t.HasCheckConstraint("CK_Cards_CardType", "[CardType] IN ('Debit','Credit','Prepaid')");
            t.HasCheckConstraint("CK_Cards_Network", "[Network] IN ('RuPay','Visa','Mastercard')");
            t.HasCheckConstraint("CK_Cards_Status", "[Status] IN ('Pending','Active','Blocked','Hotlisted','Expired','Cancelled')");
        });

        // Primary key
        builder.HasKey(e => e.CardId);

        builder.Property(e => e.CardId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.CardNumberToken)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(e => e.CardNumberMasked)
            .IsRequired()
            .HasMaxLength(19);
        builder.Property(e => e.CardHolderName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.CardType)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Network)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.ExpiryDate)
            .HasColumnType("date");
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.BlockReason)
            .HasMaxLength(250);
        builder.Property(e => e.DailyAtmLimit)
            .HasPrecision(18, 2);
        builder.Property(e => e.DailyPosLimit)
            .HasPrecision(18, 2);
        builder.Property(e => e.DailyOnlineLimit)
            .HasPrecision(18, 2);
        builder.Property(e => e.CreditLimit)
            .HasPrecision(18, 2);
        builder.Property(e => e.AvailableCredit)
            .HasPrecision(18, 2);
        builder.Property(e => e.OutstandingAmount)
            .HasPrecision(18, 2);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.CardNumberToken)
            .HasDatabaseName("UX_Cards_CardNumberToken")
            .IsUnique();

        builder.HasIndex(e => new { e.AccountId, e.Status })
            .HasDatabaseName("IX_Cards_AccountId_Status");

        builder.HasIndex(e => new { e.CustomerId, e.Status })
            .HasDatabaseName("IX_Cards_CustomerId_Status");

        // Relationships
        builder.HasOne(e => e.Account)
            .WithMany(p => p.Cards)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
            .WithMany(p => p.Cards)
            .HasForeignKey(e => e.CustomerId)
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
