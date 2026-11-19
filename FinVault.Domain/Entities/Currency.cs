using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
ISO 4217 currency reference data. INR is the base currency of the Indian domestic
banking model; other rows exist only for future extensibility.

RELATIONSHIP
- Optionally belongs to Country through CountryId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Account through CurrencyId.
- Referenced by Transaction through CurrencyId.
*/

public class Currency
{
    // =========================
    // Primary Key
    // =========================

    public int CurrencyId { get; set; }

    // =========================
    // Currency Information
    // =========================

    public string CurrencyCode { get; set; } = null!;

    public string NumericCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public int DecimalPlaces { get; set; } = 2;

    public bool IsBaseCurrency { get; set; }

    public bool IsActive { get; set; } = true;

    // =========================
    // Foreign Keys
    // =========================

    public int? CountryId { get; set; }

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

    public virtual Country? Country { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Account> Accounts { get; set; }
        = new List<Account>();

    public virtual ICollection<Transaction> Transactions { get; set; }
        = new List<Transaction>();
}
