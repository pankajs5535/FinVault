using FinVault.Application.DTOs.ReconciliationBatchDtos;
using FinVault.Application.DTOs.ReconciliationRecordDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IReconciliationService
    {
        // 1. RECONCILIATION BATCH CRUD
        Task<IEnumerable<ReconciliationBatchDto>> GetAllBatchesAsync();
        Task<ReconciliationBatchDto?> GetBatchByIdAsync(int reconciliationBatchId);
        Task<ReconciliationBatchDto?> GetByBatchNumberAsync(string batchNumber);
        Task<ReconciliationBatchDto?> GetByFileHashAsync(string fileHash);
        Task<ReconciliationBatchDto?> GetBatchDetailsAsync(int reconciliationBatchId);
        Task<bool> ExistsByBatchNumberAsync(string batchNumber);
        Task<bool> ExistsByFileHashAsync(string fileHash);
        Task<ReconciliationBatchDto> CreateBatchAsync(CreateReconciliationBatchDto dto);
        Task<ReconciliationBatchDto?> UpdateBatchAsync(UpdateReconciliationBatchDto dto);
        Task<bool> DeleteBatchAsync(DeleteReconciliationBatchDto dto);

        // 2. BATCH TYPE AND BUSINESS DATE
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesByReconciliationTypeAsync(string reconciliationType);
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesByBusinessDateAsync(DateTime businessDate);
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesByTypeAndBusinessDateAsync(
            string reconciliationType,
            DateTime businessDate);
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesByBusinessDateRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 3. BATCH STATUS
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesByStatusAsync(string status);
        Task<IEnumerable<ReconciliationBatchDto>> GetPendingBatchesAsync();
        Task<IEnumerable<ReconciliationBatchDto>> GetInProgressBatchesAsync();
        Task<IEnumerable<ReconciliationBatchDto>> GetCompletedBatchesAsync();
        Task<IEnumerable<ReconciliationBatchDto>> GetCompletedWithExceptionsAsync();
        Task<IEnumerable<ReconciliationBatchDto>> GetFailedBatchesAsync();

        // 4. BATCH INITIATION AND RESULTS
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesByPaymentMethodIdAsync(int paymentMethodId);
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesByInitiatedByUserIdAsync(int userId);
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesWithDifferencesAsync();
        Task<IEnumerable<ReconciliationBatchDto>> GetBatchesWithRecordsAsync();
        Task<IEnumerable<ReconciliationBatchDto>> GetCompletedBatchesWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 5. RECONCILIATION RECORD CRUD
        Task<IEnumerable<ReconciliationRecordDto>> GetAllRecordsAsync();
        Task<ReconciliationRecordDto?> GetRecordByIdAsync(int reconciliationRecordId);
        Task<ReconciliationRecordDto?> GetRecordDetailsAsync(int reconciliationRecordId);
        Task<IEnumerable<ReconciliationRecordDto>> GetByExternalReferenceAsync(string externalReference);
        Task<bool> ExistsByExternalReferenceAsync(string externalReference);
        Task<ReconciliationRecordDto> CreateRecordAsync(CreateReconciliationRecordDto dto);
        Task<ReconciliationRecordDto?> UpdateRecordAsync(UpdateReconciliationRecordDto dto);
        Task<bool> DeleteRecordAsync(DeleteReconciliationRecordDto dto);

        // 6. RECORD BATCH AND TRANSACTION RELATIONSHIPS
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsByBatchIdAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetBatchRecordsWithTransactionsAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsByTransactionIdAsync(int transactionId);
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsWithoutTransactionAsync(int reconciliationBatchId);

        // 7. RECORD MATCH STATUS
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsByMatchStatusAsync(string matchStatus);
        Task<IEnumerable<ReconciliationRecordDto>> GetMatchedRecordsAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetUnmatchedRecordsAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetAmountMismatchRecordsAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetMissingInBankRecordsAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetMissingInExternalRecordsAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetManuallyResolvedRecordsAsync(int reconciliationBatchId);
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsWithDifferencesAsync(int reconciliationBatchId);

        // 8. RECORD AMOUNT AND DATE ANALYSIS
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsByExternalDateRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsByExternalAmountRangeAsync(
            decimal minimumAmount,
            decimal maximumAmount);

        // 9. EXCEPTION RESOLUTION AND AUDIT
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsByResolvedByUserIdAsync(int userId);
        Task<IEnumerable<ReconciliationRecordDto>> GetRecordsResolvedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<ReconciliationRecordDto>> GetUnresolvedRecordsAsync(int reconciliationBatchId);

        // 10. SEARCH AND REPORTING
        Task<IEnumerable<ReconciliationBatchDto>> SearchBatchesAsync(string searchTerm);
        Task<IEnumerable<ReconciliationRecordDto>> SearchRecordsAsync(string searchTerm);
        Task<int> CountBatchesByStatusAndBusinessDateAsync(string status, DateTime businessDate);
        Task<int> CountRecordsByBatchAndMatchStatusAsync(int reconciliationBatchId, string matchStatus);
        Task<decimal> GetTotalExternalAmountByBatchAsync(int reconciliationBatchId);
        Task<decimal> GetTotalInternalAmountByBatchAsync(int reconciliationBatchId);
        Task<decimal> GetTotalDifferenceAmountByBatchAsync(int reconciliationBatchId);
    }
}

//Banking rules:
//-Prevent duplicate batch numbers, file hashes, and external settlement references.
//- Validate batch totals against imported settlement records.
//- Preserve unmatched records and amount discrepancies until resolved.
//- Manual resolution must record the resolving user, timestamp, and notes.
//- Use the RowVersion concurrency token when updating batches.
//- Do not mark a batch as fully reconciled while unresolved exceptions remain.