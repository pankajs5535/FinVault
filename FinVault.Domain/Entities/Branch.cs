using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Physical or digital bank branch with Indian IFSC and MICR identification. Branches can
form a hierarchy (regional office to branch) and have a manager.

RELATIONSHIP
- Belongs to Country through CountryId.
- Optionally belongs to Branch through ParentBranchId.
- Optionally belongs to Employee through ManagerEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Customer through HomeBranchId.
- Referenced by Branch through ParentBranchId.
- Referenced by Employee through BranchId.
- Referenced by Account through BranchId.
- Referenced by Loan through BranchId.
*/

public class Branch
{
    // =========================
    // Primary Key
    // =========================

    public int BranchId { get; set; }

    // =========================
    // Identification
    // =========================

    public string BranchCode { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    // 11-character IFSC: 4 letters, '0', then 6 alphanumerics.
    public string IfscCode { get; set; } = null!;

    public string? MicrCode { get; set; }

    // Allowed values: Regular | Main | Digital | Service
    public string BranchType { get; set; } = "Regular";

    // =========================
    // Address
    // =========================

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string State { get; set; } = null!;

    public string PinCode { get; set; } = null!;

    // =========================
    // Contact
    // =========================

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    // =========================
    // Status
    // =========================

    public bool IsActive { get; set; } = true;

    public DateTime OpenedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int CountryId { get; set; }

    public int? ParentBranchId { get; set; }

    public int? ManagerEmployeeId { get; set; }

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

    public virtual Country Country { get; set; } = null!;

    public virtual Branch? ParentBranch { get; set; }

    public virtual Employee? ManagerEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Customer> Customers { get; set; }
        = new List<Customer>();

    public virtual ICollection<Branch> ChildBranches { get; set; }
        = new List<Branch>();

    public virtual ICollection<Employee> Employees { get; set; }
        = new List<Employee>();

    public virtual ICollection<Account> Accounts { get; set; }
        = new List<Account>();

    public virtual ICollection<Loan> Loans { get; set; }
        = new List<Loan>();
}
