using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Bank staff member. Linked one-to-one to a User login, assigned to a branch, and used for
maker-checker approvals, KYC verification, loan approval and case assignment.

RELATIONSHIP
- Belongs to User through UserId (one-to-one).
- Belongs to Branch through BranchId.
- Optionally belongs to Employee through ReportsToEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Customer through RelationshipManagerId.
- Referenced by KYCDetails through VerifiedByEmployeeId.
- Referenced by Document through VerifiedByEmployeeId.
- Referenced by Branch through ManagerEmployeeId.
- Referenced by Employee through ReportsToEmployeeId.
- Referenced by Loan through ApprovedByEmployeeId.
- Referenced by Dispute through AssignedToEmployeeId.
- Referenced by FraudAlert through ReviewedByEmployeeId.
- Referenced by ComplianceRecord through FiledByEmployeeId.
- Referenced by AIConversation through EscalatedToEmployeeId.
*/

public class Employee
{
    // =========================
    // Primary Key
    // =========================

    public int EmployeeId { get; set; }

    // =========================
    // Employee Information
    // =========================

    public string EmployeeCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    // =========================
    // Employment
    // =========================

    public DateTime DateOfJoining { get; set; }

    public DateTime? DateOfLeaving { get; set; }

    // Allowed values: Active | OnLeave | Suspended | Resigned | Terminated
    public string EmploymentStatus { get; set; } = "Active";

    // Maximum amount this employee may approve under maker-checker rules.
    public decimal? ApprovalLimit { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int UserId { get; set; }

    public int BranchId { get; set; }

    public int? ReportsToEmployeeId { get; set; }

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

    public virtual User User { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual Employee? ReportsToEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Customer> ManagedCustomers { get; set; }
        = new List<Customer>();

    public virtual ICollection<KYCDetails> VerifiedKYCRecords { get; set; }
        = new List<KYCDetails>();

    public virtual ICollection<Document> VerifiedDocuments { get; set; }
        = new List<Document>();

    public virtual ICollection<Branch> ManagedBranches { get; set; }
        = new List<Branch>();

    public virtual ICollection<Employee> DirectReports { get; set; }
        = new List<Employee>();

    public virtual ICollection<Loan> ApprovedLoans { get; set; }
        = new List<Loan>();

    public virtual ICollection<Dispute> AssignedDisputes { get; set; }
        = new List<Dispute>();

    public virtual ICollection<FraudAlert> ReviewedFraudAlerts { get; set; }
        = new List<FraudAlert>();

    public virtual ICollection<ComplianceRecord> FiledComplianceRecords { get; set; }
        = new List<ComplianceRecord>();

    public virtual ICollection<AIConversation> EscalatedConversations { get; set; }
        = new List<AIConversation>();
}
