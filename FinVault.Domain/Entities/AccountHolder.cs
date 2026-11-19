using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Link between an Account and a Customer, supporting joint accounts, minors with guardians
and authorised signatories.

RELATIONSHIP
- Belongs to Account through AccountId.
- Belongs to Customer through CustomerId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class AccountHolder
{
    // =========================
    // Primary Key
    // =========================

    public int AccountHolderId { get; set; }

    // =========================
    // Holder Details
    // =========================

    // Allowed values: Primary | Joint | Guardian | AuthorizedSignatory
    public string HolderType { get; set; } = "Primary";

    public bool CanOperate { get; set; } = true;

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    // =========================
    // Foreign Keys
    // =========================

    public int AccountId { get; set; }

    public int CustomerId { get; set; }

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

    public virtual Account Account { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
