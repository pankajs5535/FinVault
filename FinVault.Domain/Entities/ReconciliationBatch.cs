using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
One reconciliation run comparing internal transactions with an external settlement
source (NPCI UPI/IMPS files, RBI NEFT/RTGS, card switch, ATM switch) or the general
ledger for a business date.

RELATIONSHIP
- Optionally belongs to PaymentMethod through PaymentMethodId.
- Optionally belongs to User through InitiatedByUserId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by ReconciliationRecord through ReconciliationBatchId.
*/

public class ReconciliationBatch
{
    // =========================
    // Primary Key
    // =========================

    public int ReconciliationBatchId { get; set; }

    // =========================
    // Batch Details
    // =========================

    public string BatchNumber { get; set; } = null!;

    // Allowed values: Upi | Neft | Rtgs | Imps | CardSettlement | AtmSwitch | GeneralLedger
    public string ReconciliationType { get; set; } = null!;

    public DateTime BusinessDate { get; set; }

    public string SourceName { get; set; } = null!;

    public string? FileName { get; set; }

    // SHA-256 of the settlement file; prevents the same file being loaded twice.
    public string? FileHash { get; set; }

    // =========================
    // Totals
    // =========================

    public int TotalRecords { get; set; } = 0;

    public int MatchedRecords { get; set; } = 0;

    public int UnmatchedRecords { get; set; } = 0;

    public decimal TotalAmount { get; set; }

    public decimal MatchedAmount { get; set; }

    public decimal DifferenceAmount { get; set; }

    // =========================
    // Status
    // =========================

    // Allowed values: Pending | InProgress | Completed | CompletedWithExceptions | Failed
    public string Status { get; set; } = "Pending";

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? PaymentMethodId { get; set; }

    public int? InitiatedByUserId { get; set; }

    // =========================
    // Concurrency
    // =========================

    public byte[] RowVersion { get; set; } = null!;

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

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual User? InitiatedByUser { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<ReconciliationRecord> ReconciliationRecords { get; set; }
        = new List<ReconciliationRecord>();
}
