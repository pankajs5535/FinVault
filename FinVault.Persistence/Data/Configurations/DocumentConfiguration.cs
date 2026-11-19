using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents", t =>
        {
            t.HasCheckConstraint("CK_Documents_DocumentType", "[DocumentType] IN ('Pan','Aadhaar','Passport','VoterId','DrivingLicence','Photo','Signature','AddressProof','IncomeProof','Form60','LoanAgreement','Other')");
            t.HasCheckConstraint("CK_Documents_VerificationStatus", "[VerificationStatus] IN ('Pending','Verified','Rejected','Expired')");
        });

        // Primary key
        builder.HasKey(e => e.DocumentId);

        builder.Property(e => e.DocumentId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.DocumentType)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(e => e.DocumentNumberMasked)
            .HasMaxLength(30);
        builder.Property(e => e.IssueDate)
            .HasColumnType("date");
        builder.Property(e => e.ExpiryDate)
            .HasColumnType("date");
        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(260);
        builder.Property(e => e.ContentType)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.StoragePath)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(e => e.ContentHash)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(e => e.VerificationStatus)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.CustomerId, e.DocumentType })
            .HasDatabaseName("IX_Documents_CustomerId_DocumentType");

        builder.HasIndex(e => e.ContentHash)
            .HasDatabaseName("IX_Documents_ContentHash");

        // Relationships
        builder.HasOne(e => e.Customer)
            .WithMany(p => p.Documents)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.KYCDetails)
            .WithMany(p => p.Documents)
            .HasForeignKey(e => e.KYCDetailsId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Loan)
            .WithMany(p => p.Documents)
            .HasForeignKey(e => e.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.VerifiedByEmployee)
            .WithMany(p => p.VerifiedDocuments)
            .HasForeignKey(e => e.VerifiedByEmployeeId)
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
