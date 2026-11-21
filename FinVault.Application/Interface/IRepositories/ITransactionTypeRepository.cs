
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories;

public interface ITransactionTypeRepository : IGenericRepository<TransactionType>
{
    // =====================================================
    // 1. TRANSACTION TYPE IDENTIFICATION
    // =====================================================

    // Finds a transaction type using its unique code.
    // Useful for identifying transaction rules during transaction processing.
    Task<TransactionType?> GetByCodeAsync(string code);

    // Finds transaction types using their names.
    // Useful for transaction type selection and administration.
    Task<IEnumerable<TransactionType>> GetByNameAsync(string name);


    // =====================================================
    // 2. TRANSACTION TYPE CATEGORY AND STATUS
    // =====================================================

    // Retrieves transaction types belonging to a particular category.
    // Useful for grouping deposits, withdrawals, transfers, payments and other operations.
    Task<IEnumerable<TransactionType>> GetByCategoryAsync(string category);

    // Retrieves all active transaction types.
    // Useful when selecting transaction types for new transactions.
    Task<IEnumerable<TransactionType>> GetActiveTransactionTypesAsync();

    // Retrieves all inactive transaction types.
    // Useful for administration and historical transaction type review.
    Task<IEnumerable<TransactionType>> GetInactiveTransactionTypesAsync();

    // Retrieves transaction types that require approval.
    // Useful for identifying transactions that need authorization before processing.
    Task<IEnumerable<TransactionType>> GetApprovalRequiredTransactionTypesAsync();

    // Retrieves transaction types that support reversal.
    // Useful for identifying transaction types eligible for reversal operations.
    Task<IEnumerable<TransactionType>> GetReversibleTransactionTypesAsync();

    // Retrieves transaction types that do not support reversal.
    // Useful for preventing unsupported reversal operations.
    Task<IEnumerable<TransactionType>> GetNonReversibleTransactionTypesAsync();

    // Retrieves transaction types with an approval threshold.
    // Useful for reviewing transaction types that require amount-based authorization.
    Task<IEnumerable<TransactionType>> GetTransactionTypesWithApprovalThresholdAsync();

    // Retrieves transaction types with a configured daily limit.
    // Useful for monitoring transaction limits and risk controls.
    Task<IEnumerable<TransactionType>> GetTransactionTypesWithDailyLimitAsync();


    // =====================================================
    // 3. TRANSACTION TYPE BUSINESS RELATIONSHIPS
    // =====================================================

    // Retrieves transaction types that have associated transactions.
    // Useful for identifying transaction types currently used by the banking system.
    Task<IEnumerable<TransactionType>> GetTransactionTypesWithTransactionsAsync();

    // Retrieves transaction types that have associated fees.
    // Useful for reviewing transaction types connected to banking charges.
    Task<IEnumerable<TransactionType>> GetTransactionTypesWithFeesAsync();

    // Retrieves a transaction type with its transactions, fees and audit users.
    // Useful for displaying complete transaction type information.
    Task<TransactionType?> GetTransactionTypeDetailsAsync(int transactionTypeId);


    // =====================================================
    // 4. TRANSACTION TYPE SEARCH AND VALIDATION
    // =====================================================

    // Retrieves recently created transaction types.
    // Useful for monitoring newly configured transaction operations.
    Task<IEnumerable<TransactionType>> GetRecentlyCreatedTransactionTypesAsync(int count);

    // Searches transaction types by code, name or description.
    // Supports optional category, active-status, approval and reversal filters.
    // Pagination prevents loading large transaction type lists at once.
    Task<IEnumerable<TransactionType>> SearchTransactionTypesAsync(string? searchTerm, string? category, bool? isActive, bool? requiresApproval, bool? isReversible, int pageNumber, int pageSize);

    // Checks whether a transaction type is referenced by transactions or fees.
    // Useful before deleting or deactivating a transaction type.
    Task<bool> IsTransactionTypeInUseAsync(int transactionTypeId);
}
