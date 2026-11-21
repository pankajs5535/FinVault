using FinVault.Application.Interface.IRepositories;
using FinVault.Persistence.Data;
using FinVault.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinVault.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        // 1. DATABASE


        services.AddDbContext<FinVaultDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        // 2. GENERIC REPOSITORY


        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


        // 3. MASTER DATA REPOSITORIES


        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IAccountTypeRepository, AccountTypeRepository>();
        services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();


        // 4. CUSTOMER AND ACCOUNT REPOSITORIES


        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountHolderRepository, AccountHolderRepository>();
        services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();


        // 5. CARD REPOSITORIES


        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardTransactionRepository, CardTransactionRepository>();


        // 6. TRANSACTION AND PAYMENT REPOSITORIES


        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IPaymentRequestRepository, PaymentRequestRepository>();
        services.AddScoped<IPaymentGatewayLogRepository, PaymentGatewayLogRepository>();
        services.AddScoped<IRefundRepository, RefundRepository>();


        // 7. ACCOUNTING REPOSITORIES


        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IJournalEntryLineRepository, JournalEntryLineRepository>();
        services.AddScoped<IInterestRateRepository, InterestRateRepository>();
        services.AddScoped<IInterestCalculationRepository, InterestCalculationRepository>();


        // 8. LOAN REPOSITORIES


        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ILoanRepaymentRepository, LoanRepaymentRepository>();
        services.AddScoped<ILoanScheduleRepository, LoanScheduleRepository>();


        // 9. KYC AND DOCUMENT REPOSITORIES


        services.AddScoped<IKYCDetailsRepository, KYCDetailsRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();


        // 10. FRAUD AND RISK REPOSITORIES


        services.AddScoped<IFraudRuleRepository, FraudRuleRepository>();
        services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();
        services.AddScoped<IRiskAssessmentRepository, RiskAssessmentRepository>();


        // 11. RECONCILIATION REPOSITORIES


        services.AddScoped<IReconciliationBatchRepository, ReconciliationBatchRepository>();
        services.AddScoped<IReconciliationRecordRepository, ReconciliationRecordRepository>();


        // 12. DISPUTE REPOSITORY


        services.AddScoped<IDisputeRepository, DisputeRepository>();


        // 13. NOTIFICATION REPOSITORIES


        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();


        // 14. AI REPOSITORIES


        services.AddScoped<IAIConversationRepository, AIConversationRepository>();
        services.AddScoped<IAIChatMessageRepository, AIChatMessageRepository>();
        services.AddScoped<IAIUsageLogRepository, AIUsageLogRepository>();


        // 15. USER, ROLE AND PERMISSION REPOSITORIES


        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


        // 16. AUDIT AND FEE REPOSITORIES


        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IFeeRepository, FeeRepository>();

        return services;
    }
}
