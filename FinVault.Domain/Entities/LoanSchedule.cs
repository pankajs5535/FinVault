using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
One instalment of a loan's amortisation schedule (EMI) with principal and interest
split, payment progress and penalty.

RELATIONSHIP
- Belongs to Loan through LoanId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by LoanRepayment through LoanScheduleId.
*/

public class LoanSchedule
{
    // =========================
    // Primary Key
    // =========================

    public int LoanScheduleId { get; set; }

    // =========================
    // Instalment
    // =========================

    public int InstallmentNumber { get; set; }

    public DateTime DueDate { get; set; }

    public decimal OpeningBalance { get; set; }

    public decimal ClosingBalance { get; set; }

    // =========================
    // Amounts Due
    // =========================

    public decimal PrincipalDue { get; set; }

    public decimal InterestDue { get; set; }

    public decimal PenaltyAccrued { get; set; }

    // =========================
    // Amounts Paid
    // =========================

    public decimal PrincipalPaid { get; set; }

    public decimal InterestPaid { get; set; }

    public decimal PenaltyPaid { get; set; }

    // =========================
    // Status
    // =========================

    // Allowed values: Pending | PartiallyPaid | Paid | Overdue | Waived
    public string Status { get; set; } = "Pending";

    public DateTime? PaidOn { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int LoanId { get; set; }

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

    public virtual Loan Loan { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<LoanRepayment> LoanRepayments { get; set; }
        = new List<LoanRepayment>();
}
