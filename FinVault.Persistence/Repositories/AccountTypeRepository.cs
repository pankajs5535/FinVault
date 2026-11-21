
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class AccountTypeRepository : GenericRepository<AccountType>, IAccountTypeRepository
{
    public AccountTypeRepository(FinVaultDbContext context) : base(context)
    {
    }

    // =====================================================
    // 1. ACCOUNT TYPE IDENTIFICATION
    // =====================================================

    // Finds an account type using its unique code.
    // Useful for identifying account products during account creation.
    public async Task<AccountType?> GetByCodeAsync(string code)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Code == code);
    }

    // Finds account types using their names.
    // Useful for account product selection and administration.
    public async Task<IEnumerable<AccountType>> GetByNameAsync(string name)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToListAsync();
    }


    // =====================================================
    // 2. ACCOUNT TYPE CATEGORY AND STATUS
    // =====================================================

    // Retrieves account types belonging to a particular category.
    // Useful for displaying products according to their banking category.
    public async Task<IEnumerable<AccountType>> GetByCategoryAsync(string category)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Category == category).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves all active account types.
    // Useful when customers or employees select available account products.
    public async Task<IEnumerable<AccountType>> GetActiveAccountTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves all inactive account types.
    // Useful for administration and historical product review.
    public async Task<IEnumerable<AccountType>> GetInactiveAccountTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => !x.IsActive).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves account types that earn interest.
    // Useful for identifying savings and deposit products eligible for interest.
    public async Task<IEnumerable<AccountType>> GetInterestBearingAccountTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.IsInterestBearing).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves account types that permit overdrafts.
    // Useful for identifying products that support overdraft facilities.
    public async Task<IEnumerable<AccountType>> GetOverdraftAccountTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.AllowsOverdraft).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves account types whose configured tenure overlaps the supplied range.
    // Useful for selecting fixed and recurring deposit products by tenure.
    public async Task<IEnumerable<AccountType>> GetAccountTypesByTenureRangeAsync(int minTenureMonths, int maxTenureMonths)
    {
        if (minTenureMonths < 0 || maxTenureMonths < minTenureMonths)
            throw new ArgumentOutOfRangeException(nameof(minTenureMonths), "Invalid tenure range.");

        return await _dbSet.AsNoTracking()
            .Where(x => x.MinTenureMonths.HasValue && x.MaxTenureMonths.HasValue &&
                        x.MinTenureMonths.Value <= maxTenureMonths &&
                        x.MaxTenureMonths.Value >= minTenureMonths)
            .OrderBy(x => x.MinTenureMonths)
            .ToListAsync();
    }

    // Retrieves recently created account types.
    // Useful for monitoring newly configured banking products.
    public async Task<IEnumerable<AccountType>> GetRecentlyCreatedAccountTypesAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        return await _dbSet.AsNoTracking().OrderByDescending(x => x.CreatedAt).Take(count).ToListAsync();
    }


    // =====================================================
    // 3. ACCOUNT TYPE BUSINESS RELATIONSHIPS
    // =====================================================

    // Retrieves account types that have associated accounts.
    // Useful for identifying products currently used by customers or internal banking operations.
    public async Task<IEnumerable<AccountType>> GetAccountTypesWithAccountsAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.Accounts.Any()).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves account types that have configured interest rates.
    // Useful for interest management and product administration.
    public async Task<IEnumerable<AccountType>> GetAccountTypesWithInterestRatesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.InterestRates.Any()).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves account types that have configured fees.
    // Useful for reviewing charges associated with banking products.
    public async Task<IEnumerable<AccountType>> GetAccountTypesWithFeesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.Fees.Any()).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves an account type with its accounts, interest rates, fees and audit users.
    // Useful for displaying complete account product information.
    public async Task<AccountType?> GetAccountTypeDetailsAsync(int accountTypeId)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Accounts)
            .Include(x => x.InterestRates)
            .Include(x => x.Fees)
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser)
            .FirstOrDefaultAsync(x => x.AccountTypeId == accountTypeId);
    }


    // =====================================================
    // 4. ACCOUNT TYPE SEARCH AND VALIDATION
    // =====================================================

    // Searches account types by code, name or description.
    // Supports optional category, active-status and interest-bearing filters.
    // Pagination prevents loading large account product lists at once.
    public async Task<IEnumerable<AccountType>> SearchAccountTypesAsync(string? searchTerm, string? category, bool? isActive, bool? isInterestBearing, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            query = query.Where(x => x.Code.Contains(searchTerm) || x.Name.Contains(searchTerm) ||
                                     (x.Description != null && x.Description.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(x => x.Category == category.Trim());

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (isInterestBearing.HasValue)
            query = query.Where(x => x.IsInterestBearing == isInterestBearing.Value);

        return await query.OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Checks whether an account type is referenced by accounts, interest rates or fees.
    // Useful before deleting or deactivating an account product.
    public async Task<bool> IsAccountTypeInUseAsync(int accountTypeId)
    {
        return await _dbSet.AnyAsync(x => x.AccountTypeId == accountTypeId &&
            (x.Accounts.Any() || x.InterestRates.Any() || x.Fees.Any()));
    }
}
