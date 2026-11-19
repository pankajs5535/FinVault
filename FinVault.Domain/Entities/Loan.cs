using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Loan account for personal, home, vehicle, education, business, gold or deposit-backed
loans, covering application, sanction, disbursement, outstanding balances and RBI asset
classification.

RELATIONSHIP
- Belongs to Customer through CustomerId.
- Belongs to Branch through BranchId.
- Optionally belongs to Account through DisbursementAccountId.
- Optionally belongs to Account through RepaymentAccountId.
- Optionally belongs to InterestRate through InterestRateId.
- Optionally belongs to Employee through ApprovedByEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Document through LoanId.
- Referenced by LoanSchedule through LoanId.
- Referenced by LoanRepayment through LoanId.
- Referenced by InterestCalculation through LoanId.
*/

public class Loan
{
    // =========================
    // Primary Key
    // =========================

    public int LoanId { get; set; }

    // =========================
    // Loan Information
    // =========================

    public string LoanNumber { get; set; } = null!;

    // Allowed values: Personal | Home | Vehicle | Education | Business | Gold | AgainstDeposit
    public string LoanType { get; set; } = "Personal";

    public string? Purpose { get; set; }

    // Allowed values: Applied | Approved | Rejected | Disbursed | Active | Closed | WrittenOff
    public string Status { get; set; } = "Applied";

    // =========================
    // Amounts
    // =========================

    public decimal RequestedAmount { get; set; }

    public decimal? SanctionedAmount { get; set; }

    public decimal DisbursedAmount { get; set; }

    public decimal OutstandingPrincipal { get; set; }

    public decimal OutstandingInterest { get; set; }

    public decimal? EmiAmount { get; set; }

    // =========================
    // Terms
    // =========================

    public decimal AnnualInterestRate { get; set; }

    // Allowed values: Fixed | Floating
    public string InterestType { get; set; } = "Fixed";

    public int TenureMonths { get; set; }

    // =========================
    // Dates
    // =========================

    public DateTime ApplicationDate { get; set; }

    public DateTime? SanctionDate { get; set; }

    public DateTime? DisbursementDate { get; set; }

    public DateTime? FirstEmiDate { get; set; }

    public DateTime? MaturityDate { get; set; }

    public DateTime? ClosedOn { get; set; }

    // =========================
    // Asset Classification
    // =========================

    public int DaysPastDue { get; set; } = 0;

    // Allowed values: Standard | SMA0 | SMA1 | SMA2 | SubStandard | Doubtful | Loss
    public string AssetClassification { get; set; } = "Standard";

    // =========================
    // Foreign Keys
    // =========================

    public int CustomerId { get; set; }

    public int BranchId { get; set; }

    public int? DisbursementAccountId { get; set; }

    public int? RepaymentAccountId { get; set; }

    public int? InterestRateId { get; set; }

    public int? ApprovedByEmployeeId { get; set; }

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

    public virtual Customer Customer { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual Account? DisbursementAccount { get; set; }

    public virtual Account? RepaymentAccount { get; set; }

    public virtual InterestRate? InterestRate { get; set; }

    public virtual Employee? ApprovedByEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Document> Documents { get; set; }
        = new List<Document>();

    public virtual ICollection<LoanSchedule> LoanSchedules { get; set; }
        = new List<LoanSchedule>();

    public virtual ICollection<LoanRepayment> LoanRepayments { get; set; }
        = new List<LoanRepayment>();

    public virtual ICollection<InterestCalculation> InterestCalculations { get; set; }
        = new List<InterestCalculation>();
}
