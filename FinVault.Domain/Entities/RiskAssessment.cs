using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Risk score for a customer, account or transaction produced by rules or a model, with
explanation factors and the recommended decision.

RELATIONSHIP
- Optionally belongs to Customer through CustomerId.
- Optionally belongs to Account through AccountId.
- Optionally belongs to Transaction through TransactionId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by FraudAlert through RiskAssessmentId.
*/

public class RiskAssessment
{
    // =========================
    // Primary Key
    // =========================

    public int RiskAssessmentId { get; set; }

    // =========================
    // Assessment
    // =========================

    // Allowed values: Customer | Account | Transaction
    public string AssessmentType { get; set; } = "Transaction";

    // Normalised score from 0 to 100.
    public decimal RiskScore { get; set; }

    // Allowed values: Low | Medium | High | Critical
    public string RiskLevel { get; set; } = "Low";

    // Allowed values: Approve | Review | Challenge | Decline
    public string Decision { get; set; } = "Approve";

    public string ModelVersion { get; set; } = null!;

    // Explanation factors. Must not contain raw PAN/Aadhaar/card data.
    public string? FactorsJson { get; set; }

    public DateTime AssessedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? CustomerId { get; set; }

    public int? AccountId { get; set; }

    public int? TransactionId { get; set; }

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

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<FraudAlert> FraudAlerts { get; set; }
        = new List<FraudAlert>();
}
