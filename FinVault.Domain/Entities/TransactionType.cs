using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Catalogue of transaction kinds (cash deposit, cash withdrawal, UPI transfer, NEFT,
interest credit, fee debit, loan disbursement, etc.) with approval and reversal rules.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Transaction through TransactionTypeId.
- Referenced by Fee through TransactionTypeId.
*/

public class TransactionType
{
    // =========================
    // Primary Key
    // =========================

    public int TransactionTypeId { get; set; }

    // =========================
    // Type Information
    // =========================

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    // Allowed values: Deposit | Withdrawal | Transfer | Payment | Fee | Interest | Refund | Reversal | Loan | Card
    public string Category { get; set; } = null!;

    public string? Description { get; set; }

    // =========================
    // Rules
    // =========================

    public bool RequiresApproval { get; set; }

    public decimal? ApprovalThreshold { get; set; }

    public decimal? DailyLimit { get; set; }

    public bool IsReversible { get; set; } = true;

    public bool IsActive { get; set; } = true;

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

    public virtual ICollection<Transaction> Transactions { get; set; }
        = new List<Transaction>();

    public virtual ICollection<Fee> Fees { get; set; }
        = new List<Fee>();
}
