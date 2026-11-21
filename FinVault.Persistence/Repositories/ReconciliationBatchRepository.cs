
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class ReconciliationBatchRepository : GenericRepository<ReconciliationBatch>, IReconciliationBatchRepository
    {
        public ReconciliationBatchRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. BATCH IDENTIFICATION
        // =========================================================

        // Retrieves a reconciliation batch using its unique batch number.
        // Useful for locating a specific reconciliation run during investigation.
        public async Task<ReconciliationBatch?> GetByBatchNumberAsync(string batchNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.BatchNumber == batchNumber);
        }

        // Retrieves a reconciliation batch using its settlement file hash.
        // Useful for detecting whether the same settlement file has already been imported.
        public async Task<ReconciliationBatch?> GetByFileHashAsync(string fileHash)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.FileHash == fileHash);
        }

        // Checks whether a batch number already exists.
        // Useful for preventing duplicate reconciliation batch identifiers.
        public async Task<bool> ExistsByBatchNumberAsync(string batchNumber)
        {
            return await _dbSet.AnyAsync(x => x.BatchNumber == batchNumber);
        }

        // Checks whether a settlement file hash has already been processed.
        // Useful for preventing duplicate file imports and repeated reconciliation.
        public async Task<bool> ExistsByFileHashAsync(string fileHash)
        {
            return await _dbSet.AnyAsync(x => x.FileHash == fileHash);
        }

        // Retrieves complete batch details with payment method, users and reconciliation records.
        // Useful for reviewing the complete result of a reconciliation run.
        public async Task<ReconciliationBatch?> GetBatchDetailsAsync(int reconciliationBatchId)
        {
            return await _dbSet
                .Include(x => x.PaymentMethod)
                .Include(x => x.InitiatedByUser)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.ReconciliationRecords)
                .FirstOrDefaultAsync(x => x.ReconciliationBatchId == reconciliationBatchId);
        }

        // =========================================================
        // 2. RECONCILIATION TYPE AND BUSINESS DATE
        // =========================================================

        // Retrieves batches for a particular reconciliation type.
        // Useful for reviewing UPI, NEFT, RTGS, IMPS, card, ATM and ledger reconciliation runs.
        public async Task<IEnumerable<ReconciliationBatch>> GetByReconciliationTypeAsync(string reconciliationType)
        {
            return await _dbSet
                .Where(x => x.ReconciliationType == reconciliationType)
                .OrderByDescending(x => x.BusinessDate)
                .ToListAsync();
        }

        // Retrieves batches for a particular business date.
        // Useful for daily settlement and general-ledger reconciliation monitoring.
        public async Task<IEnumerable<ReconciliationBatch>> GetByBusinessDateAsync(DateTime businessDate)
        {
            return await _dbSet
                .Where(x => x.BusinessDate == businessDate.Date)
                .OrderBy(x => x.ReconciliationType)
                .ToListAsync();
        }

        // Retrieves batches of a particular type and business date.
        // Useful for locating a specific payment-rail reconciliation run.
        public async Task<IEnumerable<ReconciliationBatch>> GetByTypeAndBusinessDateAsync(string reconciliationType, DateTime businessDate)
        {
            return await _dbSet
                .Where(x => x.ReconciliationType == reconciliationType &&
                            x.BusinessDate == businessDate.Date)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves batches within a business-date range.
        // Useful for historical reconciliation reporting.
        public async Task<IEnumerable<ReconciliationBatch>> GetByBusinessDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.BusinessDate >= startDate.Date &&
                            x.BusinessDate <= endDate.Date)
                .OrderByDescending(x => x.BusinessDate)
                .ThenBy(x => x.ReconciliationType)
                .ToListAsync();
        }

        // =========================================================
        // 3. BATCH STATUS
        // =========================================================

        // Retrieves batches matching a particular status.
        // Useful for monitoring the reconciliation processing lifecycle.
        public async Task<IEnumerable<ReconciliationBatch>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.BusinessDate)
                .ToListAsync();
        }

        // Retrieves batches awaiting reconciliation processing.
        // Useful for identifying runs that have not yet started.
        public async Task<IEnumerable<ReconciliationBatch>> GetPendingBatchesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Pending")
                .OrderBy(x => x.BusinessDate)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves batches currently being processed.
        // Useful for monitoring active reconciliation runs.
        public async Task<IEnumerable<ReconciliationBatch>> GetInProgressBatchesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "InProgress")
                .OrderBy(x => x.StartedAt)
                .ToListAsync();
        }

        // Retrieves successfully completed batches.
        // Useful for reviewing reconciliation runs completed without exceptions.
        public async Task<IEnumerable<ReconciliationBatch>> GetCompletedBatchesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Completed")
                .OrderByDescending(x => x.CompletedAt)
                .ToListAsync();
        }

        // Retrieves completed batches that contain exceptions.
        // Useful for identifying runs that need unmatched-record investigation.
        public async Task<IEnumerable<ReconciliationBatch>> GetCompletedWithExceptionsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "CompletedWithExceptions")
                .OrderByDescending(x => x.CompletedAt)
                .ToListAsync();
        }

        // Retrieves batches that failed processing.
        // Useful for investigating unsuccessful reconciliation runs.
        public async Task<IEnumerable<ReconciliationBatch>> GetFailedBatchesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Failed")
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. PAYMENT METHOD AND INITIATION
        // =========================================================

        // Retrieves batches associated with a particular payment method.
        // Useful for reviewing reconciliation activity for a configured payment rail.
        public async Task<IEnumerable<ReconciliationBatch>> GetByPaymentMethodIdAsync(int paymentMethodId)
        {
            return await _dbSet
                .Where(x => x.PaymentMethodId == paymentMethodId)
                .OrderByDescending(x => x.BusinessDate)
                .ToListAsync();
        }

        // Retrieves batches initiated by a particular user.
        // Useful for auditing which staff members initiated reconciliation runs.
        public async Task<IEnumerable<ReconciliationBatch>> GetByInitiatedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.InitiatedByUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. RECONCILIATION RESULTS
        // =========================================================

        // Retrieves batches containing unmatched records or a non-zero difference.
        // Useful for prioritizing reconciliation exceptions that require investigation.
        public async Task<IEnumerable<ReconciliationBatch>> GetBatchesWithDifferencesAsync()
        {
            return await _dbSet
                .Where(x => x.UnmatchedRecords > 0 || x.DifferenceAmount != 0)
                .OrderByDescending(x => x.BusinessDate)
                .ToListAsync();
        }

        // Retrieves batches with their reconciliation records.
        // Useful for reviewing individual matched and unmatched settlement entries.
        public async Task<IEnumerable<ReconciliationBatch>> GetBatchesWithRecordsAsync()
        {
            return await _dbSet
                .Include(x => x.ReconciliationRecords)
                .OrderByDescending(x => x.BusinessDate)
                .ToListAsync();
        }

        // Retrieves batches completed within a date range.
        // Useful for reporting reconciliation completion activity.
        public async Task<IEnumerable<ReconciliationBatch>> GetCompletedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CompletedAt.HasValue &&
                            x.CompletedAt.Value >= startDate &&
                            x.CompletedAt.Value <= endDate)
                .OrderByDescending(x => x.CompletedAt)
                .ToListAsync();
        }

        // =========================================================
        // 6. SEARCH AND COUNTS
        // =========================================================

        // Searches batches by batch number, source name or file name.
        // Useful for locating reconciliation runs from settlement-file information.
        public async Task<IEnumerable<ReconciliationBatch>> SearchBatchesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.BatchNumber.Contains(searchTerm) ||
                            x.SourceName.Contains(searchTerm) ||
                            (x.FileName != null && x.FileName.Contains(searchTerm)))
                .OrderByDescending(x => x.BusinessDate)
                .ToListAsync();
        }

        // Counts batches matching a particular status and business date.
        // Useful for daily reconciliation operations reporting.
        public async Task<int> CountByStatusAndBusinessDateAsync(string status, DateTime businessDate)
        {
            return await _dbSet.CountAsync(x =>
                x.Status == status &&
                x.BusinessDate == businessDate.Date);
        }
    }
}
