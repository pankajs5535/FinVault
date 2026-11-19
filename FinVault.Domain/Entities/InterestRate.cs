using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Time-bound interest rate card for deposit products (by AccountType) or loan products (by
LoanType), including senior-citizen premium and benchmark-linked (e.g. repo-rate)
spreads.

RELATIONSHIP
- Optionally belongs to AccountType through AccountTypeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Loan through InterestRateId.
- Referenced by InterestCalculation through InterestRateId.
*/

public class InterestRate
{
    // =========================
    // Primary Key
    // =========================

    public int InterestRateId { get; set; }

    // =========================
    // Rate Information
    // =========================

    public string RateName { get; set; } = null!;

    // Allowed values: Savings | Current | FixedDeposit | RecurringDeposit | Loan
    public string ProductCategory { get; set; } = null!;

    public string? LoanType { get; set; }

    public decimal AnnualRate { get; set; }

    public decimal? SeniorCitizenAdditionalRate { get; set; }

    // Allowed values: Simple | Daily | Monthly | Quarterly | HalfYearly | Yearly
    public string CompoundingFrequency { get; set; } = "Quarterly";

    // =========================
    // Applicability
    // =========================

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public int? MinTenureMonths { get; set; }

    public int? MaxTenureMonths { get; set; }

    // =========================
    // Benchmark (floating loans)
    // =========================

    public string? BenchmarkName { get; set; }

    public decimal? SpreadOverBenchmark { get; set; }

    // =========================
    // Validity
    // =========================

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    // =========================
    // Foreign Keys
    // =========================

    public int? AccountTypeId { get; set; }

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

    public virtual AccountType? AccountType { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Loan> Loans { get; set; }
        = new List<Loan>();

    public virtual ICollection<InterestCalculation> InterestCalculations { get; set; }
        = new List<InterestCalculation>();
}
