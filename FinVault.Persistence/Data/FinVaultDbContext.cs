using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Data
{
    public class FinVaultDbContext : DbContext
    {
        public FinVaultDbContext(DbContextOptions<FinVaultDbContext> options)
            : base(options)
        {
        }


        // Identity & Security
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


        // Customer & KYC
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<KYCDetails> KYCDetails { get; set; }
        public DbSet<Document> Documents { get; set; }

        // Branch Management
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Employee> Employees { get; set; }

        // Account Management
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }

        // Beneficiary
        public DbSet<Beneficiary> Beneficiaries { get; set; }

        // Transactions
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }

        // Ledger
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

        // Payments
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<PaymentGatewayLog> PaymentGatewayLogs { get; set; }

        // Cards
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardTransaction> CardTransactions { get; set; }

        // Loans
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanRepayment> LoanRepayments { get; set; }
        public DbSet<LoanSchedule> LoanSchedules { get; set; }

        // Interest & Charges
        public DbSet<InterestRate> InterestRates { get; set; }
        public DbSet<InterestCalculation> InterestCalculations { get; set; }
        public DbSet<Fee> Fees { get; set; }

        // Refunds & Disputes
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Dispute> Disputes { get; set; }

        // Reconciliation
        public DbSet<ReconciliationBatch> ReconciliationBatches { get; set; }
        public DbSet<ReconciliationRecord> ReconciliationRecords { get; set; }

        // Fraud Detection
        public DbSet<FraudRule> FraudRules { get; set; }
        public DbSet<FraudAlert> FraudAlerts { get; set; }
        public DbSet<RiskAssessment> RiskAssessments { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        // Audit & Compliance
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ComplianceRecord> ComplianceRecords { get; set; }

        // Reporting
        public DbSet<ReportConfiguration> ReportConfigurations { get; set; }

        // AI
        public DbSet<AIConversation> AIConversations { get; set; }
        public DbSet<AIChatMessage> AIChatMessages { get; set; }
        public DbSet<AIUsageLog> AIUsageLogs { get; set; }

        // Reference Data
        public DbSet<Country> Countries { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(FinVaultDbContext).Assembly);
        }
    }
}