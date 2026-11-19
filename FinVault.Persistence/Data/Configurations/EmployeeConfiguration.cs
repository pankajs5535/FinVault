using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees", t =>
        {
            t.HasCheckConstraint("CK_Employees_EmploymentStatus", "[EmploymentStatus] IN ('Active','OnLeave','Suspended','Resigned','Terminated')");
        });

        // Primary key
        builder.HasKey(e => e.EmployeeId);

        builder.Property(e => e.EmployeeId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.EmployeeCode)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Designation)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Department)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15);
        builder.Property(e => e.DateOfJoining)
            .HasColumnType("date");
        builder.Property(e => e.DateOfLeaving)
            .HasColumnType("date");
        builder.Property(e => e.EmploymentStatus)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.ApprovalLimit)
            .HasPrecision(18, 2);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => e.EmployeeCode)
            .HasDatabaseName("UX_Employees_EmployeeCode")
            .IsUnique();

        builder.HasIndex(e => new { e.BranchId, e.EmploymentStatus })
            .HasDatabaseName("IX_Employees_BranchId_EmploymentStatus");

        // Relationships
        builder.HasOne(e => e.User)
            .WithOne(p => p.Employee)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Branch)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ReportsToEmployee)
            .WithMany(p => p.DirectReports)
            .HasForeignKey(e => e.ReportsToEmployeeId)
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
