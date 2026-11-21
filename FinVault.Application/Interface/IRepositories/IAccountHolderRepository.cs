
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories;

public interface IAccountHolderRepository : IGenericRepository<AccountHolder>
{
    // =====================================================
    // 1. ACCOUNT HOLDER IDENTIFICATION
    // =====================================================

    // Finds an account holder using the account and customer IDs.
    // Useful for verifying whether a customer is associated with an account.
    Task<AccountHolder?> GetByAccountAndCustomerAsync(int accountId, int customerId);

    // Retrieves an account holder with account, customer and audit user details.
    // Useful for displaying complete account holder information.
    Task<AccountHolder?> GetAccountHolderDetailsAsync(int accountHolderId);


    // =====================================================
    // 2. ACCOUNT AND CUSTOMER RELATIONSHIPS
    // =====================================================

    // Retrieves all account holders associated with an account.
    // Useful for displaying primary holders, joint holders and authorized signatories.
    Task<IEnumerable<AccountHolder>> GetAccountHoldersByAccountIdAsync(int accountId);

    // Retrieves all account holder records associated with a customer.
    // Useful for displaying accounts in which a customer has a holder relationship.
    Task<IEnumerable<AccountHolder>> GetAccountHoldersByCustomerIdAsync(int customerId);

    // Retrieves active account holders.
    // Useful for identifying current account ownership and operating relationships.
    Task<IEnumerable<AccountHolder>> GetActiveAccountHoldersAsync();

    // Retrieves inactive account holders.
    // Useful for reviewing previous account ownership and holder history.
    Task<IEnumerable<AccountHolder>> GetInactiveAccountHoldersAsync();


    // =====================================================
    // 3. ACCOUNT HOLDER TYPES AND PERMISSIONS
    // =====================================================

    // Retrieves primary account holders.
    // Useful for identifying the principal customer responsible for an account.
    Task<IEnumerable<AccountHolder>> GetPrimaryAccountHoldersAsync();

    // Retrieves joint account holders.
    // Useful for reviewing accounts operated or owned by multiple customers.
    Task<IEnumerable<AccountHolder>> GetJointAccountHoldersAsync();

    // Retrieves guardian account holders.
    // Useful for reviewing accounts managed by guardians on behalf of customers.
    Task<IEnumerable<AccountHolder>> GetGuardianAccountHoldersAsync();

    // Retrieves authorized signatories.
    // Useful for identifying customers authorized to operate or sign for accounts.
    Task<IEnumerable<AccountHolder>> GetAuthorizedSignatoriesAsync();

    // Retrieves active account holders who are permitted to operate an account.
    // Useful for checking which customers can perform account operations.
    Task<IEnumerable<AccountHolder>> GetAccountOperatorsAsync(int accountId);


    // =====================================================
    // 4. ACCOUNT HOLDER EFFECTIVE PERIOD
    // =====================================================

    // Retrieves account holders whose authorization is effective on a given date.
    // Useful for verifying account ownership and operating permissions at a specific time.
    Task<IEnumerable<AccountHolder>> GetAccountHoldersEffectiveOnDateAsync(DateTime effectiveDate);

    // Retrieves account holders whose effective period ends within a date range.
    // Useful for monitoring expiring or scheduled-to-end account holder relationships.
    Task<IEnumerable<AccountHolder>> GetAccountHoldersEndingWithinRangeAsync(DateTime startDate, DateTime endDate);


    // =====================================================
    // 5. ACCOUNT HOLDER SEARCH AND VALIDATION
    // =====================================================

    // Searches account holders using account, customer and holder type filters.
    // Supports optional active-status and operating-permission filters.
    // Pagination prevents loading large account holder lists at once.
    Task<IEnumerable<AccountHolder>> SearchAccountHoldersAsync(int? accountId, int? customerId, string? holderType, bool? isActive, bool? canOperate, int pageNumber, int pageSize);

    // Checks whether a customer is an active holder of a particular account.
    // Useful before allowing account operations based on customer ownership.
    Task<bool> IsActiveAccountHolderAsync(int accountId, int customerId);

    // Checks whether a customer is permitted to operate a particular account.
    // Useful for validating account operation permissions.
    Task<bool> CanCustomerOperateAccountAsync(int accountId, int customerId);
}
