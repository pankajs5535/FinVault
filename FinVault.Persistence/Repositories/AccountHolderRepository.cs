
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class AccountHolderRepository : GenericRepository<AccountHolder>, IAccountHolderRepository
{
    public AccountHolderRepository(FinVaultDbContext context) : base(context)
    {
    }

    // =====================================================
    // 1. ACCOUNT HOLDER IDENTIFICATION
    // =====================================================

    // Finds an account holder using the account and customer IDs.
    // Useful for verifying whether a customer is associated with an account.
    public async Task<AccountHolder?> GetByAccountAndCustomerAsync(int accountId, int customerId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountId == accountId && x.CustomerId == customerId);
    }

    // Retrieves an account holder with account, customer and audit user details.
    // Useful for displaying complete account holder information.
    public async Task<AccountHolder?> GetAccountHolderDetailsAsync(int accountHolderId)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Customer)
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser)
            .FirstOrDefaultAsync(x => x.AccountHolderId == accountHolderId);
    }


    // =====================================================
    // 2. ACCOUNT AND CUSTOMER RELATIONSHIPS
    // =====================================================

    // Retrieves all account holders associated with an account.
    // Useful for displaying primary holders, joint holders and authorized signatories.
    public async Task<IEnumerable<AccountHolder>> GetAccountHoldersByAccountIdAsync(int accountId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .Include(x => x.Customer)
            .OrderBy(x => x.HolderType)
            .ToListAsync();
    }

    // Retrieves all account holder records associated with a customer.
    // Useful for displaying accounts in which a customer has a holder relationship.
    public async Task<IEnumerable<AccountHolder>> GetAccountHoldersByCustomerIdAsync(int customerId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Account)
            .OrderByDescending(x => x.EffectiveFrom)
            .ToListAsync();
    }

    // Retrieves active account holders.
    // Useful for identifying current account ownership and operating relationships.
    public async Task<IEnumerable<AccountHolder>> GetActiveAccountHoldersAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.AccountId)
            .ToListAsync();
    }

    // Retrieves inactive account holders.
    // Useful for reviewing previous account ownership and holder history.
    public async Task<IEnumerable<AccountHolder>> GetInactiveAccountHoldersAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => !x.IsActive)
            .OrderByDescending(x => x.EffectiveTo)
            .ToListAsync();
    }


    // =====================================================
    // 3. ACCOUNT HOLDER TYPES AND PERMISSIONS
    // =====================================================

    // Retrieves primary account holders.
    // Useful for identifying the principal customer responsible for an account.
    public async Task<IEnumerable<AccountHolder>> GetPrimaryAccountHoldersAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.HolderType == "Primary" && x.IsActive)
            .Include(x => x.Customer)
            .OrderBy(x => x.AccountId)
            .ToListAsync();
    }

    // Retrieves joint account holders.
    // Useful for reviewing accounts operated or owned by multiple customers.
    public async Task<IEnumerable<AccountHolder>> GetJointAccountHoldersAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.HolderType == "Joint" && x.IsActive)
            .Include(x => x.Account)
            .Include(x => x.Customer)
            .OrderBy(x => x.AccountId)
            .ToListAsync();
    }

    // Retrieves guardian account holders.
    // Useful for reviewing accounts managed by guardians on behalf of customers.
    public async Task<IEnumerable<AccountHolder>> GetGuardianAccountHoldersAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.HolderType == "Guardian" && x.IsActive)
            .Include(x => x.Account)
            .Include(x => x.Customer)
            .OrderBy(x => x.AccountId)
            .ToListAsync();
    }

    // Retrieves authorized signatories.
    // Useful for identifying customers authorized to operate or sign for accounts.
    public async Task<IEnumerable<AccountHolder>> GetAuthorizedSignatoriesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.HolderType == "AuthorizedSignatory" && x.IsActive)
            .Include(x => x.Account)
            .Include(x => x.Customer)
            .OrderBy(x => x.AccountId)
            .ToListAsync();
    }

    // Retrieves active account holders who are permitted to operate an account.
    // Useful for checking which customers can perform account operations.
    public async Task<IEnumerable<AccountHolder>> GetAccountOperatorsAsync(int accountId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AccountId == accountId && x.IsActive && x.CanOperate)
            .Include(x => x.Customer)
            .OrderBy(x => x.HolderType)
            .ToListAsync();
    }


    // =====================================================
    // 4. ACCOUNT HOLDER EFFECTIVE PERIOD
    // =====================================================

    // Retrieves account holders whose authorization is effective on a given date.
    // Useful for verifying account ownership and operating permissions at a specific time.
    public async Task<IEnumerable<AccountHolder>> GetAccountHoldersEffectiveOnDateAsync(DateTime effectiveDate)
    {
        var date = effectiveDate.Date;

        return await _dbSet.AsNoTracking()
            .Where(x => x.IsActive && x.EffectiveFrom <= date &&
                        (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= date))
            .OrderBy(x => x.AccountId)
            .ToListAsync();
    }

    // Retrieves account holders whose effective period ends within a date range.
    // Useful for monitoring expiring or scheduled-to-end account holder relationships.
    public async Task<IEnumerable<AccountHolder>> GetAccountHoldersEndingWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");

        return await _dbSet.AsNoTracking()
            .Where(x => x.IsActive && x.EffectiveTo.HasValue &&
                        x.EffectiveTo.Value >= startDate.Date &&
                        x.EffectiveTo.Value <= endDate.Date)
            .OrderBy(x => x.EffectiveTo)
            .ToListAsync();
    }


    // =====================================================
    // 5. ACCOUNT HOLDER SEARCH AND VALIDATION
    // =====================================================

    // Searches account holders using account, customer and holder type filters.
    // Supports optional active-status and operating-permission filters.
    // Pagination prevents loading large account holder lists at once.
    public async Task<IEnumerable<AccountHolder>> SearchAccountHoldersAsync(int? accountId, int? customerId, string? holderType, bool? isActive, bool? canOperate, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (accountId.HasValue)
            query = query.Where(x => x.AccountId == accountId.Value);

        if (customerId.HasValue)
            query = query.Where(x => x.CustomerId == customerId.Value);

        if (!string.IsNullOrWhiteSpace(holderType))
            query = query.Where(x => x.HolderType == holderType.Trim());

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (canOperate.HasValue)
            query = query.Where(x => x.CanOperate == canOperate.Value);

        return await query.OrderBy(x => x.AccountId)
            .ThenBy(x => x.HolderType)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Checks whether a customer is an active holder of a particular account.
    // Useful before allowing account operations based on customer ownership.
    public async Task<bool> IsActiveAccountHolderAsync(int accountId, int customerId)
    {
        return await _dbSet.AnyAsync(x => x.AccountId == accountId &&
            x.CustomerId == customerId && x.IsActive);
    }

    // Checks whether a customer is permitted to operate a particular account.
    // Useful for validating account operation permissions.
    public async Task<bool> CanCustomerOperateAccountAsync(int accountId, int customerId)
    {
        return await _dbSet.AnyAsync(x => x.AccountId == accountId &&
            x.CustomerId == customerId && x.IsActive && x.CanOperate);
    }
}
