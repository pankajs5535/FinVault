
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IReconciliationBatchRepository : IGenericRepository<ReconciliationBatch>
    {
        // =========================================================
        // 1. BATCH IDENTIFICATION
        // =========================================================

        // Retrieves a reconciliation batch using its unique batch number.
        // Useful for locating a specific reconciliation run during investigation.
        Task<ReconciliationBatch?> GetByBatchNumberAsync(string batchNumber);

        // Retrieves a reconciliation batch using its settlement file hash.
        // Useful for detecting whether the same settlement file has already been imported.
        Task<ReconciliationBatch?> GetByFileHashAsync(string fileHash);

        // Checks whether a batch number already exists.
        // Useful for preventing duplicate reconciliation batch identifiers.
        Task<bool> ExistsByBatchNumberAsync(string batchNumber);

        // Checks whether a settlement file hash has already been processed.
        // Useful for preventing duplicate file imports and repeated reconciliation.
        Task<bool> ExistsByFileHashAsync(string fileHash);

        // Retrieves complete batch details with payment method, users and reconciliation records.
        // Useful for reviewing the complete result of a reconciliation run.
        Task<ReconciliationBatch?> GetBatchDetailsAsync(int reconciliationBatchId);

        // =========================================================
        // 2. RECONCILIATION TYPE AND BUSINESS DATE
        // =========================================================

        // Retrieves batches for a particular reconciliation type.
        // Useful for reviewing UPI, NEFT, RTGS, IMPS, card, ATM and ledger reconciliation runs.
        Task<IEnumerable<ReconciliationBatch>> GetByReconciliationTypeAsync(string reconciliationType);

        // Retrieves batches for a particular business date.
        // Useful for daily settlement and general-ledger reconciliation monitoring.
        Task<IEnumerable<ReconciliationBatch>> GetByBusinessDateAsync(DateTime businessDate);

        // Retrieves batches of a particular type and business date.
        // Useful for locating a specific payment-rail reconciliation run.
        Task<IEnumerable<ReconciliationBatch>> GetByTypeAndBusinessDateAsync(string reconciliationType, DateTime businessDate);

        // Retrieves batches within a business-date range.
        // Useful for historical reconciliation reporting.
        Task<IEnumerable<ReconciliationBatch>> GetByBusinessDateRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 3. BATCH STATUS
        // =========================================================

        // Retrieves batches matching a particular status.
        // Useful for monitoring the reconciliation processing lifecycle.
        Task<IEnumerable<ReconciliationBatch>> GetByStatusAsync(string status);

        // Retrieves batches awaiting reconciliation processing.
        // Useful for identifying runs that have not yet started.
        Task<IEnumerable<ReconciliationBatch>> GetPendingBatchesAsync();

        // Retrieves batches currently being processed.
        // Useful for monitoring active reconciliation runs.
        Task<IEnumerable<ReconciliationBatch>> GetInProgressBatchesAsync();

        // Retrieves successfully completed batches.
        // Useful for reviewing reconciliation runs completed without exceptions.
        Task<IEnumerable<ReconciliationBatch>> GetCompletedBatchesAsync();

        // Retrieves completed batches that contain exceptions.
        // Useful for identifying runs that need unmatched-record investigation.
        Task<IEnumerable<ReconciliationBatch>> GetCompletedWithExceptionsAsync();

        // Retrieves batches that failed processing.
        // Useful for investigating unsuccessful reconciliation runs.
        Task<IEnumerable<ReconciliationBatch>> GetFailedBatchesAsync();

        // =========================================================
        // 4. PAYMENT METHOD AND INITIATION
        // =========================================================

        // Retrieves batches associated with a particular payment method.
        // Useful for reviewing reconciliation activity for a configured payment rail.
        Task<IEnumerable<ReconciliationBatch>> GetByPaymentMethodIdAsync(int paymentMethodId);

        // Retrieves batches initiated by a particular user.
        // Useful for auditing which staff members initiated reconciliation runs.
        Task<IEnumerable<ReconciliationBatch>> GetByInitiatedByUserIdAsync(int userId);

        // =========================================================
        // 5. RECONCILIATION RESULTS
        // =========================================================

        // Retrieves batches containing unmatched records or a non-zero difference.
        // Useful for prioritizing reconciliation exceptions that require investigation.
        Task<IEnumerable<ReconciliationBatch>> GetBatchesWithDifferencesAsync();

        // Retrieves batches with their reconciliation records.
        // Useful for reviewing individual matched and unmatched settlement entries.
        Task<IEnumerable<ReconciliationBatch>> GetBatchesWithRecordsAsync();

        // Retrieves batches completed within a date range.
        // Useful for reporting reconciliation completion activity.
        Task<IEnumerable<ReconciliationBatch>> GetCompletedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 6. SEARCH AND COUNTS
        // =========================================================

        // Searches batches by batch number, source name or file name.
        // Useful for locating reconciliation runs from settlement-file information.
        Task<IEnumerable<ReconciliationBatch>> SearchBatchesAsync(string searchTerm);

        // Counts batches matching a particular status and business date.
        // Useful for daily reconciliation operations reporting.
        Task<int> CountByStatusAndBusinessDateAsync(string status, DateTime businessDate);
    }
}
