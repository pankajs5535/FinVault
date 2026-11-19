using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Fine-grained capability such as Accounts.Read or Payments.Approve that is granted to
roles.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class Permission
{
    // =========================
    // Primary Key
    // =========================

    public int PermissionId { get; set; }

    // =========================
    // Permission Information
    // =========================

    // Unique machine-readable code, e.g. Payments.Approve.
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ModuleName { get; set; } = null!;

    public string? Description { get; set; }

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

    public virtual ICollection<Role> Roles { get; set; }
        = new List<Role>();
}
