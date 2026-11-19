using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Product definition for accounts: Savings, Current, Fixed Deposit, Recurring Deposit,
plus Internal ledger (GL) accounts used by double-entry posting.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Account through AccountTypeId.
- Referenced by InterestRate through AccountTypeId.
- Referenced by Fee through AccountTypeId.
- Internal accounts (GL, suspense, fee income, tax payable) use Category = Internal
  and have no AccountHolder rows.
*/

public class AccountType
{
    // =========================
    // Primary Key
    // =========================

    public int AccountTypeId { get; set; }

    // =========================
    // Product Information
    // =========================

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    // Allowed values: Savings | Current | FixedDeposit | RecurringDeposit | Internal
    public string Category { get; set; } = null!;

    public string? Description { get; set; }

    // =========================
    // Rules and Limits
    // =========================

    public decimal MinimumBalance { get; set; }

    public decimal MinimumAverageBalance { get; set; }

    public decimal? DailyTransactionLimit { get; set; }

    public bool AllowsOverdraft { get; set; }

    public bool IsInterestBearing { get; set; }

    // Allowed values: Monthly | Quarterly | HalfYearly | Yearly | AtMaturity
    public string? InterestPayoutFrequency { get; set; }

    public int? MinTenureMonths { get; set; }

    public int? MaxTenureMonths { get; set; }

    // =========================
    // Status
    // =========================

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

    public virtual ICollection<Account> Accounts { get; set; }
        = new List<Account>();

    public virtual ICollection<InterestRate> InterestRates { get; set; }
        = new List<InterestRate>();

    public virtual ICollection<Fee> Fees { get; set; }
        = new List<Fee>();
}
