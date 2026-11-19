using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Know-Your-Customer record for a customer. Supports PAN, Aadhaar (privacy protected) and
CKYC data plus periodic re-KYC.
PAN is stored encrypted with a keyed hash for lookup; Aadhaar is stored only as a vault
reference token and last 4 digits. A customer may have several records over time;
exactly one is current.

RELATIONSHIP
- Belongs to Customer through CustomerId.
- Optionally belongs to Employee through VerifiedByEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Document through KYCDetailsId.
*/

public class KYCDetails
{
    // =========================
    // Primary Key
    // =========================

    public int KYCDetailsId { get; set; }

    // =========================
    // Verification Summary
    // =========================

    // Allowed values: Minimum | Full | Video | EKyc
    public string KycLevel { get; set; } = "Minimum";

    // Allowed values: Pending | InProgress | Verified | Rejected | Expired
    public string VerificationStatus { get; set; } = "Pending";

    public bool IsCurrent { get; set; } = true;

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime? NextReKycDueDate { get; set; }

    // =========================
    // PAN
    // =========================

    // AES-256 ciphertext of the PAN. Key is held in a KMS/HSM, not in the database.
    public string? PanEncrypted { get; set; }

    // HMAC-SHA256 of the normalised PAN, used for duplicate detection and lookup.
    public string? PanHash { get; set; }

    // Display value such as ABCDE****F.
    public string? PanMasked { get; set; }

    public bool IsPanVerified { get; set; }

    public DateTime? PanVerifiedAt { get; set; }

    // True when the customer has no PAN and submitted Form 60.
    public bool IsForm60Submitted { get; set; }

    // =========================
    // Aadhaar (Privacy Protected)
    // =========================

    // Token issued by the Aadhaar Data Vault. The 12-digit Aadhaar number is never
    // stored.
    public string? AadhaarVaultReference { get; set; }

    public string? AadhaarLast4 { get; set; }

    // Allowed values: OtpEkyc | BiometricEkyc | OfflineXml | VirtualId
    public string? AadhaarVerificationMethod { get; set; }

    public bool IsAadhaarVerified { get; set; }

    public DateTime? AadhaarVerifiedAt { get; set; }

    // =========================
    // CKYC
    // =========================

    // 14-digit identifier issued by the Central KYC Records Registry.
    public string? CkycNumber { get; set; }

    // Allowed values: NotInitiated | Pending | Uploaded | Verified | Rejected
    public string CkycStatus { get; set; } = "NotInitiated";

    public DateTime? CkycVerifiedAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int CustomerId { get; set; }

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

    public virtual Employee? VerifiedByEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Document> Documents { get; set; }
        = new List<Document>();
}
