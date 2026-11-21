
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        // =========================================================
        // 1. TRANSACTION IDENTIFICATION
        // =========================================================

        // Retrieves a transaction using its unique system-generated reference.
        // Useful for tracking a financial transaction across banking operations.
        Task<Transaction?> GetByTransactionReferenceAsync(string transactionReference);

        // Retrieves a transaction using its client-supplied idempotency key.
        // Useful for preventing duplicate financial transactions when requests are retried.
        Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);

        // Retrieves a transaction using its external payment network reference.
        // Useful for tracing transactions during bank and payment network reconciliation.
        Task<Transaction?> GetByExternalReferenceAsync(string externalReference);

        // Retrieves complete transaction details with accounts, currency, type, fee, users and journal entries.
        // Useful for investigating the complete financial transaction and its accounting records.
        Task<Transaction?> GetTransactionDetailsAsync(int transactionId);

        // Checks whether a transaction reference already exists.
        // Useful for preventing duplicate transaction identifiers.
        Task<bool> ExistsByTransactionReferenceAsync(string transactionReference);

        // Checks whether an idempotency key already exists.
        // Useful for preventing duplicate transaction creation during retries.
        Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey);

        // =========================================================
        // 2. ACCOUNT TRANSACTION HISTORY
        // =========================================================

        // Retrieves transactions initiated from a source account.
        // Useful for displaying outgoing account transaction history.
        Task<IEnumerable<Transaction>> GetBySourceAccountIdAsync(int accountId);

        // Retrieves transactions credited to a destination account.
        // Useful for displaying incoming account transaction history.
        Task<IEnumerable<Transaction>> GetByDestinationAccountIdAsync(int accountId);

        // Retrieves transactions where an account is either the source or destination.
        // Useful for generating a complete account statement.
        Task<IEnumerable<Transaction>> GetAccountTransactionHistoryAsync(int accountId);

        // Retrieves transactions belonging to a particular transaction type.
        // Useful for separating deposits, withdrawals, transfers and other banking operations.
        Task<IEnumerable<Transaction>> GetByTransactionTypeIdAsync(int transactionTypeId);

        // Retrieves transactions recorded in a particular currency.
        // Useful for reviewing transaction activity by currency.
        Task<IEnumerable<Transaction>> GetByCurrencyIdAsync(int currencyId);

        // =========================================================
        // 3. TRANSACTION STATUS AND CHANNEL
        // =========================================================

        // Retrieves transactions matching a particular lifecycle status.
        // Useful for monitoring initiated, pending, processing, posted and failed transactions.
        Task<IEnumerable<Transaction>> GetByStatusAsync(string status);

        // Retrieves transactions matching a particular banking channel.
        // Useful for monitoring branch, ATM, mobile, UPI, POS and API activity.
        Task<IEnumerable<Transaction>> GetByChannelAsync(string channel);

        // Retrieves posted transactions.
        // Useful for reviewing transactions that have reached the posted state.
        Task<IEnumerable<Transaction>> GetPostedTransactionsAsync();

        // Retrieves transactions awaiting processing.
        // Useful for monitoring transactions that have not yet reached a final processing outcome.
        Task<IEnumerable<Transaction>> GetPendingTransactionsAsync();

        // Retrieves transactions placed on hold.
        // Useful for reviewing transactions awaiting operational or compliance clearance.
        Task<IEnumerable<Transaction>> GetOnHoldTransactionsAsync();

        // Retrieves failed transactions.
        // Useful for investigating unsuccessful banking operations.
        Task<IEnumerable<Transaction>> GetFailedTransactionsAsync();

        // =========================================================
        // 4. DATE-BASED TRANSACTION HISTORY
        // =========================================================

        // Retrieves transactions recorded within the supplied transaction date range.
        // Useful for daily and monthly transaction reporting.
        Task<IEnumerable<Transaction>> GetTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves posted transactions within the supplied posting date range.
        // Useful for reviewing transactions by their actual posting time.
        Task<IEnumerable<Transaction>> GetPostedTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves transactions belonging to a particular value date.
        // Useful for reviewing transactions according to their banking value date.
        Task<IEnumerable<Transaction>> GetTransactionsByValueDateAsync(DateTime valueDate);

        // Retrieves the latest transactions for an account.
        // Useful for displaying recent account activity.
        Task<IEnumerable<Transaction>> GetRecentTransactionsByAccountIdAsync(int accountId, int count);

        // =========================================================
        // 5. APPROVAL AND REVERSAL TRACKING
        // =========================================================

        // Retrieves transactions initiated by a particular user.
        // Useful for auditing transaction initiation activity.
        Task<IEnumerable<Transaction>> GetByInitiatedByUserIdAsync(int userId);

        // Retrieves transactions approved by a particular user.
        // Useful for reviewing transaction approval activity and maker-checker controls.
        Task<IEnumerable<Transaction>> GetByApprovedByUserIdAsync(int userId);

        // Retrieves reversal transactions linked to an original transaction.
        // Useful for tracing the reversal history of a financial transaction.
        Task<IEnumerable<Transaction>> GetReversalTransactionsAsync(int originalTransactionId);

        // Retrieves transactions that reverse another transaction.
        // Useful for reviewing the original transaction associated with each reversal.
        Task<IEnumerable<Transaction>> GetTransactionsWithOriginalTransactionAsync();

        // =========================================================
        // 6. JOURNAL AND RELATED BANKING RECORDS
        // =========================================================

        // Retrieves a transaction with its journal entries and journal lines.
        // Useful for investigating the double-entry accounting impact of a transaction.
        Task<Transaction?> GetWithJournalEntriesAsync(int transactionId);

        // Retrieves a transaction with its linked payment request.
        // Useful for tracing payment instructions to their resulting transactions.
        Task<Transaction?> GetWithPaymentRequestAsync(int transactionId);

        // Retrieves transactions linked to card transaction records.
        // Useful for tracing card activity to the underlying banking transaction.
        Task<IEnumerable<Transaction>> GetTransactionsWithCardTransactionsAsync();

        // Retrieves transactions linked to loan repayments.
        // Useful for reviewing loan collections recorded through banking transactions.
        Task<IEnumerable<Transaction>> GetTransactionsWithLoanRepaymentsAsync();

        // =========================================================
        // 7. FINANCIAL REPORTING AND SEARCH
        // =========================================================

        // Calculates the total amount of posted outgoing transactions for an account.
        // Useful for reporting confirmed outgoing transaction volume.
        Task<decimal> GetTotalPostedOutgoingAmountAsync(int accountId);

        // Calculates the total amount of posted incoming transactions for an account.
        // Useful for reporting confirmed incoming transaction volume.
        Task<decimal> GetTotalPostedIncomingAmountAsync(int accountId);

        // Calculates the total amount of posted transactions within a date range.
        // Useful for reporting posted transaction volume for a reporting period.
        Task<decimal> GetTotalPostedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Searches transactions by reference, external reference, description or remarks.
        // Useful for locating transaction records during customer support and investigation.
        Task<IEnumerable<Transaction>> SearchTransactionsAsync(string searchTerm);

        // Counts transactions for an account matching the supplied status.
        // Useful for monitoring account transaction activity by lifecycle status.
        Task<int> CountAccountTransactionsByStatusAsync(int accountId, string status);
    }
}
