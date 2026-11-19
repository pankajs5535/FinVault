using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Bank customer master record (CIF). Holds identity and profile data; PAN, Aadhaar and
CKYC data live in KYCDetails.
A customer may optionally have an internet/mobile banking login (User).

RELATIONSHIP
- Optionally belongs to User through UserId (one-to-one).
- Belongs to Branch through HomeBranchId.
- Optionally belongs to Employee through RelationshipManagerId.
- Belongs to Country through NationalityCountryId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Address through CustomerId.
- Referenced by KYCDetails through CustomerId.
- Referenced by Document through CustomerId.
- Referenced by AccountHolder through CustomerId.
- Referenced by Beneficiary through CustomerId.
- Referenced by PaymentRequest through CustomerId.
- Referenced by Card through CustomerId.
- Referenced by Loan through CustomerId.
- Referenced by Dispute through CustomerId.
- Referenced by RiskAssessment through CustomerId.
- Referenced by FraudAlert through CustomerId.
- Referenced by Notification through CustomerId.
- Referenced by ComplianceRecord through CustomerId.
- Referenced by AIConversation through CustomerId.
*/

public class Customer
{
    // =========================
    // Primary Key
    // =========================

    public int CustomerId { get; set; }

    // =========================
    // Identity
    // =========================

    // Unique customer information file (CIF) number.
    public string CustomerNumber { get; set; } = null!;

    // Allowed values: Individual | Minor | Business
    public string CustomerType { get; set; } = "Individual";

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    // Allowed values: Male | Female | Other
    public string? Gender { get; set; }

    public string? FatherOrSpouseName { get; set; }

    // =========================
    // Contact
    // =========================

    public string MobileNumber { get; set; } = null!;

    public string? AlternateMobileNumber { get; set; }

    public string? Email { get; set; }

    // =========================
    // Profile
    // =========================

    public string? Occupation { get; set; }

    public decimal? AnnualIncome { get; set; }

    public string PreferredLanguage { get; set; } = "en";

    // =========================
    // Status
    // =========================

    // Allowed values: Active | Inactive | Dormant | Blocked | Closed
    public string Status { get; set; } = "Active";

    // AML/KYC risk categorisation used for periodic KYC frequency.
    // Allowed values: Low | Medium | High
    public string RiskCategory { get; set; } = "Low";

    public DateTime OnboardedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? UserId { get; set; }

    public int HomeBranchId { get; set; }

    public int? RelationshipManagerId { get; set; }

    public int NationalityCountryId { get; set; }

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

    public virtual User? User { get; set; }

    public virtual Branch HomeBranch { get; set; } = null!;

    public virtual Employee? RelationshipManager { get; set; }

    public virtual Country NationalityCountry { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Address> Addresses { get; set; }
        = new List<Address>();

    public virtual ICollection<KYCDetails> KYCDetailsHistory { get; set; }
        = new List<KYCDetails>();

    public virtual ICollection<Document> Documents { get; set; }
        = new List<Document>();

    public virtual ICollection<AccountHolder> AccountHolders { get; set; }
        = new List<AccountHolder>();

    public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        = new List<Beneficiary>();

    public virtual ICollection<PaymentRequest> PaymentRequests { get; set; }
        = new List<PaymentRequest>();

    public virtual ICollection<Card> Cards { get; set; }
        = new List<Card>();

    public virtual ICollection<Loan> Loans { get; set; }
        = new List<Loan>();

    public virtual ICollection<Dispute> Disputes { get; set; }
        = new List<Dispute>();

    public virtual ICollection<RiskAssessment> RiskAssessments { get; set; }
        = new List<RiskAssessment>();

    public virtual ICollection<FraudAlert> FraudAlerts { get; set; }
        = new List<FraudAlert>();

    public virtual ICollection<Notification> Notifications { get; set; }
        = new List<Notification>();

    public virtual ICollection<ComplianceRecord> ComplianceRecords { get; set; }
        = new List<ComplianceRecord>();

    public virtual ICollection<AIConversation> AIConversations { get; set; }
        = new List<AIConversation>();
}
