using FinVault.Application.DTOs.JournalEntryDtos;
using FinVault.Application.DTOs.JournalEntryLineDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IJournalEntryService
    {
        // 1. JOURNAL ENTRY IDENTIFICATION
        Task<IEnumerable<JournalEntryDto>> GetAllAsync();
        Task<JournalEntryDto?> GetByIdAsync(int journalEntryId);
        Task<JournalEntryDto?> GetByJournalNumberAsync(string journalNumber);
        Task<JournalEntryDto?> GetJournalEntryDetailsAsync(int journalEntryId);
        Task<bool> ExistsByJournalNumberAsync(string journalNumber);
        Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto);
        Task<JournalEntryDto?> UpdateAsync(UpdateJournalEntryDto dto);
        Task<bool> DeleteAsync(DeleteJournalEntryDto dto);

        // 2. JOURNAL CLASSIFICATION AND STATUS
        Task<IEnumerable<JournalEntryDto>> GetByJournalTypeAsync(string journalType);
        Task<IEnumerable<JournalEntryDto>> GetByStatusAsync(string status);
        Task<IEnumerable<JournalEntryDto>> GetDraftEntriesAsync();
        Task<IEnumerable<JournalEntryDto>> GetPostedEntriesAsync();
        Task<IEnumerable<JournalEntryDto>> GetReversedEntriesAsync();

        // 3. TRANSACTION AND CURRENCY ASSOCIATIONS
        Task<IEnumerable<JournalEntryDto>> GetByTransactionIdAsync(int transactionId);
        Task<IEnumerable<JournalEntryDto>> GetByCurrencyIdAsync(int currencyId);
        Task<IEnumerable<JournalEntryDto>> GetByPostedByUserIdAsync(int userId);

        // 4. REVERSAL MANAGEMENT
        Task<JournalEntryDto?> GetOriginalEntryAsync(int reversalEntryId);
        Task<IEnumerable<JournalEntryDto>> GetReversalEntriesAsync(int originalJournalEntryId);
        Task<IEnumerable<JournalEntryDto>> GetEntriesWithReversalsAsync();

        // 5. JOURNAL DATE AND PERIOD MANAGEMENT
        Task<IEnumerable<JournalEntryDto>> GetByEntryDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<JournalEntryDto>> GetPostedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 6. JOURNAL LINE MANAGEMENT
        Task<IEnumerable<JournalEntryLineDto>> GetLinesByJournalEntryIdAsync(int journalEntryId);
        Task<JournalEntryLineDto?> GetJournalEntryLineDetailsAsync(int journalEntryLineId);
        Task<JournalEntryLineDto?> GetLineByJournalEntryAndLineNumberAsync(int journalEntryId, int lineNumber);
        Task<JournalEntryLineDto> CreateLineAsync(CreateJournalEntryLineDto dto);
        Task<bool> ExistsByJournalEntryAndLineNumberAsync(int journalEntryId, int lineNumber);
        Task<IEnumerable<JournalEntryLineDto>> GetDebitLinesByJournalEntryIdAsync(int journalEntryId);
        Task<IEnumerable<JournalEntryLineDto>> GetCreditLinesByJournalEntryIdAsync(int journalEntryId);
        Task<IEnumerable<JournalEntryLineDto>> GetJournalLinesWithAccountsAsync(int journalEntryId);

        // 7. ACCOUNT ASSOCIATIONS
        Task<IEnumerable<JournalEntryLineDto>> GetLinesByAccountIdAsync(int accountId);
        Task<IEnumerable<JournalEntryLineDto>> GetDebitLinesByAccountIdAsync(int accountId);
        Task<IEnumerable<JournalEntryLineDto>> GetCreditLinesByAccountIdAsync(int accountId);
        Task<IEnumerable<JournalEntryLineDto>> GetAccountLinesByDateRangeAsync(
            int accountId,
            DateTime startDate,
            DateTime endDate);

        // 8. ACCOUNTING VALIDATION
        Task<decimal> GetTotalDebitByJournalEntryIdAsync(int journalEntryId);
        Task<decimal> GetTotalCreditByJournalEntryIdAsync(int journalEntryId);
        Task<bool> IsJournalEntryBalancedAsync(int journalEntryId);
        Task<int> CountLinesByJournalEntryIdAsync(int journalEntryId);
        Task<IEnumerable<JournalEntryDto>> GetUnbalancedDraftEntriesAsync();
        Task<IEnumerable<JournalEntryDto>> GetEntriesWithoutLinesAsync();

        // 9. INTEREST AND FINANCIAL REPORTING
        Task<IEnumerable<JournalEntryDto>> GetEntriesWithInterestCalculationsAsync();
        Task<decimal> GetTotalPostedDebitsAsync(int currencyId, DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalPostedCreditsAsync(int currencyId, DateTime startDate, DateTime endDate);

        // 10. AUDIT AND SEARCH
        Task<IEnumerable<JournalEntryLineDto>> GetLinesByCreatedByUserIdAsync(int userId);
        Task<IEnumerable<JournalEntryDto>> SearchJournalEntriesAsync(string searchTerm);
        Task<IEnumerable<JournalEntryLineDto>> SearchJournalEntryLinesAsync(string searchTerm);
    }
}


//Banking rules:
//-A journal can be posted only when it contains lines and total debits equal total credits.
//- Each journal line must contain either a debit or a credit, never both.
//- Posted journal entries and their lines are immutable.
//- Correct posted entries through reversal entries; preserve the original journal.
//- Enforce unique journal numbers and line numbers within each journal.
//- Use RowVersion when updating journal headers.