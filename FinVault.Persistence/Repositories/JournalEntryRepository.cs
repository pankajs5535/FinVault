
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class JournalEntryRepository : GenericRepository<JournalEntry>, IJournalEntryRepository
    {
        public JournalEntryRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. JOURNAL IDENTIFICATION
        // =========================================================

        // Retrieves a journal entry using its unique journal number.
        // Useful for locating accounting entries during financial investigations.
        public async Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.JournalNumber == journalNumber);
        }

        // Retrieves complete journal details with currency, transaction, lines and posting information.
        // Useful for reviewing the complete double-entry accounting record.
        public async Task<JournalEntry?> GetJournalEntryDetailsAsync(int journalEntryId)
        {
            return await _dbSet
                .Include(x => x.Currency)
                .Include(x => x.Transaction)
                .Include(x => x.PostedByUser)
                .Include(x => x.ReversalOfJournalEntry)
                .Include(x => x.Lines)
                    .ThenInclude(x => x.Account)
                .Include(x => x.InterestCalculations)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.JournalEntryId == journalEntryId);
        }

        // Checks whether a journal number already exists.
        // Useful for preventing duplicate journal references.
        public async Task<bool> ExistsByJournalNumberAsync(string journalNumber)
        {
            return await _dbSet.AnyAsync(x => x.JournalNumber == journalNumber);
        }

        // =========================================================
        // 2. JOURNAL CLASSIFICATION AND STATUS
        // =========================================================

        // Retrieves journal entries matching a particular journal type.
        // Useful for separating transaction, interest, fee, loan, adjustment and reversal entries.
        public async Task<IEnumerable<JournalEntry>> GetByJournalTypeAsync(string journalType)
        {
            return await _dbSet
                .Where(x => x.JournalType == journalType)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves journal entries matching a particular status.
        // Useful for monitoring draft, posted and reversed accounting records.
        public async Task<IEnumerable<JournalEntry>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves all draft journal entries.
        // Useful for identifying accounting entries awaiting validation and posting.
        public async Task<IEnumerable<JournalEntry>> GetDraftEntriesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Draft")
                .OrderBy(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves all posted journal entries.
        // Useful for reviewing finalized accounting records.
        public async Task<IEnumerable<JournalEntry>> GetPostedEntriesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Posted")
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves all reversed journal entries.
        // Useful for auditing accounting corrections and reversal activity.
        public async Task<IEnumerable<JournalEntry>> GetReversedEntriesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Reversed")
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // =========================================================
        // 3. TRANSACTION AND CURRENCY ASSOCIATIONS
        // =========================================================

        // Retrieves journal entries associated with a particular banking transaction.
        // Useful for tracing a transaction to its accounting entries.
        public async Task<IEnumerable<JournalEntry>> GetByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .Where(x => x.TransactionId == transactionId)
                .OrderBy(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves journal entries recorded in a particular currency.
        // Useful for currency-wise accounting and reconciliation.
        public async Task<IEnumerable<JournalEntry>> GetByCurrencyIdAsync(int currencyId)
        {
            return await _dbSet
                .Where(x => x.CurrencyId == currencyId)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves journal entries posted by a particular user.
        // Useful for auditing accounting posting activity.
        public async Task<IEnumerable<JournalEntry>> GetByPostedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.PostedByUserId == userId)
                .OrderByDescending(x => x.PostedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. REVERSAL MANAGEMENT
        // =========================================================

        // Retrieves the original journal entry referenced by a reversal entry.
        // Useful for tracing an accounting correction to its original entry.
        public async Task<JournalEntry?> GetOriginalEntryAsync(int reversalEntryId)
        {
            var reversalEntry = await _dbSet
                .Where(x => x.JournalEntryId == reversalEntryId)
                .Select(x => x.ReversalOfJournalEntryId)
                .FirstOrDefaultAsync();

            if (!reversalEntry.HasValue)
                return null;

            return await _dbSet.FirstOrDefaultAsync(x => x.JournalEntryId == reversalEntry.Value);
        }

        // Retrieves all reversal entries created against an original journal entry.
        // Useful for reviewing the complete reversal history of an accounting record.
        public async Task<IEnumerable<JournalEntry>> GetReversalEntriesAsync(int originalJournalEntryId)
        {
            return await _dbSet
                .Where(x => x.ReversalOfJournalEntryId == originalJournalEntryId)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves journal entries that have been reversed.
        // Useful for identifying original entries that have subsequent reversal records.
        public async Task<IEnumerable<JournalEntry>> GetEntriesWithReversalsAsync()
        {
            return await _dbSet
                .Where(x => x.ReversalEntries.Any())
                .Include(x => x.ReversalEntries)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // =========================================================
        // 5. JOURNAL DATE AND PERIOD MANAGEMENT
        // =========================================================

        // Retrieves journal entries within the supplied accounting date range.
        // Useful for daily, monthly and financial-period reporting.
        public async Task<IEnumerable<JournalEntry>> GetByEntryDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.EntryDate >= startDate && x.EntryDate <= endDate)
                .OrderBy(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves journal entries posted within the supplied date range.
        // Useful for monitoring accounting posting activity.
        public async Task<IEnumerable<JournalEntry>> GetPostedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Posted" &&
                            x.PostedAt.HasValue &&
                            x.PostedAt.Value >= startDate &&
                            x.PostedAt.Value <= endDate)
                .OrderByDescending(x => x.PostedAt)
                .ToListAsync();
        }

        // =========================================================
        // 6. ACCOUNTING VALIDATION
        // =========================================================

        // Retrieves draft journal entries whose debit and credit totals do not balance.
        // Useful for identifying accounting entries that cannot safely be posted.
        public async Task<IEnumerable<JournalEntry>> GetUnbalancedDraftEntriesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Draft" &&
                            (x.TotalDebit != x.TotalCredit || x.TotalDebit <= 0))
                .OrderBy(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves journal entries that do not contain any journal lines.
        // Useful for identifying incomplete accounting entries before posting.
        public async Task<IEnumerable<JournalEntry>> GetEntriesWithoutLinesAsync()
        {
            return await _dbSet
                .Where(x => !x.Lines.Any())
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // Retrieves journal entries that have at least one associated interest calculation.
        // Useful for tracing interest calculations to their accounting records.
        public async Task<IEnumerable<JournalEntry>> GetEntriesWithInterestCalculationsAsync()
        {
            return await _dbSet
                .Include(x => x.InterestCalculations)
                .Where(x => x.InterestCalculations.Any())
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        // =========================================================
        // 7. REPORTING AND SEARCH
        // =========================================================

        // Calculates the total posted debit amount for a currency and accounting date range.
        // Useful for debit-side accounting reports and reconciliation.
        public async Task<decimal> GetTotalPostedDebitsAsync(int currencyId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CurrencyId == currencyId &&
                            x.Status == "Posted" &&
                            x.EntryDate >= startDate &&
                            x.EntryDate <= endDate)
                .SumAsync(x => (decimal?)x.TotalDebit) ?? 0m;
        }

        // Calculates the total posted credit amount for a currency and accounting date range.
        // Useful for credit-side accounting reports and reconciliation.
        public async Task<decimal> GetTotalPostedCreditsAsync(int currencyId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CurrencyId == currencyId &&
                            x.Status == "Posted" &&
                            x.EntryDate >= startDate &&
                            x.EntryDate <= endDate)
                .SumAsync(x => (decimal?)x.TotalCredit) ?? 0m;
        }

        // Searches journal entries by journal number or description.
        // Useful for locating accounting records during audits and investigations.
        public async Task<IEnumerable<JournalEntry>> SearchJournalEntriesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.JournalNumber.Contains(searchTerm) ||
                            x.Description.Contains(searchTerm))
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }
    }
}
