
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories;

public interface IAccountRepository : IGenericRepository<Account>
{
    // =====================================================
    // 1. ACCOUNT IDENTIFICATION
    // =====================================================

    // Finds an account using its unique account number.
    // Useful for customer account identification and banking operations.
    Task<Account?> GetByAccountNumberAsync(string accountNumber);

    // Retrieves an account with its account type, branch and currency.
    // Useful for displaying complete account information.
    Task<Account?> GetAccountDetailsAsync(int accountId);


    // =====================================================
    // 2. ACCOUNT STATUS AND BALANCES
    // =====================================================

    // Retrieves accounts having a particular status.
    // Statuses: Pending, Active, Dormant, Inactive, Frozen and Closed.
    // Useful for account lifecycle management and operational monitoring.
    Task<IEnumerable<Account>> GetAccountsByStatusAsync(string status);

    // Retrieves all active accounts.
    // Useful for identifying accounts currently available for banking operations.
    Task<IEnumerable<Account>> GetActiveAccountsAsync();

    // Retrieves accounts belonging to a particular branch.
    // Useful for branch-level account administration and reporting.
    Task<IEnumerable<Account>> GetAccountsByBranchIdAsync(int branchId);

    // Retrieves accounts belonging to a particular account type.
    // Useful for savings, current and deposit product management.
    Task<IEnumerable<Account>> GetAccountsByAccountTypeIdAsync(int accountTypeId);

    // Retrieves accounts maintained in a particular currency.
    // Useful for currency-based account reporting and administration.
    Task<IEnumerable<Account>> GetAccountsByCurrencyIdAsync(int currencyId);

    // Retrieves accounts whose available balance is below the supplied amount.
    // Useful for low-balance monitoring and customer notification workflows.
    Task<IEnumerable<Account>> GetAccountsWithLowAvailableBalanceAsync(decimal threshold);

    // Retrieves accounts with an outstanding hold amount.
    // Useful for monitoring funds reserved for pending transactions or other obligations.
    Task<IEnumerable<Account>> GetAccountsWithHoldsAsync();


    // =====================================================
    // 3. ACCOUNT HOLDERS AND RELATIONSHIPS
    // =====================================================

    // Retrieves accounts belonging to a particular customer.
    // Useful for displaying all accounts associated with a customer.
    Task<IEnumerable<Account>> GetAccountsByCustomerIdAsync(int customerId);

    // Retrieves accounts that have more than one active account holder.
    // Useful for identifying joint accounts and their operating arrangements.
    Task<IEnumerable<Account>> GetJointAccountsAsync();

    // Retrieves accounts linked to a particular payout account.
    // Useful for reviewing deposit accounts that transfer maturity or interest proceeds.
    Task<IEnumerable<Account>> GetLinkedDepositAccountsAsync(int payoutAccountId);


    // =====================================================
    // 4. DEPOSIT MATURITY AND ACCOUNT ACTIVITY
    // =====================================================

    // Retrieves deposit accounts maturing within the supplied date range.
    // Useful for maturity notifications and deposit renewal processing.
    Task<IEnumerable<Account>> GetAccountsByMaturityDateRangeAsync(DateTime startDate, DateTime endDate);

    // Retrieves accounts that have reached maturity and are not closed.
    // Useful for identifying deposits requiring maturity settlement or renewal.
    Task<IEnumerable<Account>> GetMaturedDepositAccountsAsync(DateTime asOfDate);

    // Retrieves accounts ordered by their most recent transaction date.
    // Useful for monitoring account activity and identifying inactive accounts.
    Task<IEnumerable<Account>> GetRecentlyActiveAccountsAsync(int count);


    // =====================================================
    // 5. ACCOUNT SEARCH AND VALIDATION
    // =====================================================

    // Searches accounts by account number, nominee name or status.
    // Supports optional branch, account type, currency and status filters.
    // Pagination prevents loading large account lists at once.
    Task<IEnumerable<Account>> SearchAccountsAsync(string? searchTerm, int? branchId, int? accountTypeId, int? currencyId, string? status, int pageNumber, int pageSize);

    // Checks whether an account is referenced by other banking records.
    // Useful before account deletion or closure.
    Task<bool> IsAccountInUseAsync(int accountId);
}
