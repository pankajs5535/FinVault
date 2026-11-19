using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Return of funds for an earlier Transaction (failed payment, duplicate debit, merchant
refund, dispute outcome). The refund itself is posted as a separate Transaction.

RELATIONSHIP
- Belongs to Transaction through OriginalTransactionId.
- Optionally belongs to Transaction through RefundTransactionId (one-to-one).
- Optionally belongs to Dispute through DisputeId.
- Optionally belongs to User through RequestedByUserId.
- Optionally belongs to User through ApprovedByUserId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class Refund
{
    // =========================
    // Primary Key
    // =========================

    public int RefundId { get; set; }

    // =========================
    // Refund Details
    // =========================

    public string RefundReference { get; set; } = null!;

    // Allowed values: Full | Partial
    public string RefundType { get; set; } = "Full";

    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    // =========================
    // Lifecycle
    // =========================

    // Allowed values: Requested | Approved | Rejected | Processed | Failed
    public string Status { get; set; } = "Requested";

    public DateTime RequestedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string? RejectionReason { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int OriginalTransactionId { get; set; }

    public int? RefundTransactionId { get; set; }

    public int? DisputeId { get; set; }

    public int? RequestedByUserId { get; set; }

    public int? ApprovedByUserId { get; set; }

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

    public virtual Transaction OriginalTransaction { get; set; } = null!;

    public virtual Transaction? RefundTransaction { get; set; }

    public virtual Dispute? Dispute { get; set; }

    public virtual User? RequestedByUser { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
