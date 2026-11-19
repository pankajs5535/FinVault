using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Payment received against a loan (EMI, part-payment, foreclosure or penalty) split into
principal, interest and penalty components.

RELATIONSHIP
- Belongs to Loan through LoanId.
- Optionally belongs to LoanSchedule through LoanScheduleId.
- Optionally belongs to Transaction through TransactionId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class LoanRepayment
{
    // =========================
    // Primary Key
    // =========================

    public int LoanRepaymentId { get; set; }

    // =========================
    // Repayment
    // =========================

    public string RepaymentReference { get; set; } = null!;

    // Allowed values: Emi | PartPayment | ForeClosure | Penalty
    public string RepaymentType { get; set; } = "Emi";

    // Allowed values: AutoDebit | Nach | Online | Branch
    public string PaymentMode { get; set; } = "AutoDebit";

    public DateTime PaidAt { get; set; }

    // =========================
    // Amounts
    // =========================

    public decimal AmountPaid { get; set; }

    public decimal PrincipalComponent { get; set; }

    public decimal InterestComponent { get; set; }

    public decimal PenaltyComponent { get; set; }

    // =========================
    // Status
    // =========================

    // Allowed values: Pending | Successful | Failed | Reversed
    public string Status { get; set; } = "Pending";

    public string? FailureReason { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int LoanId { get; set; }

    public int? LoanScheduleId { get; set; }

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

    public virtual Loan Loan { get; set; } = null!;

    public virtual LoanSchedule? LoanSchedule { get; set; }

    public virtual Transaction? Transaction { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
