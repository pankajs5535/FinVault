using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Metadata for an uploaded customer document (KYC proof, address proof, income proof, loan
paperwork). The file itself lives in encrypted object storage; only its location and
hash are stored here.

RELATIONSHIP
- Belongs to Customer through CustomerId.
- Optionally belongs to KYCDetails through KYCDetailsId.
- Optionally belongs to Loan through LoanId.
- Optionally belongs to Employee through VerifiedByEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class Document
{
    // =========================
    // Primary Key
    // =========================

    public int DocumentId { get; set; }

    // =========================
    // Document Details
    // =========================

    // Allowed values: Pan | Aadhaar | Passport | VoterId | DrivingLicence | Photo | Signature | AddressProof | IncomeProof | Form60 | LoanAgreement | Other
    public string DocumentType { get; set; } = "Other";

    // Masked identifier only, e.g. XXXX-XXXX-1234.
    public string? DocumentNumberMasked { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    // =========================
    // File Information
    // =========================

    public string FileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    // Key/path in the encrypted document store.
    public string StoragePath { get; set; } = null!;

    // SHA-256 of the file for tamper detection.
    public string ContentHash { get; set; } = null!;

    public bool IsEncrypted { get; set; } = true;

    // =========================
    // Verification
    // =========================

    // Allowed values: Pending | Verified | Rejected | Expired
    public string VerificationStatus { get; set; } = "Pending";

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int CustomerId { get; set; }

    public int? KYCDetailsId { get; set; }

    public int? LoanId { get; set; }

    public int? VerifiedByEmployeeId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual Customer Customer { get; set; } = null!;

    public virtual KYCDetails? KYCDetails { get; set; }

    public virtual Loan? Loan { get; set; }

    public virtual Employee? VerifiedByEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
