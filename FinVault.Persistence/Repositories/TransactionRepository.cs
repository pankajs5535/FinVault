
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. TRANSACTION IDENTIFICATION
        // =========================================================

        // Retrieves a transaction using its unique system-generated reference.
        // Useful for tracking a financial transaction across banking operations.
        public async Task<Transaction?> GetByTransactionReferenceAsync(string transactionReference)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TransactionReference == transactionReference);
        }

        // Retrieves a transaction using its client-supplied idempotency key.
        // Useful for preventing duplicate financial transactions when requests are retried.
        public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey);
        }

        // Retrieves a transaction using its external payment network reference.
        // Useful for tracing transactions during bank and payment network reconciliation.
        public async Task<Transaction?> GetByExternalReferenceAsync(string externalReference)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.ExternalReference == externalReference);
        }

        // Retrieves complete transaction details with accounts, currency, type, fee, users and journal entries.
        // Useful for investigating the complete financial transaction and its accounting records.
        public async Task<Transaction?> GetTransactionDetailsAsync(int transactionId)
        {
            return await _dbSet
                .Include(x => x.TransactionType)
                .Include(x => x.SourceAccount)
                .Include(x => x.DestinationAccount)
                .Include(x => x.Currency)
                .Include(x => x.Fee)
                .Include(x => x.InitiatedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.ReversalOfTransaction)
                .Include(x => x.JournalEntries)
                    .ThenInclude(x => x.Lines)
                .Include(x => x.PaymentRequest)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        // Checks whether a transaction reference already exists.
        // Useful for preventing duplicate transaction identifiers.
        public async Task<bool> ExistsByTransactionReferenceAsync(string transactionReference)
        {
            return await _dbSet.AnyAsync(x => x.TransactionReference == transactionReference);
        }

        // Checks whether an idempotency key already exists.
        // Useful for preventing duplicate transaction creation during retries.
        public async Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _dbSet.AnyAsync(x => x.IdempotencyKey == idempotencyKey);
        }

        // =========================================================
        // 2. ACCOUNT TRANSACTION HISTORY
        // =========================================================

        // Retrieves transactions initiated from a source account.
        // Useful for displaying outgoing account transaction history.
        public async Task<IEnumerable<Transaction>> GetBySourceAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.SourceAccountId == accountId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions credited to a destination account.
        // Useful for displaying incoming account transaction history.
        public async Task<IEnumerable<Transaction>> GetByDestinationAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.DestinationAccountId == accountId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions where an account is either the source or destination.
        // Useful for generating a complete account statement.
        public async Task<IEnumerable<Transaction>> GetAccountTransactionHistoryAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.SourceAccountId == accountId || x.DestinationAccountId == accountId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions belonging to a particular transaction type.
        // Useful for separating deposits, withdrawals, transfers and other banking operations.
        public async Task<IEnumerable<Transaction>> GetByTransactionTypeIdAsync(int transactionTypeId)
        {
            return await _dbSet
                .Where(x => x.TransactionTypeId == transactionTypeId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions recorded in a particular currency.
        // Useful for reviewing transaction activity by currency.
        public async Task<IEnumerable<Transaction>> GetByCurrencyIdAsync(int currencyId)
        {
            return await _dbSet
                .Where(x => x.CurrencyId == currencyId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. TRANSACTION STATUS AND CHANNEL
        // =========================================================

        // Retrieves transactions matching a particular lifecycle status.
        // Useful for monitoring initiated, pending, processing, posted and failed transactions.
        public async Task<IEnumerable<Transaction>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions matching a particular banking channel.
        // Useful for monitoring branch, ATM, mobile, UPI, POS and API activity.
        public async Task<IEnumerable<Transaction>> GetByChannelAsync(string channel)
        {
            return await _dbSet
                .Where(x => x.Channel == channel)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves posted transactions.
        // Useful for reviewing transactions that have reached the posted state.
        public async Task<IEnumerable<Transaction>> GetPostedTransactionsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Posted")
                .OrderByDescending(x => x.PostedAt)
                .ToListAsync();
        }

        // Retrieves transactions awaiting processing.
        // Useful for monitoring transactions that have not yet reached a final processing outcome.
        public async Task<IEnumerable<Transaction>> GetPendingTransactionsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Pending")
                .OrderBy(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions placed on hold.
        // Useful for reviewing transactions awaiting operational or compliance clearance.
        public async Task<IEnumerable<Transaction>> GetOnHoldTransactionsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "OnHold")
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves failed transactions.
        // Useful for investigating unsuccessful banking operations.
        public async Task<IEnumerable<Transaction>> GetFailedTransactionsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Failed")
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. DATE-BASED TRANSACTION HISTORY
        // =========================================================

        // Retrieves transactions recorded within the supplied transaction date range.
        // Useful for daily and monthly transaction reporting.
        public async Task<IEnumerable<Transaction>> GetTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.TransactionAt >= startDate && x.TransactionAt <= endDate)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves posted transactions within the supplied posting date range.
        // Useful for reviewing transactions by their actual posting time.
        public async Task<IEnumerable<Transaction>> GetPostedTransactionsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Posted" &&
                            x.PostedAt.HasValue &&
                            x.PostedAt.Value >= startDate &&
                            x.PostedAt.Value <= endDate)
                .OrderByDescending(x => x.PostedAt)
                .ToListAsync();
        }

        // Retrieves transactions belonging to a particular value date.
        // Useful for reviewing transactions according to their banking value date.
        public async Task<IEnumerable<Transaction>> GetTransactionsByValueDateAsync(DateTime valueDate)
        {
            return await _dbSet
                .Where(x => x.ValueDate == valueDate.Date)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves the latest transactions for an account.
        // Useful for displaying recent account activity.
        public async Task<IEnumerable<Transaction>> GetRecentTransactionsByAccountIdAsync(int accountId, int count)
        {
            return await _dbSet
                .Where(x => x.SourceAccountId == accountId || x.DestinationAccountId == accountId)
                .OrderByDescending(x => x.TransactionAt)
                .Take(Math.Max(0, count))
                .ToListAsync();
        }

        // =========================================================
        // 5. APPROVAL AND REVERSAL TRACKING
        // =========================================================

        // Retrieves transactions initiated by a particular user.
        // Useful for auditing transaction initiation activity.
        public async Task<IEnumerable<Transaction>> GetByInitiatedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.InitiatedByUserId == userId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions approved by a particular user.
        // Useful for reviewing transaction approval activity and maker-checker controls.
        public async Task<IEnumerable<Transaction>> GetByApprovedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.ApprovedByUserId == userId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves reversal transactions linked to an original transaction.
        // Useful for tracing the reversal history of a financial transaction.
        public async Task<IEnumerable<Transaction>> GetReversalTransactionsAsync(int originalTransactionId)
        {
            return await _dbSet
                .Where(x => x.ReversalOfTransactionId == originalTransactionId)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions that reverse another transaction.
        // Useful for reviewing the original transaction associated with each reversal.
        public async Task<IEnumerable<Transaction>> GetTransactionsWithOriginalTransactionAsync()
        {
            return await _dbSet
                .Include(x => x.ReversalOfTransaction)
                .Where(x => x.ReversalOfTransactionId != null)
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // =========================================================
        // 6. JOURNAL AND RELATED BANKING RECORDS
        // =========================================================

        // Retrieves a transaction with its journal entries and journal lines.
        // Useful for investigating the double-entry accounting impact of a transaction.
        public async Task<Transaction?> GetWithJournalEntriesAsync(int transactionId)
        {
            return await _dbSet
                .Include(x => x.JournalEntries)
                    .ThenInclude(x => x.Lines)
                .FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        // Retrieves a transaction with its linked payment request.
        // Useful for tracing payment instructions to their resulting transactions.
        public async Task<Transaction?> GetWithPaymentRequestAsync(int transactionId)
        {
            return await _dbSet
                .Include(x => x.PaymentRequest)
                .FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        // Retrieves transactions linked to card transaction records.
        // Useful for tracing card activity to the underlying banking transaction.
        public async Task<IEnumerable<Transaction>> GetTransactionsWithCardTransactionsAsync()
        {
            return await _dbSet
                .Include(x => x.CardTransactions)
                .Where(x => x.CardTransactions.Any())
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Retrieves transactions linked to loan repayments.
        // Useful for reviewing loan collections recorded through banking transactions.
        public async Task<IEnumerable<Transaction>> GetTransactionsWithLoanRepaymentsAsync()
        {
            return await _dbSet
                .Include(x => x.LoanRepayments)
                .Where(x => x.LoanRepayments.Any())
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // =========================================================
        // 7. FINANCIAL REPORTING AND SEARCH
        // =========================================================

        // Calculates the total amount of posted outgoing transactions for an account.
        // Useful for reporting confirmed outgoing transaction volume.
        public async Task<decimal> GetTotalPostedOutgoingAmountAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.SourceAccountId == accountId && x.Status == "Posted")
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Calculates the total amount of posted incoming transactions for an account.
        // Useful for reporting confirmed incoming transaction volume.
        public async Task<decimal> GetTotalPostedIncomingAmountAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.DestinationAccountId == accountId && x.Status == "Posted")
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Calculates the total amount of posted transactions within a date range.
        // Useful for reporting posted transaction volume for a reporting period.
        public async Task<decimal> GetTotalPostedAmountWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Posted" &&
                            x.PostedAt.HasValue &&
                            x.PostedAt.Value >= startDate &&
                            x.PostedAt.Value <= endDate)
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Searches transactions by reference, external reference, description or remarks.
        // Useful for locating transaction records during customer support and investigation.
        public async Task<IEnumerable<Transaction>> SearchTransactionsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.TransactionReference.Contains(searchTerm) ||
                            (x.ExternalReference != null && x.ExternalReference.Contains(searchTerm)) ||
                            (x.Description != null && x.Description.Contains(searchTerm)) ||
                            (x.Remarks != null && x.Remarks.Contains(searchTerm)))
                .OrderByDescending(x => x.TransactionAt)
                .ToListAsync();
        }

        // Counts transactions for an account matching the supplied status.
        // Useful for monitoring account transaction activity by lifecycle status.
        public async Task<int> CountAccountTransactionsByStatusAsync(int accountId, string status)
        {
            return await _dbSet.CountAsync(x =>
                (x.SourceAccountId == accountId || x.DestinationAccountId == accountId) &&
                x.Status == status);
        }
    }
}
