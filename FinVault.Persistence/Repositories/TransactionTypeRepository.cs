
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class TransactionTypeRepository : GenericRepository<TransactionType>, ITransactionTypeRepository
{
    public TransactionTypeRepository(FinVaultDbContext context) : base(context)
    {
    }

    // =====================================================
    // 1. TRANSACTION TYPE IDENTIFICATION
    // =====================================================

    // Finds a transaction type using its unique code.
    // Useful for identifying transaction rules during transaction processing.
    public async Task<TransactionType?> GetByCodeAsync(string code)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Code == code);
    }

    // Finds transaction types using their names.
    // Useful for transaction type selection and administration.
    public async Task<IEnumerable<TransactionType>> GetByNameAsync(string name)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToListAsync();
    }


    // =====================================================
    // 2. TRANSACTION TYPE CATEGORY AND STATUS
    // =====================================================

    // Retrieves transaction types belonging to a particular category.
    // Useful for grouping deposits, withdrawals, transfers, payments and other operations.
    public async Task<IEnumerable<TransactionType>> GetByCategoryAsync(string category)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Category == category).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves all active transaction types.
    // Useful when selecting transaction types for new transactions.
    public async Task<IEnumerable<TransactionType>> GetActiveTransactionTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves all inactive transaction types.
    // Useful for administration and historical transaction type review.
    public async Task<IEnumerable<TransactionType>> GetInactiveTransactionTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => !x.IsActive).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves transaction types that require approval.
    // Useful for identifying transactions that need authorization before processing.
    public async Task<IEnumerable<TransactionType>> GetApprovalRequiredTransactionTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.RequiresApproval).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves transaction types that support reversal.
    // Useful for identifying transaction types eligible for reversal operations.
    public async Task<IEnumerable<TransactionType>> GetReversibleTransactionTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.IsReversible).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves transaction types that do not support reversal.
    // Useful for preventing unsupported reversal operations.
    public async Task<IEnumerable<TransactionType>> GetNonReversibleTransactionTypesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => !x.IsReversible).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves transaction types with an approval threshold.
    // Useful for reviewing transaction types that require amount-based authorization.
    public async Task<IEnumerable<TransactionType>> GetTransactionTypesWithApprovalThresholdAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.ApprovalThreshold.HasValue).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves transaction types with a configured daily limit.
    // Useful for monitoring transaction limits and risk controls.
    public async Task<IEnumerable<TransactionType>> GetTransactionTypesWithDailyLimitAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.DailyLimit.HasValue).OrderBy(x => x.Name).ToListAsync();
    }


    // =====================================================
    // 3. TRANSACTION TYPE BUSINESS RELATIONSHIPS
    // =====================================================

    // Retrieves transaction types that have associated transactions.
    // Useful for identifying transaction types currently used by the banking system.
    public async Task<IEnumerable<TransactionType>> GetTransactionTypesWithTransactionsAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.Transactions.Any()).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves transaction types that have associated fees.
    // Useful for reviewing transaction types connected to banking charges.
    public async Task<IEnumerable<TransactionType>> GetTransactionTypesWithFeesAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.Fees.Any()).OrderBy(x => x.Name).ToListAsync();
    }

    // Retrieves a transaction type with its transactions, fees and audit users.
    // Useful for displaying complete transaction type information.
    public async Task<TransactionType?> GetTransactionTypeDetailsAsync(int transactionTypeId)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Transactions)
            .Include(x => x.Fees)
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser)
            .FirstOrDefaultAsync(x => x.TransactionTypeId == transactionTypeId);
    }


    // =====================================================
    // 4. TRANSACTION TYPE SEARCH AND VALIDATION
    // =====================================================

    // Retrieves recently created transaction types.
    // Useful for monitoring newly configured transaction operations.
    public async Task<IEnumerable<TransactionType>> GetRecentlyCreatedTransactionTypesAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        return await _dbSet.AsNoTracking().OrderByDescending(x => x.CreatedAt).Take(count).ToListAsync();
    }

    // Searches transaction types by code, name or description.
    // Supports optional category, active-status, approval and reversal filters.
    // Pagination prevents loading large transaction type lists at once.
    public async Task<IEnumerable<TransactionType>> SearchTransactionTypesAsync(string? searchTerm, string? category, bool? isActive, bool? requiresApproval, bool? isReversible, int pageNumber, int pageSize)
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

        if (requiresApproval.HasValue)
            query = query.Where(x => x.RequiresApproval == requiresApproval.Value);

        if (isReversible.HasValue)
            query = query.Where(x => x.IsReversible == isReversible.Value);

        return await query.OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Checks whether a transaction type is referenced by transactions or fees.
    // Useful before deleting or deactivating a transaction type.
    public async Task<bool> IsTransactionTypeInUseAsync(int transactionTypeId)
    {
        return await _dbSet.AnyAsync(x => x.TransactionTypeId == transactionTypeId &&
            (x.Transactions.Any() || x.Fees.Any()));
    }
}
