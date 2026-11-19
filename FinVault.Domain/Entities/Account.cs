using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Bank account (customer deposit account or internal ledger account) holding balances and
lifecycle state.
Balance and AvailableBalance are maintained by the posting service inside the same
database transaction as the journal entry; RowVersion guards against lost updates.

RELATIONSHIP
- Belongs to AccountType through AccountTypeId.
- Belongs to Branch through BranchId.
- Belongs to Currency through CurrencyId.
- Optionally belongs to Account through PayoutAccountId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Account through PayoutAccountId.
- Referenced by AccountHolder through AccountId.
- Referenced by Beneficiary through InternalAccountId.
- Referenced by Transaction through SourceAccountId.
- Referenced by Transaction through DestinationAccountId.
- Referenced by JournalEntryLine through AccountId.
- Referenced by PaymentRequest through SourceAccountId.
- Referenced by Card through AccountId.
- Referenced by Loan through DisbursementAccountId.
- Referenced by Loan through RepaymentAccountId.
- Referenced by InterestCalculation through AccountId.
- Referenced by RiskAssessment through AccountId.
- Referenced by FraudAlert through AccountId.
- Referenced by ComplianceRecord through AccountId.
- PayoutAccountId links a deposit to the account that receives interest/maturity
  proceeds.
*/

public class Account
{
    // =========================
    // Primary Key
    // =========================

    public int AccountId { get; set; }

    // =========================
    // Account Information
    // =========================

    // Indian bank account number (9 to 18 digits).
    public string AccountNumber { get; set; } = null!;

    // Allowed values: Pending | Active | Dormant | Inactive | Frozen | Closed
    public string Status { get; set; } = "Pending";

    public string? StatusReason { get; set; }

    // Allowed values: Single | Joint | EitherOrSurvivor | FormerOrSurvivor
    public string ModeOfOperation { get; set; } = "Single";

    // =========================
    // Balances
    // =========================

    // Ledger balance in INR (or account currency).
    public decimal Balance { get; set; }

    public decimal AvailableBalance { get; set; }

    public decimal HoldAmount { get; set; }

    public decimal OverdraftLimit { get; set; }

    // =========================
    // Lifecycle
    // =========================

    public DateTime OpenedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    public DateTime? LastTransactionAt { get; set; }

    // =========================
    // Nominee
    // =========================

    public string? NomineeName { get; set; }

    public string? NomineeRelationship { get; set; }

    // =========================
    // Deposit Terms (Fixed / Recurring Deposits)
    // =========================

    public decimal? PrincipalAmount { get; set; }

    public decimal? MaturityAmount { get; set; }

    public decimal? ContractedInterestRate { get; set; }

    public int? TenureMonths { get; set; }

    public DateTime? MaturityDate { get; set; }

    public bool AutoRenew { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int AccountTypeId { get; set; }

    public int BranchId { get; set; }

    public int CurrencyId { get; set; }

    public int? PayoutAccountId { get; set; }

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

    public virtual AccountType AccountType { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual Currency Currency { get; set; } = null!;

    public virtual Account? PayoutAccount { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Account> LinkedDepositAccounts { get; set; }
        = new List<Account>();

    public virtual ICollection<AccountHolder> AccountHolders { get; set; }
        = new List<AccountHolder>();

    public virtual ICollection<Beneficiary> BeneficiaryReferences { get; set; }
        = new List<Beneficiary>();

    public virtual ICollection<Transaction> OutgoingTransactions { get; set; }
        = new List<Transaction>();

    public virtual ICollection<Transaction> IncomingTransactions { get; set; }
        = new List<Transaction>();

    public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; }
        = new List<JournalEntryLine>();

    public virtual ICollection<PaymentRequest> PaymentRequests { get; set; }
        = new List<PaymentRequest>();

    public virtual ICollection<Card> Cards { get; set; }
        = new List<Card>();

    public virtual ICollection<Loan> DisbursementLoans { get; set; }
        = new List<Loan>();

    public virtual ICollection<Loan> RepaymentLoans { get; set; }
        = new List<Loan>();

    public virtual ICollection<InterestCalculation> InterestCalculations { get; set; }
        = new List<InterestCalculation>();

    public virtual ICollection<RiskAssessment> RiskAssessments { get; set; }
        = new List<RiskAssessment>();

    public virtual ICollection<FraudAlert> FraudAlerts { get; set; }
        = new List<FraudAlert>();

    public virtual ICollection<ComplianceRecord> ComplianceRecords { get; set; }
        = new List<ComplianceRecord>();
}
