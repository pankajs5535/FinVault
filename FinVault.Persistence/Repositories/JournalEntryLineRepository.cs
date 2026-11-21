
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class JournalEntryLineRepository : GenericRepository<JournalEntryLine>, IJournalEntryLineRepository
    {
        public JournalEntryLineRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. JOURNAL LINE IDENTIFICATION
        // =========================================================

        // Retrieves a journal line with its journal entry, account and creator.
        // Useful for reviewing the complete accounting impact of an individual line.
        public async Task<JournalEntryLine?> GetJournalEntryLineDetailsAsync(int journalEntryLineId)
        {
            return await _dbSet
                .Include(x => x.JournalEntry)
                .Include(x => x.Account)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.JournalEntryLineId == journalEntryLineId);
        }

        // Retrieves a journal line using its journal entry and line number.
        // Useful for locating a specific debit or credit leg within a journal.
        public async Task<JournalEntryLine?> GetByJournalEntryAndLineNumberAsync(int journalEntryId, int lineNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x =>
                x.JournalEntryId == journalEntryId && x.LineNumber == lineNumber);
        }

        // Checks whether a line number already exists within a journal entry.
        // Useful for preventing duplicate line numbers before inserting accounting records.
        public async Task<bool> ExistsByJournalEntryAndLineNumberAsync(int journalEntryId, int lineNumber)
        {
            return await _dbSet.AnyAsync(x =>
                x.JournalEntryId == journalEntryId && x.LineNumber == lineNumber);
        }

        // =========================================================
        // 2. JOURNAL ENTRY ASSOCIATIONS
        // =========================================================

        // Retrieves all lines belonging to a journal entry.
        // Useful for validating the complete debit and credit structure before posting.
        public async Task<IEnumerable<JournalEntryLine>> GetByJournalEntryIdAsync(int journalEntryId)
        {
            return await _dbSet
                .Where(x => x.JournalEntryId == journalEntryId)
                .OrderBy(x => x.LineNumber)
                .ToListAsync();
        }

        // Retrieves debit lines belonging to a journal entry.
        // Useful for reviewing the accounts receiving debit postings.
        public async Task<IEnumerable<JournalEntryLine>> GetDebitLinesByJournalEntryIdAsync(int journalEntryId)
        {
            return await _dbSet
                .Where(x => x.JournalEntryId == journalEntryId && x.DebitAmount > 0)
                .OrderBy(x => x.LineNumber)
                .ToListAsync();
        }

        // Retrieves credit lines belonging to a journal entry.
        // Useful for reviewing the accounts receiving credit postings.
        public async Task<IEnumerable<JournalEntryLine>> GetCreditLinesByJournalEntryIdAsync(int journalEntryId)
        {
            return await _dbSet
                .Where(x => x.JournalEntryId == journalEntryId && x.CreditAmount > 0)
                .OrderBy(x => x.LineNumber)
                .ToListAsync();
        }

        // Retrieves journal lines with their associated account details.
        // Useful for displaying account-wise accounting entries in a journal voucher.
        public async Task<IEnumerable<JournalEntryLine>> GetJournalLinesWithAccountsAsync(int journalEntryId)
        {
            return await _dbSet
                .Include(x => x.Account)
                .Where(x => x.JournalEntryId == journalEntryId)
                .OrderBy(x => x.LineNumber)
                .ToListAsync();
        }

        // =========================================================
        // 3. ACCOUNT ASSOCIATIONS
        // =========================================================

        // Retrieves all journal lines associated with an account.
        // Useful for reviewing the account's accounting movement history.
        public async Task<IEnumerable<JournalEntryLine>> GetByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Include(x => x.JournalEntry)
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.JournalEntry.EntryDate)
                .ThenByDescending(x => x.LineNumber)
                .ToListAsync();
        }

        // Retrieves debit lines associated with an account.
        // Useful for analyzing debit-side account movements.
        public async Task<IEnumerable<JournalEntryLine>> GetDebitLinesByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Include(x => x.JournalEntry)
                .Where(x => x.AccountId == accountId && x.DebitAmount > 0)
                .OrderByDescending(x => x.JournalEntry.EntryDate)
                .ToListAsync();
        }

        // Retrieves credit lines associated with an account.
        // Useful for analyzing credit-side account movements.
        public async Task<IEnumerable<JournalEntryLine>> GetCreditLinesByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Include(x => x.JournalEntry)
                .Where(x => x.AccountId == accountId && x.CreditAmount > 0)
                .OrderByDescending(x => x.JournalEntry.EntryDate)
                .ToListAsync();
        }

        // Retrieves account journal lines within an accounting date range.
        // Useful for account statements and accounting-period investigations.
        public async Task<IEnumerable<JournalEntryLine>> GetAccountLinesByDateRangeAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(x => x.JournalEntry)
                .Where(x => x.AccountId == accountId &&
                            x.JournalEntry.EntryDate >= startDate &&
                            x.JournalEntry.EntryDate <= endDate)
                .OrderBy(x => x.JournalEntry.EntryDate)
                .ThenBy(x => x.LineNumber)
                .ToListAsync();
        }

        // =========================================================
        // 4. ACCOUNTING VALIDATION
        // =========================================================

        // Calculates the total debit amount for a journal entry.
        // Useful for checking the debit total before accounting posting.
        public async Task<decimal> GetTotalDebitByJournalEntryIdAsync(int journalEntryId)
        {
            return await _dbSet
                .Where(x => x.JournalEntryId == journalEntryId)
                .SumAsync(x => (decimal?)x.DebitAmount) ?? 0m;
        }

        // Calculates the total credit amount for a journal entry.
        // Useful for checking the credit total before accounting posting.
        public async Task<decimal> GetTotalCreditByJournalEntryIdAsync(int journalEntryId)
        {
            return await _dbSet
                .Where(x => x.JournalEntryId == journalEntryId)
                .SumAsync(x => (decimal?)x.CreditAmount) ?? 0m;
        }

        // Checks whether a journal entry has equal debit and credit totals.
        // Useful for validating double-entry accounting before posting.
        public async Task<bool> IsJournalEntryBalancedAsync(int journalEntryId)
        {
            var totals = await _dbSet
                .Where(x => x.JournalEntryId == journalEntryId)
                .GroupBy(x => x.JournalEntryId)
                .Select(x => new
                {
                    TotalDebit = x.Sum(line => line.DebitAmount),
                    TotalCredit = x.Sum(line => line.CreditAmount),
                    LineCount = x.Count()
                })
                .FirstOrDefaultAsync();

            return totals != null &&
                   totals.LineCount >= 2 &&
                   totals.TotalDebit > 0 &&
                   totals.TotalDebit == totals.TotalCredit;
        }

        // Counts the number of lines belonging to a journal entry.
        // Useful for detecting incomplete or unexpectedly empty journal entries.
        public async Task<int> CountLinesByJournalEntryIdAsync(int journalEntryId)
        {
            return await _dbSet.CountAsync(x => x.JournalEntryId == journalEntryId);
        }

        // =========================================================
        // 5. AUDIT AND SEARCH
        // =========================================================

        // Retrieves journal lines created by a particular user.
        // Useful for tracking who created individual accounting entries.
        public async Task<IEnumerable<JournalEntryLine>> GetByCreatedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.JournalEntry)
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Searches journal lines by narration or journal number.
        // Useful for locating accounting entries during audits and investigations.
        public async Task<IEnumerable<JournalEntryLine>> SearchJournalEntryLinesAsync(string searchTerm)
        {
            return await _dbSet
                .Include(x => x.JournalEntry)
                .Include(x => x.Account)
                .Where(x => (x.Narration != null && x.Narration.Contains(searchTerm)) ||
                            x.JournalEntry.JournalNumber.Contains(searchTerm))
                .OrderByDescending(x => x.JournalEntry.EntryDate)
                .ThenBy(x => x.LineNumber)
                .ToListAsync();
        }
    }
}
