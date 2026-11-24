
using FinVault.Application.DTOs.TransactionDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ITransactionService
    {

        // 1. TRANSACTION IDENTIFICATION


        Task<IEnumerable<TransactionDto>> GetAllAsync();

        Task<TransactionDto?> GetByIdAsync(int transactionId);

        Task<TransactionDto?> GetByTransactionReferenceAsync(string transactionReference);

        Task<TransactionDto?> GetByIdempotencyKeyAsync(string idempotencyKey);

        Task<TransactionDto?> GetByExternalReferenceAsync(string externalReference);

        Task<TransactionDto?> GetTransactionDetailsAsync(int transactionId);

        Task<TransactionDto> CreateAsync(CreateTransactionDto dto);

        Task<TransactionDto?> UpdateAsync(UpdateTransactionDto dto);



        // 2. ACCOUNT TRANSACTION HISTORY


        Task<IEnumerable<TransactionDto>> GetBySourceAccountIdAsync(int accountId);

        Task<IEnumerable<TransactionDto>> GetByDestinationAccountIdAsync(int accountId);

        Task<IEnumerable<TransactionDto>> GetAccountTransactionHistoryAsync(int accountId);

        Task<IEnumerable<TransactionDto>> GetByTransactionTypeIdAsync(int transactionTypeId);

        Task<IEnumerable<TransactionDto>> GetByCurrencyIdAsync(int currencyId);

        Task<IEnumerable<TransactionDto>> GetRecentTransactionsByAccountIdAsync(int accountId, int count);



        // 3. TRANSACTION STATUS AND CHANNEL


        Task<IEnumerable<TransactionDto>> GetByStatusAsync(string status);

        Task<IEnumerable<TransactionDto>> GetByChannelAsync(string channel);

        Task<IEnumerable<TransactionDto>> GetPostedTransactionsAsync();

        Task<IEnumerable<TransactionDto>> GetPendingTransactionsAsync();

        Task<IEnumerable<TransactionDto>> GetOnHoldTransactionsAsync();

        Task<IEnumerable<TransactionDto>> GetFailedTransactionsAsync();



        // 4. DATE-BASED TRANSACTION OPERATIONS


        Task<IEnumerable<TransactionDto>> GetTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<TransactionDto>> GetPostedTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<TransactionDto>> GetTransactionsByValueDateAsync(DateTime valueDate);



        // 5. APPROVAL AND REVERSAL MANAGEMENT


        Task<IEnumerable<TransactionDto>> GetByInitiatedByUserIdAsync(int userId);

        Task<IEnumerable<TransactionDto>> GetByApprovedByUserIdAsync(int userId);

        Task<IEnumerable<TransactionDto>> GetReversalTransactionsAsync(int originalTransactionId);

        Task<IEnumerable<TransactionDto>> GetTransactionsWithOriginalTransactionAsync(int originalTransactionId);

        Task<bool> ApproveTransactionAsync(int transactionId, int approvedByUserId);

        Task<bool> RejectTransactionAsync(int transactionId, int approvedByUserId, string reason);

        Task<bool> PlaceOnHoldAsync(int transactionId, string reason);

        Task<bool> ReleaseHoldAsync(int transactionId);

        Task<bool> ReverseTransactionAsync(int transactionId, string reason);



        // 6. JOURNAL AND RELATED BANKING RECORDS


        Task<TransactionDto?> GetWithJournalEntriesAsync(int transactionId);

        Task<TransactionDto?> GetWithPaymentRequestAsync(int transactionId);

        Task<IEnumerable<TransactionDto>> GetTransactionsWithCardTransactionsAsync();

        Task<IEnumerable<TransactionDto>> GetTransactionsWithLoanRepaymentsAsync();



        // 7. FINANCIAL REPORTING


        Task<decimal> GetTotalPostedOutgoingAmountAsync(int accountId);

        Task<decimal> GetTotalPostedIncomingAmountAsync(int accountId);

        Task<decimal> GetTotalPostedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);

        Task<int> CountAccountTransactionsByStatusAsync(int accountId, string status);



        // 8. SEARCH OPERATIONS


        Task<IEnumerable<TransactionDto>> SearchTransactionsAsync(string searchTerm);



        // 9. VALIDATION AND ELIGIBILITY


        Task<bool> IsTransactionReferenceExistsAsync(string transactionReference);

        Task<bool> IsIdempotencyKeyExistsAsync(string idempotencyKey);

        Task<bool> IsTransactionPostedAsync(int transactionId);

        Task<bool> CanTransactionBeApprovedAsync(int transactionId);

        Task<bool> CanTransactionBeReversedAsync(int transactionId);

        Task<bool> IsAccountEligibleForTransactionAsync(int accountId, decimal amount);
    }
}
