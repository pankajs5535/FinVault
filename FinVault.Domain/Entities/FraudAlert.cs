using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Alert raised when a FraudRule fires or a RiskAssessment crosses a threshold. Reviewed by
a fraud analyst and either confirmed as fraud or cleared as a false positive.

RELATIONSHIP
- Belongs to FraudRule through FraudRuleId.
- Optionally belongs to Transaction through TransactionId.
- Optionally belongs to Customer through CustomerId.
- Optionally belongs to Account through AccountId.
- Optionally belongs to Card through CardId.
- Optionally belongs to RiskAssessment through RiskAssessmentId.
- Optionally belongs to Employee through ReviewedByEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by ComplianceRecord through FraudAlertId.
*/

public class FraudAlert
{
    // =========================
    // Primary Key
    // =========================

    public int FraudAlertId { get; set; }

    // =========================
    // Alert Details
    // =========================

    public string AlertNumber { get; set; } = null!;

    // Allowed values: Low | Medium | High | Critical
    public string Severity { get; set; } = "Medium";

    public decimal? Score { get; set; }

    public string Description { get; set; } = null!;

    // =========================
    // Lifecycle
    // =========================

    // Allowed values: Open | UnderReview | ConfirmedFraud | FalsePositive | Closed
    public string Status { get; set; } = "Open";

    public DateTime RaisedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewNotes { get; set; }

    public string? ActionTaken { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int FraudRuleId { get; set; }

    public int? TransactionId { get; set; }

    public int? CustomerId { get; set; }

    public int? AccountId { get; set; }

    public int? CardId { get; set; }

    public int? RiskAssessmentId { get; set; }

    public int? ReviewedByEmployeeId { get; set; }

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

    public virtual FraudRule FraudRule { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Card? Card { get; set; }

    public virtual RiskAssessment? RiskAssessment { get; set; }

    public virtual Employee? ReviewedByEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<ComplianceRecord> ComplianceRecords { get; set; }
        = new List<ComplianceRecord>();
}
