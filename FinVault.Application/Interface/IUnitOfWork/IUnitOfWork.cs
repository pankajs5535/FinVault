
using FinVault.Application.Interface.IRepositories;

namespace FinVault.Application.Interface.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // ============================================================
        // 1. MASTER DATA REPOSITORIES
        // ============================================================

        ICountryRepository Country { get; }
        ICurrencyRepository Currency { get; }
        IBranchRepository Branch { get; }
        IAccountTypeRepository AccountType { get; }
        ITransactionTypeRepository TransactionType { get; }

        // ============================================================
        // 2. CUSTOMER AND ACCOUNT REPOSITORIES
        // ============================================================

        ICustomerRepository Customer { get; }
        IAddressRepository Address { get; }
        IAccountRepository Account { get; }
        IAccountHolderRepository AccountHolder { get; }
        IBeneficiaryRepository Beneficiary { get; }

        // ============================================================
        // 3. CARD REPOSITORIES
        // ============================================================

        ICardRepository Card { get; }
        ICardTransactionRepository CardTransaction { get; }

        // ============================================================
        // 4. TRANSACTION AND PAYMENT REPOSITORIES
        // ============================================================

        ITransactionRepository Transaction { get; }
        IPaymentMethodRepository PaymentMethod { get; }
        IPaymentRequestRepository PaymentRequest { get; }
        IPaymentGatewayLogRepository PaymentGatewayLog { get; }
        IRefundRepository Refund { get; }

        // ============================================================
        // 5. ACCOUNTING REPOSITORIES
        // ============================================================

        IJournalEntryRepository JournalEntry { get; }
        IJournalEntryLineRepository JournalEntryLine { get; }
        IInterestRateRepository InterestRate { get; }
        IInterestCalculationRepository InterestCalculation { get; }

        // ============================================================
        // 6. LOAN REPOSITORIES
        // ============================================================

        ILoanRepository Loan { get; }
        ILoanRepaymentRepository LoanRepayment { get; }
        ILoanScheduleRepository LoanSchedule { get; }

        // ============================================================
        // 7. KYC AND DOCUMENT REPOSITORIES
        // ============================================================

        IKYCDetailsRepository KYCDetails { get; }
        IDocumentRepository Document { get; }
        IComplianceRecordRepository ComplianceRecord { get; }

        // ============================================================
        // 8. FRAUD AND RISK REPOSITORIES
        // ============================================================

        IFraudRuleRepository FraudRule { get; }
        IFraudAlertRepository FraudAlert { get; }
        IRiskAssessmentRepository RiskAssessment { get; }

        // ============================================================
        // 9. RECONCILIATION REPOSITORIES
        // ============================================================

        IReconciliationBatchRepository ReconciliationBatch { get; }
        IReconciliationRecordRepository ReconciliationRecord { get; }

        // ============================================================
        // 10. DISPUTE REPOSITORY
        // ============================================================

        IDisputeRepository Dispute { get; }

        // ============================================================
        // 11. NOTIFICATION REPOSITORIES
        // ============================================================

        INotificationRepository Notification { get; }
        INotificationTemplateRepository NotificationTemplate { get; }

        // ============================================================
        // 12. AI REPOSITORIES
        // ============================================================

        IAIConversationRepository AIConversation { get; }
        IAIChatMessageRepository AIChatMessage { get; }
        IAIUsageLogRepository AIUsageLog { get; }

        // ============================================================
        // 13. USER, ROLE AND PERMISSION REPOSITORIES
        // ============================================================
        IEmployeeRepository Employee { get; }
        IUserRepository User { get; }
        IRoleRepository Role { get; }
        IPermissionRepository Permission { get; }
        IRefreshTokenRepository RefreshToken { get; }

        // ============================================================
        // 14. AUDIT AND FEE REPOSITORIES
        // ============================================================

        IAuditLogRepository AuditLog { get; }
        IFeeRepository Fee { get; }

        // ============================================================
        // 15. REPORT CONFIGURATION REPOSITORY
        // ============================================================

        IReportConfigurationRepository ReportConfiguration { get; }

        // ============================================================
        // 16. DATABASE OPERATIONS
        // ============================================================

        // Saves all pending changes from the shared DbContext.
        // Useful for committing multiple banking operations together.
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Begins a database transaction.
        // Useful when a banking operation requires multiple database changes to succeed or fail together.
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        // Commits the active database transaction.
        // Useful after all operations in a banking transaction have completed successfully.
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        // Rolls back the active database transaction.
        // Useful for preventing partial updates when a banking operation fails.
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
