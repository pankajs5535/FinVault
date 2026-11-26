using FinVault.Application.DTOs.TransactionDtos;
using FinVault.Application.Interfaces.Services;

namespace FinVault.Application.Interfaces.Services
{
    public interface ITransactionService
    {
        // 1. CRUD AND TRANSACTION IDENTIFICATION
        Task<IEnumerable<TransactionDto>> GetAllAsync();
        Task<TransactionDto?> GetByIdAsync(int transactionId);
        Task<TransactionDto?> GetByTransactionReferenceAsync(string transactionReference);
        Task<TransactionDto?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task<TransactionDto?> GetByExternalReferenceAsync(string externalReference);
        Task<TransactionDto?> GetTransactionDetailsAsync(int transactionId);
        Task<TransactionDto> CreateAsync(CreateTransactionDto dto);
        Task<TransactionDto?> UpdateAsync(UpdateTransactionDto dto);
        Task<bool> DeleteAsync(DeleteTransactionDto dto);
        Task<bool> ExistsByTransactionReferenceAsync(string transactionReference);
        Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey);

        // 2. ACCOUNT TRANSACTION HISTORY
        Task<IEnumerable<TransactionDto>> GetBySourceAccountIdAsync(int accountId);
        Task<IEnumerable<TransactionDto>> GetByDestinationAccountIdAsync(int accountId);
        Task<IEnumerable<TransactionDto>> GetAccountTransactionHistoryAsync(int accountId);
        Task<IEnumerable<TransactionDto>> GetByTransactionTypeIdAsync(int transactionTypeId);
        Task<IEnumerable<TransactionDto>> GetByCurrencyIdAsync(int currencyId);

        // 3. TRANSACTION STATUS AND CHANNEL
        Task<IEnumerable<TransactionDto>> GetByStatusAsync(string status);
        Task<IEnumerable<TransactionDto>> GetByChannelAsync(string channel);
        Task<IEnumerable<TransactionDto>> GetPostedTransactionsAsync();
        Task<IEnumerable<TransactionDto>> GetPendingTransactionsAsync();
        Task<IEnumerable<TransactionDto>> GetOnHoldTransactionsAsync();
        Task<IEnumerable<TransactionDto>> GetFailedTransactionsAsync();

        // 4. DATE-BASED TRANSACTION HISTORY
        Task<IEnumerable<TransactionDto>> GetTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TransactionDto>> GetPostedTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TransactionDto>> GetTransactionsByValueDateAsync(DateTime valueDate);
        Task<IEnumerable<TransactionDto>> GetRecentTransactionsByAccountIdAsync(int accountId, int count);

        // 5. APPROVAL AND REVERSAL TRACKING
        Task<IEnumerable<TransactionDto>> GetByInitiatedByUserIdAsync(int userId);
        Task<IEnumerable<TransactionDto>> GetByApprovedByUserIdAsync(int userId);
        Task<IEnumerable<TransactionDto>> GetReversalTransactionsAsync(int originalTransactionId);
        Task<IEnumerable<TransactionDto>> GetTransactionsWithOriginalTransactionAsync();

        // 6. JOURNAL AND RELATED BANKING RECORDS
        Task<TransactionDto?> GetWithJournalEntriesAsync(int transactionId);
        Task<TransactionDto?> GetWithPaymentRequestAsync(int transactionId);
        Task<IEnumerable<TransactionDto>> GetTransactionsWithCardTransactionsAsync();
        Task<IEnumerable<TransactionDto>> GetTransactionsWithLoanRepaymentsAsync();

        // 7. FINANCIAL REPORTING AND SEARCH
        Task<decimal> GetTotalPostedOutgoingAmountAsync(int accountId);
        Task<decimal> GetTotalPostedIncomingAmountAsync(int accountId);
        Task<decimal> GetTotalPostedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TransactionDto>> SearchTransactionsAsync(string searchTerm);
        Task<int> CountAccountTransactionsByStatusAsync(int accountId, string status);
    }
}


//Banking rules:
//-Transaction type codes must be unique.
//- Only active transaction types can be used for new transactions.
//- Enforce approval thresholds and daily limits during transaction processing.
//- Do not delete transaction types already referenced by transactions or fees; deactivate them instead.
//- Reversal eligibility must follow IsReversible.