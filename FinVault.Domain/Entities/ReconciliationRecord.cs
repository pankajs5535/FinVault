using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Line-level match result inside a ReconciliationBatch between an external settlement
entry and an internal Transaction.

RELATIONSHIP
- Belongs to ReconciliationBatch through ReconciliationBatchId.
- Optionally belongs to Transaction through TransactionId.
- Optionally belongs to User through ResolvedByUserId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class ReconciliationRecord
{
    // =========================
    // Primary Key
    // =========================

    public int ReconciliationRecordId { get; set; }

    // =========================
    // External Entry
    // =========================

    public string ExternalReference { get; set; } = null!;

    public decimal ExternalAmount { get; set; }

    public DateTime ExternalDate { get; set; }

    // =========================
    // Match Result
    // =========================

    public decimal? InternalAmount { get; set; }

    public decimal DifferenceAmount { get; set; }

    // Allowed values: Matched | Unmatched | AmountMismatch | MissingInBank | MissingInExternal | Manual
    public string MatchStatus { get; set; } = "Unmatched";

    public string? ExceptionReason { get; set; }

    // =========================
    // Resolution
    // =========================

    public DateTime? ResolvedAt { get; set; }

    public string? ResolutionNotes { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int ReconciliationBatchId { get; set; }

    public int? TransactionId { get; set; }

    public int? ResolvedByUserId { get; set; }

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

    public virtual ReconciliationBatch ReconciliationBatch { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }

    public virtual User? ResolvedByUser { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
