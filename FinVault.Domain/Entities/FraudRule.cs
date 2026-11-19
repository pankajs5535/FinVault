using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Configurable fraud-detection rule (velocity, amount, device, geo, blacklist,
behavioural) evaluated against transactions, with the action to take on a match.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by FraudAlert through FraudRuleId.
*/

public class FraudRule
{
    // =========================
    // Primary Key
    // =========================

    public int FraudRuleId { get; set; }

    // =========================
    // Rule Information
    // =========================

    public string RuleCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    // Allowed values: Velocity | Amount | Geo | Device | Behavioural | Blacklist | Pattern
    public string RuleType { get; set; } = "Velocity";

    // Allowed values: Low | Medium | High | Critical
    public string Severity { get; set; } = "Medium";

    // =========================
    // Rule Definition
    // =========================

    // Serialised condition definition evaluated by the fraud engine.
    public string RuleDefinitionJson { get; set; } = null!;

    public decimal? ThresholdAmount { get; set; }

    public int? TimeWindowMinutes { get; set; }

    // Allowed values: Alert | Block | StepUpAuth | Review
    public string ActionOnMatch { get; set; } = "Alert";

    public int Priority { get; set; } = 100;

    public int Version { get; set; } = 1;

    // =========================
    // Status
    // =========================

    public bool IsActive { get; set; } = true;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

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

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<FraudAlert> FraudAlerts { get; set; }
        = new List<FraudAlert>();
}
