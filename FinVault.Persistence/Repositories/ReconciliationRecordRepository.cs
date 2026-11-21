
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class ReconciliationRecordRepository : GenericRepository<ReconciliationRecord>, IReconciliationRecordRepository
    {
        public ReconciliationRecordRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. RECORD IDENTIFICATION
        // =========================================================

        // Retrieves a reconciliation record using its primary key.
        // Useful for opening an individual settlement match result.
        public async Task<ReconciliationRecord?> GetRecordDetailsAsync(int reconciliationRecordId)
        {
            return await _dbSet
                .Include(x => x.ReconciliationBatch)
                .Include(x => x.Transaction)
                .Include(x => x.ResolvedByUser)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.ReconciliationRecordId == reconciliationRecordId);
        }

        // Retrieves records using an external settlement reference.
        // Useful for tracing a settlement entry received from a bank or payment provider.
        public async Task<IEnumerable<ReconciliationRecord>> GetByExternalReferenceAsync(string externalReference)
        {
            return await _dbSet
                .Where(x => x.ExternalReference == externalReference)
                .OrderByDescending(x => x.ExternalDate)
                .ToListAsync();
        }

        // Checks whether an external reference already exists in reconciliation records.
        // Useful for detecting possible duplicate settlement entries before processing.
        public async Task<bool> ExistsByExternalReferenceAsync(string externalReference)
        {
            return await _dbSet.AnyAsync(x => x.ExternalReference == externalReference);
        }

        // =========================================================
        // 2. BATCH AND TRANSACTION RELATIONSHIPS
        // =========================================================

        // Retrieves records belonging to a reconciliation batch.
        // Useful for reviewing all settlement entries processed in one reconciliation run.
        public async Task<IEnumerable<ReconciliationRecord>> GetByBatchIdAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId)
                .OrderBy(x => x.ExternalDate)
                .ThenBy(x => x.ExternalReference)
                .ToListAsync();
        }

        // Retrieves records belonging to a batch with their internal transactions.
        // Useful for comparing external settlement entries against internal banking transactions.
        public async Task<IEnumerable<ReconciliationRecord>> GetBatchRecordsWithTransactionsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId)
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves records linked to an internal transaction.
        // Useful for tracing which external settlement entries correspond to a banking transaction.
        public async Task<IEnumerable<ReconciliationRecord>> GetByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .Where(x => x.TransactionId == transactionId)
                .Include(x => x.ReconciliationBatch)
                .OrderByDescending(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves records that have not been linked to an internal transaction.
        // Useful for identifying settlement entries requiring transaction matching.
        public async Task<IEnumerable<ReconciliationRecord>> GetWithoutTransactionAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.TransactionId == null)
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // =========================================================
        // 3. MATCH STATUS
        // =========================================================

        // Retrieves records matching a particular match status.
        // Useful for monitoring matched and exceptional settlement entries.
        public async Task<IEnumerable<ReconciliationRecord>> GetByMatchStatusAsync(string matchStatus)
        {
            return await _dbSet
                .Where(x => x.MatchStatus == matchStatus)
                .OrderByDescending(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves successfully matched records in a batch.
        // Useful for reviewing settlement entries reconciled with internal transactions.
        public async Task<IEnumerable<ReconciliationRecord>> GetMatchedRecordsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.MatchStatus == "Matched")
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves unmatched records in a batch.
        // Useful for investigating external entries without an internal match.
        public async Task<IEnumerable<ReconciliationRecord>> GetUnmatchedRecordsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.MatchStatus == "Unmatched")
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves records with amount mismatches in a batch.
        // Useful for investigating differences between external and internal transaction amounts.
        public async Task<IEnumerable<ReconciliationRecord>> GetAmountMismatchRecordsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.MatchStatus == "AmountMismatch")
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves records missing from the bank settlement data.
        // Useful for identifying internal transactions absent from the external settlement file.
        public async Task<IEnumerable<ReconciliationRecord>> GetMissingInBankRecordsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.MatchStatus == "MissingInBank")
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves records missing from the external settlement data.
        // Useful for identifying external settlement entries absent from internal records.
        public async Task<IEnumerable<ReconciliationRecord>> GetMissingInExternalRecordsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.MatchStatus == "MissingInExternal")
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves records resolved through manual review.
        // Useful for auditing exceptions handled by reconciliation staff.
        public async Task<IEnumerable<ReconciliationRecord>> GetManuallyResolvedRecordsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.MatchStatus == "Manual")
                .OrderByDescending(x => x.ResolvedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. AMOUNT AND DATE ANALYSIS
        // =========================================================

        // Retrieves records whose difference amount is non-zero.
        // Useful for identifying financial discrepancies requiring investigation.
        public async Task<IEnumerable<ReconciliationRecord>> GetRecordsWithDifferencesAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.DifferenceAmount != 0)
                .OrderByDescending(x => Math.Abs(x.DifferenceAmount))
                .ToListAsync();
        }

        // Retrieves records with external dates within a supplied range.
        // Useful for reviewing settlement entries across business dates.
        public async Task<IEnumerable<ReconciliationRecord>> GetByExternalDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ExternalDate >= startDate.Date && x.ExternalDate <= endDate.Date)
                .OrderByDescending(x => x.ExternalDate)
                .ToListAsync();
        }

        // Retrieves records with an external amount within a supplied range.
        // Useful for investigating settlement entries by transaction value.
        public async Task<IEnumerable<ReconciliationRecord>> GetByExternalAmountRangeAsync(decimal minimumAmount, decimal maximumAmount)
        {
            return await _dbSet
                .Where(x => x.ExternalAmount >= minimumAmount && x.ExternalAmount <= maximumAmount)
                .OrderBy(x => x.ExternalAmount)
                .ToListAsync();
        }

        // =========================================================
        // 5. RESOLUTION AND AUDIT
        // =========================================================

        // Retrieves records resolved by a particular user.
        // Useful for reviewing staff responsibility for reconciliation exceptions.
        public async Task<IEnumerable<ReconciliationRecord>> GetByResolvedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.ResolvedByUserId == userId)
                .OrderByDescending(x => x.ResolvedAt)
                .ToListAsync();
        }

        // Retrieves records resolved within a supplied date range.
        // Useful for reporting reconciliation exception resolution activity.
        public async Task<IEnumerable<ReconciliationRecord>> GetResolvedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ResolvedAt.HasValue &&
                            x.ResolvedAt.Value >= startDate &&
                            x.ResolvedAt.Value <= endDate)
                .OrderByDescending(x => x.ResolvedAt)
                .ToListAsync();
        }

        // Retrieves unresolved records from a batch.
        // Useful for identifying exceptions that still require reconciliation review.
        public async Task<IEnumerable<ReconciliationRecord>> GetUnresolvedRecordsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId && x.ResolvedAt == null)
                .OrderBy(x => x.ExternalDate)
                .ToListAsync();
        }

        // =========================================================
        // 6. SEARCH AND COUNTS
        // =========================================================

        // Searches records by external reference, exception reason or resolution notes.
        // Useful for locating settlement discrepancies during investigation.
        public async Task<IEnumerable<ReconciliationRecord>> SearchRecordsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.ExternalReference.Contains(searchTerm) ||
                            (x.ExceptionReason != null && x.ExceptionReason.Contains(searchTerm)) ||
                            (x.ResolutionNotes != null && x.ResolutionNotes.Contains(searchTerm)))
                .OrderByDescending(x => x.ExternalDate)
                .ToListAsync();
        }

        // Counts records in a batch matching a particular match status.
        // Useful for calculating reconciliation result summaries.
        public async Task<int> CountByBatchAndMatchStatusAsync(int reconciliationBatchId, string matchStatus)
        {
            return await _dbSet.CountAsync(x =>
                x.ReconciliationBatchId == reconciliationBatchId &&
                x.MatchStatus == matchStatus);
        }

        // Calculates the total external amount for records in a batch.
        // Useful for comparing imported settlement totals with the source file control total.
        public async Task<decimal> GetTotalExternalAmountByBatchAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId)
                .SumAsync(x => (decimal?)x.ExternalAmount) ?? 0m;
        }

        // Calculates the total internal amount for records in a batch.
        // Useful for comparing matched internal transaction values with external settlement totals.
        public async Task<decimal> GetTotalInternalAmountByBatchAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId)
                .SumAsync(x => x.InternalAmount) ?? 0m;
        }

        // Calculates the total difference amount for records in a batch.
        // Useful for summarizing the financial discrepancy across a reconciliation run.
        public async Task<decimal> GetTotalDifferenceAmountByBatchAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatchId == reconciliationBatchId)
                .SumAsync(x => (decimal?)x.DifferenceAmount) ?? 0m;
        }
    }
}
