
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IJournalEntryRepository : IGenericRepository<JournalEntry>
    {
        // =========================================================
        // 1. JOURNAL IDENTIFICATION
        // =========================================================

        // Retrieves a journal entry using its unique journal number.
        // Useful for locating accounting entries during financial investigations.
        Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber);

        // Retrieves complete journal details with currency, transaction, lines and posting information.
        // Useful for reviewing the complete double-entry accounting record.
        Task<JournalEntry?> GetJournalEntryDetailsAsync(int journalEntryId);

        // Checks whether a journal number already exists.
        // Useful for preventing duplicate journal references.
        Task<bool> ExistsByJournalNumberAsync(string journalNumber);

        // =========================================================
        // 2. JOURNAL CLASSIFICATION AND STATUS
        // =========================================================

        // Retrieves journal entries matching a particular journal type.
        // Useful for separating transaction, interest, fee, loan, adjustment and reversal entries.
        Task<IEnumerable<JournalEntry>> GetByJournalTypeAsync(string journalType);

        // Retrieves journal entries matching a particular status.
        // Useful for monitoring draft, posted and reversed accounting records.
        Task<IEnumerable<JournalEntry>> GetByStatusAsync(string status);

        // Retrieves all draft journal entries.
        // Useful for identifying accounting entries awaiting validation and posting.
        Task<IEnumerable<JournalEntry>> GetDraftEntriesAsync();

        // Retrieves all posted journal entries.
        // Useful for reviewing finalized accounting records.
        Task<IEnumerable<JournalEntry>> GetPostedEntriesAsync();

        // Retrieves all reversed journal entries.
        // Useful for auditing accounting corrections and reversal activity.
        Task<IEnumerable<JournalEntry>> GetReversedEntriesAsync();

        // =========================================================
        // 3. TRANSACTION AND CURRENCY ASSOCIATIONS
        // =========================================================

        // Retrieves journal entries associated with a particular banking transaction.
        // Useful for tracing a transaction to its accounting entries.
        Task<IEnumerable<JournalEntry>> GetByTransactionIdAsync(int transactionId);

        // Retrieves journal entries recorded in a particular currency.
        // Useful for currency-wise accounting and reconciliation.
        Task<IEnumerable<JournalEntry>> GetByCurrencyIdAsync(int currencyId);

        // Retrieves journal entries posted by a particular user.
        // Useful for auditing accounting posting activity.
        Task<IEnumerable<JournalEntry>> GetByPostedByUserIdAsync(int userId);

        // =========================================================
        // 4. REVERSAL MANAGEMENT
        // =========================================================

        // Retrieves the original journal entry referenced by a reversal entry.
        // Useful for tracing an accounting correction to its original entry.
        Task<JournalEntry?> GetOriginalEntryAsync(int reversalEntryId);

        // Retrieves all reversal entries created against an original journal entry.
        // Useful for reviewing the complete reversal history of an accounting record.
        Task<IEnumerable<JournalEntry>> GetReversalEntriesAsync(int originalJournalEntryId);

        // Retrieves journal entries that have been reversed.
        // Useful for identifying original entries that have subsequent reversal records.
        Task<IEnumerable<JournalEntry>> GetEntriesWithReversalsAsync();

        // =========================================================
        // 5. JOURNAL DATE AND PERIOD MANAGEMENT
        // =========================================================

        // Retrieves journal entries within the supplied accounting date range.
        // Useful for daily, monthly and financial-period reporting.
        Task<IEnumerable<JournalEntry>> GetByEntryDateRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves journal entries posted within the supplied date range.
        // Useful for monitoring accounting posting activity.
        Task<IEnumerable<JournalEntry>> GetPostedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 6. ACCOUNTING VALIDATION
        // =========================================================

        // Retrieves draft journal entries whose debit and credit totals do not balance.
        // Useful for identifying accounting entries that cannot safely be posted.
        Task<IEnumerable<JournalEntry>> GetUnbalancedDraftEntriesAsync();

        // Retrieves journal entries that do not contain any journal lines.
        // Useful for identifying incomplete accounting entries before posting.
        Task<IEnumerable<JournalEntry>> GetEntriesWithoutLinesAsync();

        // Retrieves journal entries that have at least one associated interest calculation.
        // Useful for tracing interest calculations to their accounting records.
        Task<IEnumerable<JournalEntry>> GetEntriesWithInterestCalculationsAsync();

        // =========================================================
        // 7. REPORTING AND SEARCH
        // =========================================================

        // Calculates the total posted debit amount for a currency and accounting date range.
        // Useful for debit-side accounting reports and reconciliation.
        Task<decimal> GetTotalPostedDebitsAsync(int currencyId, DateTime startDate, DateTime endDate);

        // Calculates the total posted credit amount for a currency and accounting date range.
        // Useful for credit-side accounting reports and reconciliation.
        Task<decimal> GetTotalPostedCreditsAsync(int currencyId, DateTime startDate, DateTime endDate);

        // Searches journal entries by journal number or description.
        // Useful for locating accounting records during audits and investigations.
        Task<IEnumerable<JournalEntry>> SearchJournalEntriesAsync(string searchTerm);
    }
}
