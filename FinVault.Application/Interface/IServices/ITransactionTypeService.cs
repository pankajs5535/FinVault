
using FinVault.Application.DTOs.TransactionTypeDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ITransactionTypeService
    {

        // 1. CRUD AND IDENTIFICATION


        Task<IEnumerable<TransactionTypeDto>> GetAllAsync();

        Task<TransactionTypeDto?> GetByIdAsync(int transactionTypeId);

        Task<TransactionTypeDto?> GetByCodeAsync(string code);

        Task<IEnumerable<TransactionTypeDto>> GetByNameAsync(string name);

        Task<TransactionTypeDto?> GetTransactionTypeDetailsAsync(int transactionTypeId);

        Task<TransactionTypeDto> CreateAsync(CreateTransactionTypeDto dto);

        Task<TransactionTypeDto?> UpdateAsync(UpdateTransactionTypeDto dto);

        Task<bool> DeleteAsync(int transactionTypeId);



        // 2. CATEGORY AND STATUS MANAGEMENT


        Task<IEnumerable<TransactionTypeDto>> GetByCategoryAsync(string category);

        Task<IEnumerable<TransactionTypeDto>> GetActiveTransactionTypesAsync();

        Task<IEnumerable<TransactionTypeDto>> GetInactiveTransactionTypesAsync();

        Task<bool> ActivateAsync(int transactionTypeId);

        Task<bool> DeactivateAsync(int transactionTypeId);



        // 3. APPROVAL AND REVERSAL RULES


        Task<IEnumerable<TransactionTypeDto>> GetApprovalRequiredTransactionTypesAsync();

        Task<IEnumerable<TransactionTypeDto>> GetReversibleTransactionTypesAsync();

        Task<IEnumerable<TransactionTypeDto>> GetNonReversibleTransactionTypesAsync();

        Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithApprovalThresholdAsync();

        Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithDailyLimitAsync();

        Task<bool> UpdateApprovalRequirementAsync(int transactionTypeId, bool requiresApproval, decimal? approvalThreshold);

        Task<bool> UpdateDailyLimitAsync(int transactionTypeId, decimal? dailyLimit);

        Task<bool> UpdateReversalPermissionAsync(int transactionTypeId, bool isReversible);



        // 4. BUSINESS RELATIONSHIPS


        Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithTransactionsAsync();

        Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithFeesAsync();



        // 5. SEARCH AND FILTER


        Task<IEnumerable<TransactionTypeDto>> GetRecentlyCreatedTransactionTypesAsync(int count);

        Task<IEnumerable<TransactionTypeDto>> SearchAsync(
            string? searchTerm,
            string? category,
            bool? isActive,
            bool? requiresApproval,
            bool? isReversible,
            int pageNumber,
            int pageSize);



        // 6. VALIDATION AND EXISTENCE


        Task<bool> IsCodeExistsAsync(string code);

        Task<bool> IsNameExistsAsync(string name);

        Task<bool> IsTransactionTypeInUseAsync(int transactionTypeId);

        Task<bool> CanTransactionTypeBeDeletedAsync(int transactionTypeId);
    }
}
