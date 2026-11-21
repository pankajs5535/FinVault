
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IJournalEntryLineRepository : IGenericRepository<JournalEntryLine>
    {
        // =========================================================
        // 1. JOURNAL LINE IDENTIFICATION
        // =========================================================

        // Retrieves a journal line with its journal entry, account and creator.
        // Useful for reviewing the complete accounting impact of an individual line.
        Task<JournalEntryLine?> GetJournalEntryLineDetailsAsync(int journalEntryLineId);

        // Retrieves a journal line using its journal entry and line number.
        // Useful for locating a specific debit or credit leg within a journal.
        Task<JournalEntryLine?> GetByJournalEntryAndLineNumberAsync(int journalEntryId, int lineNumber);

        // Checks whether a line number already exists within a journal entry.
        // Useful for preventing duplicate line numbers before inserting accounting records.
        Task<bool> ExistsByJournalEntryAndLineNumberAsync(int journalEntryId, int lineNumber);

        // =========================================================
        // 2. JOURNAL ENTRY ASSOCIATIONS
        // =========================================================

        // Retrieves all lines belonging to a journal entry.
        // Useful for validating the complete debit and credit structure before posting.
        Task<IEnumerable<JournalEntryLine>> GetByJournalEntryIdAsync(int journalEntryId);

        // Retrieves debit lines belonging to a journal entry.
        // Useful for reviewing the accounts receiving debit postings.
        Task<IEnumerable<JournalEntryLine>> GetDebitLinesByJournalEntryIdAsync(int journalEntryId);

        // Retrieves credit lines belonging to a journal entry.
        // Useful for reviewing the accounts receiving credit postings.
        Task<IEnumerable<JournalEntryLine>> GetCreditLinesByJournalEntryIdAsync(int journalEntryId);

        // Retrieves journal lines with their associated account details.
        // Useful for displaying account-wise accounting entries in a journal voucher.
        Task<IEnumerable<JournalEntryLine>> GetJournalLinesWithAccountsAsync(int journalEntryId);

        // =========================================================
        // 3. ACCOUNT ASSOCIATIONS
        // =========================================================

        // Retrieves all journal lines associated with an account.
        // Useful for reviewing the account's accounting movement history.
        Task<IEnumerable<JournalEntryLine>> GetByAccountIdAsync(int accountId);

        // Retrieves debit lines associated with an account.
        // Useful for analyzing debit-side account movements.
        Task<IEnumerable<JournalEntryLine>> GetDebitLinesByAccountIdAsync(int accountId);

        // Retrieves credit lines associated with an account.
        // Useful for analyzing credit-side account movements.
        Task<IEnumerable<JournalEntryLine>> GetCreditLinesByAccountIdAsync(int accountId);

        // Retrieves account journal lines within an accounting date range.
        // Useful for account statements and accounting-period investigations.
        Task<IEnumerable<JournalEntryLine>> GetAccountLinesByDateRangeAsync(int accountId, DateTime startDate, DateTime endDate);

        // =========================================================
        // 4. ACCOUNTING VALIDATION
        // =========================================================

        // Calculates the total debit amount for a journal entry.
        // Useful for checking the debit total before accounting posting.
        Task<decimal> GetTotalDebitByJournalEntryIdAsync(int journalEntryId);

        // Calculates the total credit amount for a journal entry.
        // Useful for checking the credit total before accounting posting.
        Task<decimal> GetTotalCreditByJournalEntryIdAsync(int journalEntryId);

        // Checks whether a journal entry has equal debit and credit totals.
        // Useful for validating double-entry accounting before posting.
        Task<bool> IsJournalEntryBalancedAsync(int journalEntryId);

        // Counts the number of lines belonging to a journal entry.
        // Useful for detecting incomplete or unexpectedly empty journal entries.
        Task<int> CountLinesByJournalEntryIdAsync(int journalEntryId);

        // =========================================================
        // 5. AUDIT AND SEARCH
        // =========================================================

        // Retrieves journal lines created by a particular user.
        // Useful for tracking who created individual accounting entries.
        Task<IEnumerable<JournalEntryLine>> GetByCreatedByUserIdAsync(int userId);

        // Searches journal lines by narration or journal number.
        // Useful for locating accounting entries during audits and investigations.
        Task<IEnumerable<JournalEntryLine>> SearchJournalEntryLinesAsync(string searchTerm);
    }
}
