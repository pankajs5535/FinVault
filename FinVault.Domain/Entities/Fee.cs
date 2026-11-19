using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Bank charge definition (SMS charges, NEFT/RTGS/IMPS fees, card annual fee, cheque-book
fee, loan processing fee, penal charges) with GST applicability.

RELATIONSHIP
- Optionally belongs to TransactionType through TransactionTypeId.
- Optionally belongs to PaymentMethod through PaymentMethodId.
- Optionally belongs to AccountType through AccountTypeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Transaction through FeeId.
*/

public class Fee
{
    // =========================
    // Primary Key
    // =========================

    public int FeeId { get; set; }

    // =========================
    // Fee Information
    // =========================

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    // Allowed values: Flat | Percentage | Slab
    public string CalculationMethod { get; set; } = "Flat";

    // Allowed values: PerTransaction | Monthly | Quarterly | Yearly | OneTime
    public string Frequency { get; set; } = "PerTransaction";

    // =========================
    // Amounts
    // =========================

    public decimal? FlatAmount { get; set; }

    public decimal? Percentage { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }

    // =========================
    // Tax
    // =========================

    public bool IsGstApplicable { get; set; } = true;

    public decimal GstRate { get; set; }

    // =========================
    // Validity
    // =========================

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    // =========================
    // Foreign Keys
    // =========================

    public int? TransactionTypeId { get; set; }

    public int? PaymentMethodId { get; set; }

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

    public virtual TransactionType? TransactionType { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual AccountType? AccountType { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Transaction> ChargedTransactions { get; set; }
        = new List<Transaction>();
}
