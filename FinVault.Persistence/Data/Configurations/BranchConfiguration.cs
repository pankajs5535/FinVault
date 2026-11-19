using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches", t =>
        {
            t.HasCheckConstraint("CK_Branches_BranchType", "[BranchType] IN ('Regular','Main','Digital','Service')");
            t.HasCheckConstraint("CK_Branches_IfscCode", "[IfscCode] LIKE '[A-Z][A-Z][A-Z][A-Z]0[A-Z0-9][A-Z0-9][A-Z0-9][A-Z0-9][A-Z0-9][A-Z0-9]'");
            t.HasCheckConstraint("CK_Branches_PinCode", "[PinCode] LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]'");
        });

        // Primary key
        builder.HasKey(e => e.BranchId);

        builder.Property(e => e.BranchId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.BranchCode)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(e => e.BranchName)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(e => e.IfscCode)
            .IsRequired()
            .HasMaxLength(11);
        builder.Property(e => e.MicrCode)
            .HasMaxLength(9);
        builder.Property(e => e.BranchType)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.AddressLine2)
            .HasMaxLength(200);
        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.District)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.State)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.PinCode)
            .IsRequired()
            .HasMaxLength(6);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15);
        builder.Property(e => e.Email)
            .HasMaxLength(256);
        builder.Property(e => e.OpenedOn)
            .HasColumnType("date");
        builder.Property(e => e.ClosedOn)
            .HasColumnType("date");

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.BranchCode)
            .HasDatabaseName("UX_Branches_BranchCode")
            .IsUnique();

        builder.HasIndex(e => e.IfscCode)
            .HasDatabaseName("UX_Branches_IfscCode")
            .IsUnique();

        builder.HasIndex(e => e.MicrCode)
            .HasDatabaseName("UX_Branches_MicrCode")
            .IsUnique()
            .HasFilter("[MicrCode] IS NOT NULL");

        builder.HasIndex(e => new { e.State, e.City })
            .HasDatabaseName("IX_Branches_State_City");

        // Relationships
        builder.HasOne(e => e.Country)
            .WithMany(p => p.Branches)
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ParentBranch)
            .WithMany(p => p.ChildBranches)
            .HasForeignKey(e => e.ParentBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ManagerEmployee)
            .WithMany(p => p.ManagedBranches)
            .HasForeignKey(e => e.ManagerEmployeeId)
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
