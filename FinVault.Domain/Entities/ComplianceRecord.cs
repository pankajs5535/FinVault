using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Regulatory and AML/CFT record: Suspicious Transaction Reports (STR), Cash Transaction
Reports (CTR), counterfeit currency and non-profit reports to FIU-IND, sanctions/PEP
screening, periodic KYC review and FATCA/CRS.

RELATIONSHIP
- Optionally belongs to Customer through CustomerId.
- Optionally belongs to Account through AccountId.
- Optionally belongs to Transaction through TransactionId.
- Optionally belongs to FraudAlert through FraudAlertId.
- Optionally belongs to Employee through FiledByEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class ComplianceRecord
{
    // =========================
    // Primary Key
    // =========================

    public int ComplianceRecordId { get; set; }

    // =========================
    // Record Details
    // =========================

    public string ReferenceNumber { get; set; } = null!;

    // Allowed values: Str | Ctr | Ccr | Ntr | AmlScreening | PepCheck | SanctionsScreening | PeriodicKycReview | FatcaCrs
    public string RecordType { get; set; } = null!;

    public string Description { get; set; } = null!;

    // Allowed values: Low | Medium | High | Critical
    public string RiskLevel { get; set; } = "Low";

    // =========================
    // Filing
    // =========================

    // Allowed values: Open | UnderReview | Filed | Cleared | Closed
    public string Status { get; set; } = "Open";

    public string ReportingAuthority { get; set; } = "FIU-IND";

    public string? FilingReference { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? FiledAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? CustomerId { get; set; }

    public int? AccountId { get; set; }

    public int? TransactionId { get; set; }

    public int? FraudAlertId { get; set; }

    public int? FiledByEmployeeId { get; set; }

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

    public virtual Customer? Customer { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Transaction? Transaction { get; set; }

    public virtual FraudAlert? FraudAlert { get; set; }

    public virtual Employee? FiledByEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
