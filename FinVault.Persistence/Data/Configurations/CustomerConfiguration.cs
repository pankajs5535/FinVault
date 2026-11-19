using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", t =>
        {
            t.HasCheckConstraint("CK_Customers_CustomerType", "[CustomerType] IN ('Individual','Minor','Business')");
            t.HasCheckConstraint("CK_Customers_Gender", "[Gender] IN ('Male','Female','Other')");
            t.HasCheckConstraint("CK_Customers_Status", "[Status] IN ('Active','Inactive','Dormant','Blocked','Closed')");
            t.HasCheckConstraint("CK_Customers_RiskCategory", "[RiskCategory] IN ('Low','Medium','High')");
        });

        // Primary key
        builder.HasKey(e => e.CustomerId);

        builder.Property(e => e.CustomerId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.CustomerNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.CustomerType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.MiddleName)
            .HasMaxLength(100);
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.DateOfBirth)
            .HasColumnType("date");
        builder.Property(e => e.Gender)
            .HasMaxLength(10);
        builder.Property(e => e.FatherOrSpouseName)
            .HasMaxLength(200);
        builder.Property(e => e.MobileNumber)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(e => e.AlternateMobileNumber)
            .HasMaxLength(15);
        builder.Property(e => e.Email)
            .HasMaxLength(256);
        builder.Property(e => e.Occupation)
            .HasMaxLength(100);
        builder.Property(e => e.AnnualIncome)
            .HasPrecision(18, 2);
        builder.Property(e => e.PreferredLanguage)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.RiskCategory)
            .IsRequired()
            .HasMaxLength(10);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Optimistic concurrency (SQL Server rowversion)
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.CustomerNumber)
            .HasDatabaseName("UX_Customers_CustomerNumber")
            .IsUnique();

        builder.HasIndex(e => e.MobileNumber)
            .HasDatabaseName("IX_Customers_MobileNumber");

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("IX_Customers_Email");

        builder.HasIndex(e => new { e.LastName, e.FirstName })
            .HasDatabaseName("IX_Customers_LastName_FirstName");

        // Relationships
        builder.HasOne(e => e.User)
            .WithOne(p => p.Customer)
            .HasForeignKey<Customer>(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.HomeBranch)
            .WithMany(p => p.Customers)
            .HasForeignKey(e => e.HomeBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RelationshipManager)
            .WithMany(p => p.ManagedCustomers)
            .HasForeignKey(e => e.RelationshipManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.NationalityCountry)
            .WithMany(p => p.Customers)
            .HasForeignKey(e => e.NationalityCountryId)
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
