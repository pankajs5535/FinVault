using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Saved payee of a customer for NEFT, RTGS, IMPS, same-bank transfers or UPI. New payees
pass through a cooling period before use.

RELATIONSHIP
- Belongs to Customer through CustomerId.
- Optionally belongs to Account through InternalAccountId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by PaymentRequest through BeneficiaryId.
*/

public class Beneficiary
{
    // =========================
    // Primary Key
    // =========================

    public int BeneficiaryId { get; set; }

    // =========================
    // Beneficiary Details
    // =========================

    public string BeneficiaryName { get; set; } = null!;

    public string? Nickname { get; set; }

    // Allowed values: IntraBank | InterBank | UPI
    public string BeneficiaryType { get; set; } = "InterBank";

    public string? AccountNumber { get; set; }

    public string? IfscCode { get; set; }

    public string? BankName { get; set; }

    public string? BranchName { get; set; }

    public string? UpiVpa { get; set; }

    public string? MobileNumber { get; set; }

    // =========================
    // Status and Limits
    // =========================

    // Allowed values: Pending | Active | Blocked | Removed
    public string Status { get; set; } = "Pending";

    public DateTime? ActivatedAt { get; set; }

    public DateTime? CoolingPeriodEndsAt { get; set; }

    public bool IsFavorite { get; set; }

    public decimal? PerTransactionLimit { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int CustomerId { get; set; }

    public int? InternalAccountId { get; set; }

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

    public virtual Account? InternalAccount { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<PaymentRequest> PaymentRequests { get; set; }
        = new List<PaymentRequest>();
}
