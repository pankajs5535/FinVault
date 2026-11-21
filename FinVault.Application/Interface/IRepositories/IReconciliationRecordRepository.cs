
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IReconciliationRecordRepository : IGenericRepository<ReconciliationRecord>
    {
        // =========================================================
        // 1. RECORD IDENTIFICATION
        // =========================================================

        // Retrieves a reconciliation record using its primary key.
        // Useful for opening an individual settlement match result.
        Task<ReconciliationRecord?> GetRecordDetailsAsync(int reconciliationRecordId);

        // Retrieves records using an external settlement reference.
        // Useful for tracing a settlement entry received from a bank or payment provider.
        Task<IEnumerable<ReconciliationRecord>> GetByExternalReferenceAsync(string externalReference);

        // Checks whether an external reference already exists in reconciliation records.
        // Useful for detecting possible duplicate settlement entries before processing.
        Task<bool> ExistsByExternalReferenceAsync(string externalReference);

        // =========================================================
        // 2. BATCH AND TRANSACTION RELATIONSHIPS
        // =========================================================

        // Retrieves records belonging to a reconciliation batch.
        // Useful for reviewing all settlement entries processed in one reconciliation run.
        Task<IEnumerable<ReconciliationRecord>> GetByBatchIdAsync(int reconciliationBatchId);

        // Retrieves records belonging to a batch with their internal transactions.
        // Useful for comparing external settlement entries against internal banking transactions.
        Task<IEnumerable<ReconciliationRecord>> GetBatchRecordsWithTransactionsAsync(int reconciliationBatchId);

        // Retrieves records linked to an internal transaction.
        // Useful for tracing which external settlement entries correspond to a banking transaction.
        Task<IEnumerable<ReconciliationRecord>> GetByTransactionIdAsync(int transactionId);

        // Retrieves records that have not been linked to an internal transaction.
        // Useful for identifying settlement entries requiring transaction matching.
        Task<IEnumerable<ReconciliationRecord>> GetWithoutTransactionAsync(int reconciliationBatchId);

        // =========================================================
        // 3. MATCH STATUS
        // =========================================================

        // Retrieves records matching a particular match status.
        // Useful for monitoring matched and exceptional settlement entries.
        Task<IEnumerable<ReconciliationRecord>> GetByMatchStatusAsync(string matchStatus);

        // Retrieves successfully matched records in a batch.
        // Useful for reviewing settlement entries reconciled with internal transactions.
        Task<IEnumerable<ReconciliationRecord>> GetMatchedRecordsAsync(int reconciliationBatchId);

        // Retrieves unmatched records in a batch.
        // Useful for investigating external entries without an internal match.
        Task<IEnumerable<ReconciliationRecord>> GetUnmatchedRecordsAsync(int reconciliationBatchId);

        // Retrieves records with amount mismatches in a batch.
        // Useful for investigating differences between external and internal transaction amounts.
        Task<IEnumerable<ReconciliationRecord>> GetAmountMismatchRecordsAsync(int reconciliationBatchId);

        // Retrieves records missing from the bank settlement data.
        // Useful for identifying internal transactions absent from the external settlement file.
        Task<IEnumerable<ReconciliationRecord>> GetMissingInBankRecordsAsync(int reconciliationBatchId);

        // Retrieves records missing from the external settlement data.
        // Useful for identifying external settlement entries absent from internal records.
        Task<IEnumerable<ReconciliationRecord>> GetMissingInExternalRecordsAsync(int reconciliationBatchId);

        // Retrieves records resolved through manual review.
        // Useful for auditing exceptions handled by reconciliation staff.
        Task<IEnumerable<ReconciliationRecord>> GetManuallyResolvedRecordsAsync(int reconciliationBatchId);

        // =========================================================
        // 4. AMOUNT AND DATE ANALYSIS
        // =========================================================

        // Retrieves records whose difference amount is non-zero.
        // Useful for identifying financial discrepancies requiring investigation.
        Task<IEnumerable<ReconciliationRecord>> GetRecordsWithDifferencesAsync(int reconciliationBatchId);

        // Retrieves records with external dates within a supplied range.
        // Useful for reviewing settlement entries across business dates.
        Task<IEnumerable<ReconciliationRecord>> GetByExternalDateRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves records with an external amount within a supplied range.
        // Useful for investigating settlement entries by transaction value.
        Task<IEnumerable<ReconciliationRecord>> GetByExternalAmountRangeAsync(decimal minimumAmount, decimal maximumAmount);

        // =========================================================
        // 5. RESOLUTION AND AUDIT
        // =========================================================

        // Retrieves records resolved by a particular user.
        // Useful for reviewing staff responsibility for reconciliation exceptions.
        Task<IEnumerable<ReconciliationRecord>> GetByResolvedByUserIdAsync(int userId);

        // Retrieves records resolved within a supplied date range.
        // Useful for reporting reconciliation exception resolution activity.
        Task<IEnumerable<ReconciliationRecord>> GetResolvedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves unresolved records from a batch.
        // Useful for identifying exceptions that still require reconciliation review.
        Task<IEnumerable<ReconciliationRecord>> GetUnresolvedRecordsAsync(int reconciliationBatchId);

        // =========================================================
        // 6. SEARCH AND COUNTS
        // =========================================================

        // Searches records by external reference, exception reason or resolution notes.
        // Useful for locating settlement discrepancies during investigation.
        Task<IEnumerable<ReconciliationRecord>> SearchRecordsAsync(string searchTerm);

        // Counts records in a batch matching a particular match status.
        // Useful for calculating reconciliation result summaries.
        Task<int> CountByBatchAndMatchStatusAsync(int reconciliationBatchId, string matchStatus);

        // Calculates the total external amount for records in a batch.
        // Useful for comparing imported settlement totals with the source file control total.
        Task<decimal> GetTotalExternalAmountByBatchAsync(int reconciliationBatchId);

        // Calculates the total internal amount for records in a batch.
        // Useful for comparing matched internal transaction values with external settlement totals.
        Task<decimal> GetTotalInternalAmountByBatchAsync(int reconciliationBatchId);

        // Calculates the total difference amount for records in a batch.
        // Useful for summarizing the financial discrepancy across a reconciliation run.
        Task<decimal> GetTotalDifferenceAmountByBatchAsync(int reconciliationBatchId);
    }
}
