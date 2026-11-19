using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Named set of permissions (for example Customer, Teller, BranchManager,
ComplianceOfficer, Administrator) used for role-based access control.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Many-to-many with Permission through the RolePermissions join table.
- Referenced by User through RoleId.
- Each User has exactly one Role (single-role model; see Design_Decisions).
*/

public class Role
{
    // =========================
    // Primary Key
    // =========================

    public int RoleId { get; set; }

    // =========================
    // Role Information
    // =========================

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    // System roles cannot be edited or deleted from the UI.
    public bool IsSystemRole { get; set; }

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

    public virtual ICollection<User> Users { get; set; }
        = new List<User>();

    public virtual ICollection<Permission> Permissions { get; set; }
        = new List<Permission>();
}
