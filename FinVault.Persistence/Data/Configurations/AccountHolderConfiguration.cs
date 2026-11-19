using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AccountHolderConfiguration : IEntityTypeConfiguration<AccountHolder>
{
    public void Configure(EntityTypeBuilder<AccountHolder> builder)
    {
        builder.ToTable("AccountHolders", t =>
        {
            t.HasCheckConstraint("CK_AccountHolders_HolderType", "[HolderType] IN ('Primary','Joint','Guardian','AuthorizedSignatory')");
        });

        // Primary key
        builder.HasKey(e => e.AccountHolderId);

        builder.Property(e => e.AccountHolderId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.HolderType)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.EffectiveFrom)
            .HasColumnType("date");
        builder.Property(e => e.EffectiveTo)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.AccountId, e.CustomerId })
            .HasDatabaseName("UX_AccountHolders_Account_Customer_Active")
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.HasIndex(e => e.AccountId)
            .HasDatabaseName("UX_AccountHolders_Account_OnePrimary")
            .IsUnique()
            .HasFilter("[HolderType] = 'Primary' AND [IsActive] = 1");

        // Relationships
        builder.HasOne(e => e.Account)
            .WithMany(p => p.AccountHolders)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
            .WithMany(p => p.AccountHolders)
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
