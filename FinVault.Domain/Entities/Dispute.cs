using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Customer complaint about a transaction (unauthorised use, duplicate charge, ATM cash not
dispensed, failed transaction with debit). Tracks investigation, turnaround deadline and
resolution.

RELATIONSHIP
- Belongs to Transaction through TransactionId.
- Optionally belongs to CardTransaction through CardTransactionId.
- Belongs to Customer through CustomerId.
- Optionally belongs to Employee through AssignedToEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Refund through DisputeId.
*/

public class Dispute
{
    // =========================
    // Primary Key
    // =========================

    public int DisputeId { get; set; }

    // =========================
    // Dispute Details
    // =========================

    public string DisputeNumber { get; set; } = null!;

    // Allowed values: UnauthorizedTransaction | DuplicateCharge | WrongAmount | ServiceNotReceived | AtmCashNotDispensed | FailedTransactionDebited | Other
    public string DisputeType { get; set; } = "Other";

    public string Description { get; set; } = null!;

    public decimal DisputedAmount { get; set; }

    // =========================
    // Lifecycle
    // =========================

    // Allowed values: Open | UnderInvestigation | ProvisionalCreditGiven | Resolved | Rejected | Escalated | Closed
    public string Status { get; set; } = "Open";

    // Allowed values: Low | Medium | High | Critical
    public string Priority { get; set; } = "Medium";

    public DateTime RaisedAt { get; set; }

    public DateTime? DueBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    // =========================
    // Resolution
    // =========================

    // Allowed values: InFavourOfCustomer | InFavourOfBank | Withdrawn
    public string? Resolution { get; set; }

    public string? ResolutionNotes { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int TransactionId { get; set; }

    public int? CardTransactionId { get; set; }

    public int CustomerId { get; set; }

    public int? AssignedToEmployeeId { get; set; }

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

    public virtual Transaction Transaction { get; set; } = null!;

    public virtual CardTransaction? CardTransaction { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Employee? AssignedToEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; }
        = new List<Refund>();
}
