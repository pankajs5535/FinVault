using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", t =>
        {
            t.HasCheckConstraint("CK_Accounts_Status", "[Status] IN ('Pending','Active','Dormant','Inactive','Frozen','Closed')");
            t.HasCheckConstraint("CK_Accounts_ModeOfOperation", "[ModeOfOperation] IN ('Single','Joint','EitherOrSurvivor','FormerOrSurvivor')");
            t.HasCheckConstraint("CK_Accounts_HoldAmount", "[HoldAmount] >= 0");
            t.HasCheckConstraint("CK_Accounts_OverdraftLimit", "[OverdraftLimit] >= 0");
        });

        // Primary key
        builder.HasKey(e => e.AccountId);

        builder.Property(e => e.AccountId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.AccountNumber)
            .IsRequired()
            .HasMaxLength(18);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.StatusReason)
            .HasMaxLength(250);
        builder.Property(e => e.ModeOfOperation)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.Balance)
            .HasPrecision(18, 2);
        builder.Property(e => e.AvailableBalance)
            .HasPrecision(18, 2);
        builder.Property(e => e.HoldAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.OverdraftLimit)
            .HasPrecision(18, 2);
        builder.Property(e => e.OpenedOn)
            .HasColumnType("date");
        builder.Property(e => e.ClosedOn)
            .HasColumnType("date");
        builder.Property(e => e.NomineeName)
            .HasMaxLength(150);
        builder.Property(e => e.NomineeRelationship)
            .HasMaxLength(50);
        builder.Property(e => e.PrincipalAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.MaturityAmount)
            .HasPrecision(18, 2);
        builder.Property(e => e.ContractedInterestRate)
            .HasPrecision(9, 4);
        builder.Property(e => e.MaturityDate)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.AccountNumber)
            .HasDatabaseName("UX_Accounts_AccountNumber")
            .IsUnique();

        builder.HasIndex(e => new { e.BranchId, e.Status })
            .HasDatabaseName("IX_Accounts_BranchId_Status");

        builder.HasIndex(e => e.MaturityDate)
            .HasDatabaseName("IX_Accounts_MaturityDate");

        // Relationships
        builder.HasOne(e => e.AccountType)
            .WithMany(p => p.Accounts)
            .HasForeignKey(e => e.AccountTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Branch)
            .WithMany(p => p.Accounts)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Currency)
            .WithMany(p => p.Accounts)
            .HasForeignKey(e => e.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PayoutAccount)
            .WithMany(p => p.LinkedDepositAccounts)
            .HasForeignKey(e => e.PayoutAccountId)
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
