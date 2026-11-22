using FinVault.Application.Interface.IRepositories;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Persistence.Data;
using FinVault.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinVault.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly FinVaultDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(FinVaultDbContext context)
        {
            _context = context;

            // ============================================================
            // 1. MASTER DATA REPOSITORIES
            // ============================================================

            Country = new CountryRepository(_context);
            Currency = new CurrencyRepository(_context);
            Branch = new BranchRepository(_context);
            AccountType = new AccountTypeRepository(_context);
            TransactionType = new TransactionTypeRepository(_context);

            // ============================================================
            // 2. CUSTOMER AND ACCOUNT REPOSITORIES
            // ============================================================

            Customer = new CustomerRepository(_context);
            Address = new AddressRepository(_context);
            Account = new AccountRepository(_context);
            AccountHolder = new AccountHolderRepository(_context);
            Beneficiary = new BeneficiaryRepository(_context);

            // ============================================================
            // 3. CARD REPOSITORIES
            // ============================================================

            Card = new CardRepository(_context);
            CardTransaction = new CardTransactionRepository(_context);

            // ============================================================
            // 4. TRANSACTION AND PAYMENT REPOSITORIES
            // ============================================================

            Transaction = new TransactionRepository(_context);
            PaymentMethod = new PaymentMethodRepository(_context);
            PaymentRequest = new PaymentRequestRepository(_context);
            PaymentGatewayLog = new PaymentGatewayLogRepository(_context);
            Refund = new RefundRepository(_context);

            // ============================================================
            // 5. ACCOUNTING REPOSITORIES
            // ============================================================

            JournalEntry = new JournalEntryRepository(_context);
            JournalEntryLine = new JournalEntryLineRepository(_context);
            InterestRate = new InterestRateRepository(_context);
            InterestCalculation = new InterestCalculationRepository(_context);

            // ============================================================
            // 6. LOAN REPOSITORIES
            // ============================================================

            Loan = new LoanRepository(_context);
            LoanRepayment = new LoanRepaymentRepository(_context);
            LoanSchedule = new LoanScheduleRepository(_context);

            // ============================================================
            // 7. KYC AND DOCUMENT REPOSITORIES
            // ============================================================

            KYCDetails = new KYCDetailsRepository(_context);
            Document = new DocumentRepository(_context);
            ComplianceRecord = new ComplianceRecordRepository(_context);

            // ============================================================
            // 8. FRAUD AND RISK REPOSITORIES
            // ============================================================

            FraudRule = new FraudRuleRepository(_context);
            FraudAlert = new FraudAlertRepository(_context);
            RiskAssessment = new RiskAssessmentRepository(_context);

            // ============================================================
            // 9. RECONCILIATION REPOSITORIES
            // ============================================================

            ReconciliationBatch = new ReconciliationBatchRepository(_context);
            ReconciliationRecord = new ReconciliationRecordRepository(_context);

            // ============================================================
            // 10. DISPUTE REPOSITORY
            // ============================================================

            Dispute = new DisputeRepository(_context);

            // ============================================================
            // 11. NOTIFICATION REPOSITORIES
            // ============================================================

            Notification = new NotificationRepository(_context);
            NotificationTemplate = new NotificationTemplateRepository(_context);

            // ============================================================
            // 12. AI REPOSITORIES
            // ============================================================

            AIConversation = new AIConversationRepository(_context);
            AIChatMessage = new AIChatMessageRepository(_context);
            AIUsageLog = new AIUsageLogRepository(_context);

            // ============================================================
            // 13. USER, ROLE AND PERMISSION REPOSITORIES
            // ============================================================

            User = new UserRepository(_context);
            Role = new RoleRepository(_context);
            Permission = new PermissionRepository(_context);
            RefreshToken = new RefreshTokenRepository(_context);

            // ============================================================
            // 14. AUDIT AND FEE REPOSITORIES
            // ============================================================

            AuditLog = new AuditLogRepository(_context);
            Fee = new FeeRepository(_context);

            // ============================================================
            // 15. REPORT CONFIGURATION REPOSITORY
            // ============================================================

            ReportConfiguration = new ReportConfigurationRepository(_context);
        }

        // ============================================================
        // 16. MASTER DATA REPOSITORY PROPERTIES
        // ============================================================

        public ICountryRepository Country { get; }
        public ICurrencyRepository Currency { get; }
        public IBranchRepository Branch { get; }
        public IAccountTypeRepository AccountType { get; }
        public ITransactionTypeRepository TransactionType { get; }

        // ============================================================
        // 17. CUSTOMER AND ACCOUNT REPOSITORY PROPERTIES
        // ============================================================

        public ICustomerRepository Customer { get; }
        public IAddressRepository Address { get; }
        public IAccountRepository Account { get; }
        public IAccountHolderRepository AccountHolder { get; }
        public IBeneficiaryRepository Beneficiary { get; }

        // ============================================================
        // 18. CARD REPOSITORY PROPERTIES
        // ============================================================

        public ICardRepository Card { get; }
        public ICardTransactionRepository CardTransaction { get; }

        // ============================================================
        // 19. TRANSACTION AND PAYMENT REPOSITORY PROPERTIES
        // ============================================================

        public ITransactionRepository Transaction { get; }
        public IPaymentMethodRepository PaymentMethod { get; }
        public IPaymentRequestRepository PaymentRequest { get; }
        public IPaymentGatewayLogRepository PaymentGatewayLog { get; }
        public IRefundRepository Refund { get; }

        // ============================================================
        // 20. ACCOUNTING REPOSITORY PROPERTIES
        // ============================================================

        public IJournalEntryRepository JournalEntry { get; }
        public IJournalEntryLineRepository JournalEntryLine { get; }
        public IInterestRateRepository InterestRate { get; }
        public IInterestCalculationRepository InterestCalculation { get; }

        // ============================================================
        // 21. LOAN REPOSITORY PROPERTIES
        // ============================================================

        public ILoanRepository Loan { get; }
        public ILoanRepaymentRepository LoanRepayment { get; }
        public ILoanScheduleRepository LoanSchedule { get; }

        // ============================================================
        // 22. KYC AND DOCUMENT REPOSITORY PROPERTIES
        // ============================================================

        public IKYCDetailsRepository KYCDetails { get; }
        public IDocumentRepository Document { get; }
        public IComplianceRecordRepository ComplianceRecord { get; }

        // ============================================================
        // 23. FRAUD AND RISK REPOSITORY PROPERTIES
        // ============================================================

        public IFraudRuleRepository FraudRule { get; }
        public IFraudAlertRepository FraudAlert { get; }
        public IRiskAssessmentRepository RiskAssessment { get; }

        // ============================================================
        // 24. RECONCILIATION REPOSITORY PROPERTIES
        // ============================================================

        public IReconciliationBatchRepository ReconciliationBatch { get; }
        public IReconciliationRecordRepository ReconciliationRecord { get; }

        // ============================================================
        // 25. DISPUTE REPOSITORY PROPERTY
        // ============================================================

        public IDisputeRepository Dispute { get; }

        // ============================================================
        // 26. NOTIFICATION REPOSITORY PROPERTIES
        // ============================================================

        public INotificationRepository Notification { get; }
        public INotificationTemplateRepository NotificationTemplate { get; }

        // ============================================================
        // 27. AI REPOSITORY PROPERTIES
        // ============================================================

        public IAIConversationRepository AIConversation { get; }
        public IAIChatMessageRepository AIChatMessage { get; }
        public IAIUsageLogRepository AIUsageLog { get; }

        // ============================================================
        // 28. USER, ROLE AND PERMISSION REPOSITORY PROPERTIES
        // ============================================================

        public IUserRepository User { get; }
        public IRoleRepository Role { get; }
        public IPermissionRepository Permission { get; }
        public IRefreshTokenRepository RefreshToken { get; }

        // ============================================================
        // 29. AUDIT AND FEE REPOSITORY PROPERTIES
        // ============================================================

        public IAuditLogRepository AuditLog { get; }
        public IFeeRepository Fee { get; }

        // ============================================================
        // 30. REPORT CONFIGURATION REPOSITORY PROPERTY
        // ============================================================

        public IReportConfigurationRepository ReportConfiguration { get; }

        // ============================================================
        // 31. SAVE DATABASE CHANGES
        // ============================================================

        // Saves all pending changes tracked by the shared DbContext.
        // Useful for persisting multiple repository operations together.
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // ============================================================
        // 32. BEGIN DATABASE TRANSACTION
        // ============================================================

        // Begins a new database transaction.
        // Useful when multiple banking operations must succeed or fail together.
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A database transaction is already active.");
            }

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        // ============================================================
        // 33. COMMIT DATABASE TRANSACTION
        // ============================================================

        // Commits the active database transaction.
        // Useful after all database changes have been successfully saved.
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active database transaction exists.");
            }

            try
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // ============================================================
        // 34. ROLLBACK DATABASE TRANSACTION
        // ============================================================

        // Rolls back the active database transaction.
        // Useful for preventing partial database updates when an operation fails.
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active database transaction exists.");
            }

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // ============================================================
        // 35. DISPOSE DATABASE RESOURCES
        // ============================================================

        // Disposes the active transaction synchronously.
        // Useful for releasing database transaction resources when the Unit of Work is disposed.
        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null;
        }

        // Disposes the active transaction asynchronously.
        // Useful for releasing database transaction resources without blocking the calling thread.
        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
