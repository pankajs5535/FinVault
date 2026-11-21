
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories;

public interface IAccountTypeRepository : IGenericRepository<AccountType>
{
    // =====================================================
    // 1. ACCOUNT TYPE IDENTIFICATION
    // =====================================================

    // Finds an account type using its unique code.
    // Useful for identifying account products during account creation.
    Task<AccountType?> GetByCodeAsync(string code);

    // Finds account types using their names.
    // Useful for account product selection and administration.
    Task<IEnumerable<AccountType>> GetByNameAsync(string name);


    // =====================================================
    // 2. ACCOUNT TYPE CATEGORY AND STATUS
    // =====================================================

    // Retrieves account types belonging to a particular category.
    // Categories: Savings, Current, FixedDeposit, RecurringDeposit and Internal.
    // Useful for displaying products according to their banking category.
    Task<IEnumerable<AccountType>> GetByCategoryAsync(string category);

    // Retrieves all active account types.
    // Useful when customers or employees select available account products.
    Task<IEnumerable<AccountType>> GetActiveAccountTypesAsync();

    // Retrieves all inactive account types.
    // Useful for administration and historical product review.
    Task<IEnumerable<AccountType>> GetInactiveAccountTypesAsync();

    // Retrieves account types that earn interest.
    // Useful for identifying savings and deposit products eligible for interest.
    Task<IEnumerable<AccountType>> GetInterestBearingAccountTypesAsync();

    // Retrieves account types that permit overdrafts.
    // Useful for identifying products that support overdraft facilities.
    Task<IEnumerable<AccountType>> GetOverdraftAccountTypesAsync();

    // Retrieves account types whose configured tenure overlaps the supplied range.
    // Useful for selecting fixed and recurring deposit products by tenure.
    Task<IEnumerable<AccountType>> GetAccountTypesByTenureRangeAsync(int minTenureMonths, int maxTenureMonths);

    // Retrieves recently created account types.
    // Useful for monitoring newly configured banking products.
    Task<IEnumerable<AccountType>> GetRecentlyCreatedAccountTypesAsync(int count);


    // =====================================================
    // 3. ACCOUNT TYPE BUSINESS RELATIONSHIPS
    // =====================================================

    // Retrieves account types that have associated accounts.
    // Useful for identifying products currently used by customers or internal banking operations.
    Task<IEnumerable<AccountType>> GetAccountTypesWithAccountsAsync();

    // Retrieves account types that have configured interest rates.
    // Useful for interest management and product administration.
    Task<IEnumerable<AccountType>> GetAccountTypesWithInterestRatesAsync();

    // Retrieves account types that have configured fees.
    // Useful for reviewing charges associated with banking products.
    Task<IEnumerable<AccountType>> GetAccountTypesWithFeesAsync();

    // Retrieves an account type with its accounts, interest rates, fees and audit users.
    // Useful for displaying complete account product information.
    Task<AccountType?> GetAccountTypeDetailsAsync(int accountTypeId);


    // =====================================================
    // 4. ACCOUNT TYPE SEARCH AND VALIDATION
    // =====================================================

    // Searches account types by code, name or description.
    // Supports optional category, active-status and interest-bearing filters.
    // Pagination prevents loading large account product lists at once.
    Task<IEnumerable<AccountType>> SearchAccountTypesAsync(string? searchTerm, string? category, bool? isActive, bool? isInterestBearing, int pageNumber, int pageSize);

    // Checks whether an account type is referenced by accounts, interest rates or fees.
    // Useful before deleting or deactivating an account product.
    Task<bool> IsAccountTypeInUseAsync(int accountTypeId);
}
