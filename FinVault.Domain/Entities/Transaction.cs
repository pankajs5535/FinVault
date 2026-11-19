using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Financial transaction header: one business event moving money between accounts (or
into/out of the bank).
Every posted Transaction is backed by a balanced JournalEntry. IdempotencyKey prevents
duplicate processing of retried requests; reversals are new Transactions pointing to the
original.

RELATIONSHIP
- Belongs to TransactionType through TransactionTypeId.
- Optionally belongs to Account through SourceAccountId.
- Optionally belongs to Account through DestinationAccountId.
- Belongs to Currency through CurrencyId.
- Optionally belongs to Fee through FeeId.
- Optionally belongs to User through InitiatedByUserId.
- Optionally belongs to User through ApprovedByUserId.
- Optionally belongs to Transaction through ReversalOfTransactionId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Transaction through ReversalOfTransactionId.
- Referenced by JournalEntry through TransactionId.
- Referenced by PaymentRequest through TransactionId.
- Referenced by CardTransaction through TransactionId.
- Referenced by LoanRepayment through TransactionId.
- Referenced by InterestCalculation through TransactionId.
- Referenced by Refund through OriginalTransactionId.
- Referenced by Refund through RefundTransactionId.
- Referenced by Dispute through TransactionId.
- Referenced by ReconciliationRecord through TransactionId.
- Referenced by RiskAssessment through TransactionId.
- Referenced by FraudAlert through TransactionId.
- Referenced by ComplianceRecord through TransactionId.
- PaymentRequest (one-to-one) and Refund (SourceRefund, one-to-one) navigations are
  exposed as reverse references.
*/

public class Transaction
{
    // =========================
    // Primary Key
    // =========================

    public int TransactionId { get; set; }

    // =========================
    // Reference
    // =========================

    // System-generated unique reference.
    public string TransactionReference { get; set; } = null!;

    // Client-supplied key; unique so a retried request cannot create a second
    // transaction.
    public string? IdempotencyKey { get; set; }

    // UTR / RRN / bank reference from the payment network.
    public string? ExternalReference { get; set; }

    // =========================
    // Amounts
    // =========================

    public decimal Amount { get; set; }

    public decimal FeeAmount { get; set; }

    // GST on fees.
    public decimal TaxAmount { get; set; }

    public decimal? SourceBalanceAfter { get; set; }

    // =========================
    // Lifecycle
    // =========================

    // Allowed values: Initiated | Pending | Processing | Posted | Failed | Reversed | Cancelled | OnHold
    public string Status { get; set; } = "Initiated";

    // Allowed values: Branch | ATM | NetBanking | MobileApp | UPI | POS | Api | Batch
    public string Channel { get; set; } = "Api";

    public DateTime TransactionAt { get; set; }

    public DateTime ValueDate { get; set; }

    public DateTime? PostedAt { get; set; }

    public string? FailureReason { get; set; }

    // =========================
    // Description
    // =========================

    public string? Description { get; set; }

    public string? Remarks { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int TransactionTypeId { get; set; }

    public int? SourceAccountId { get; set; }

    public int? DestinationAccountId { get; set; }

    public int CurrencyId { get; set; }

    public int? FeeId { get; set; }

    public int? InitiatedByUserId { get; set; }

    public int? ApprovedByUserId { get; set; }

    public int? ReversalOfTransactionId { get; set; }

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

    public virtual TransactionType TransactionType { get; set; } = null!;

    public virtual Account? SourceAccount { get; set; }

    public virtual Account? DestinationAccount { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual Fee? Fee { get; set; }

    public virtual User? InitiatedByUser { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual Transaction? ReversalOfTransaction { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual PaymentRequest? PaymentRequest { get; set; }

    public virtual Refund? SourceRefund { get; set; }

    public virtual ICollection<Transaction> ReversalTransactions { get; set; }
        = new List<Transaction>();

    public virtual ICollection<JournalEntry> JournalEntries { get; set; }
        = new List<JournalEntry>();

    public virtual ICollection<CardTransaction> CardTransactions { get; set; }
        = new List<CardTransaction>();

    public virtual ICollection<LoanRepayment> LoanRepayments { get; set; }
        = new List<LoanRepayment>();

    public virtual ICollection<InterestCalculation> InterestCalculations { get; set; }
        = new List<InterestCalculation>();

    public virtual ICollection<Refund> Refunds { get; set; }
        = new List<Refund>();

    public virtual ICollection<Dispute> Disputes { get; set; }
        = new List<Dispute>();

    public virtual ICollection<ReconciliationRecord> ReconciliationRecords { get; set; }
        = new List<ReconciliationRecord>();

    public virtual ICollection<RiskAssessment> RiskAssessments { get; set; }
        = new List<RiskAssessment>();

    public virtual ICollection<FraudAlert> FraudAlerts { get; set; }
        = new List<FraudAlert>();

    public virtual ICollection<ComplianceRecord> ComplianceRecords { get; set; }
        = new List<ComplianceRecord>();
}
