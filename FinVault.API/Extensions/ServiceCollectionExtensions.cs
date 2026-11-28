using FinVault.API.Filters;
using FinVault.Application.Interface.IRepositories;
using FinVault.Application.Interface.IServices;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Mapping;
using FinVault.Application.Services;
using FinVault.Infrastructure.Identity;
using FinVault.Persistence.Data;
using FinVault.Persistence.Repositories;
using FinVault.Persistence.UnitOfWork;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinVault.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. DATABASE

        services.AddDbContext<FinVaultDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // 16. AUDIT AND FEE REPOSITORIES

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IFeeRepository, FeeRepository>();

        // 17. REPORT CONFIGURATION REPOSITORY

        services.AddScoped<IReportConfigurationRepository, ReportConfigurationRepository>();

        // 18. UNIT OF WORK

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 19. MASTER DATA AND CUSTOMER SERVICES

        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAddressService, AddressService>();

        // 20. ACCOUNT AND BENEFICIARY SERVICES

        services.AddScoped<IAccountTypeService, AccountTypeService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAccountHolderService, AccountHolderService>();
        services.AddScoped<IBeneficiaryService, BeneficiaryService>();

        // 21. CARD SERVICES

        services.AddScoped<ICardService, CardService>();
        services.AddScoped<ICardTransactionService, CardTransactionService>();

        // 22. TRANSACTION AND PAYMENT SERVICES

        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITransactionTypeService, TransactionTypeService>();
        services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        services.AddScoped<IPaymentRequestService, PaymentRequestService>();
        services.AddScoped<IPaymentGatewayLogService, PaymentGatewayLogService>();
        services.AddScoped<IRefundService, RefundService>();

        // 23. ACCOUNTING AND LOAN SERVICES

        services.AddScoped<IJournalEntryService, JournalEntryService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<ILoanService, LoanService>();

        // 24. KYC AND COMPLIANCE SERVICES

        services.AddScoped<IKYCService, KYCService>();
        services.AddScoped<IComplianceRecordService, ComplianceRecordService>();

        // 25. FRAUD AND RISK SERVICES

        services.AddScoped<IFraudRuleService, FraudRuleService>();
        services.AddScoped<IFraudAlertService, FraudAlertService>();
        services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();

        // 26. RECONCILIATION AND DISPUTE SERVICES

        services.AddScoped<IReconciliationService, ReconciliationService>();
        services.AddScoped<IDisputeService, DisputeService>();

        // 27. NOTIFICATION AND AI SERVICES

        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAIConversationService, AIConversationService>();
        services.AddScoped<IAIChatMessageService, AIChatMessageService>();
        services.AddScoped<IAIUsageLogService, AIUsageLogService>();

        // 28. USER, ROLE AND PERMISSION SERVICES

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // 29. AUDIT, FEE AND REPORT SERVICES

        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IFeeService, FeeService>();
        services.AddScoped<IReportConfigurationService, ReportConfigurationService>();

        // 30. IDENTITY AND SECURITY SERVICES

        services.AddFinVaultIdentity(configuration);

        // 31. DATA PROTECTION

        services.AddDataProtection();

        // 32. AUTOMAPPER

        services.AddAutoMapper(cfg => { }, typeof(CountryMappingProfile).Assembly);

        // 33. FLUENTVALIDATION

        services.AddValidatorsFromAssembly(typeof(ICountryService).Assembly);

        // 34. VALIDATION FILTER

        services.AddScoped<ValidationFilter>();

        return services;
    }
}