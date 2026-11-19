using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Result of an interest accrual, payout or capitalisation run for a deposit account or a
loan over a period, including TDS where applicable. Once posted it links to the
JournalEntry and Transaction it created.

RELATIONSHIP
- Optionally belongs to Account through AccountId.
- Optionally belongs to Loan through LoanId.
- Belongs to InterestRate through InterestRateId.
- Optionally belongs to JournalEntry through JournalEntryId.
- Optionally belongs to Transaction through TransactionId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class InterestCalculation
{
    // =========================
    // Primary Key
    // =========================

    public int InterestCalculationId { get; set; }

    // =========================
    // Calculation
    // =========================

    // Allowed values: Accrual | Payout | Capitalisation
    public string CalculationType { get; set; } = "Accrual";

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public int DayCount { get; set; }

    // Average or closing balance used for the calculation.
    public decimal PrincipalBase { get; set; }

    public decimal AppliedRate { get; set; }

    public decimal InterestAmount { get; set; }

    public decimal TdsAmount { get; set; }

    // =========================
    // Status
    // =========================

    // Allowed values: Calculated | Posted | Reversed
    public string Status { get; set; } = "Calculated";

    public DateTime CalculatedAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? AccountId { get; set; }

    public int? LoanId { get; set; }

    public int InterestRateId { get; set; }

    public int? JournalEntryId { get; set; }

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

    public virtual Account? Account { get; set; }

    public virtual Loan? Loan { get; set; }

    public virtual InterestRate InterestRate { get; set; } = null!;

    public virtual JournalEntry? JournalEntry { get; set; }

    public virtual Transaction? Transaction { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
