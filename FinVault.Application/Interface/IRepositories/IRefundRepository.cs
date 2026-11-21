
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IRefundRepository : IGenericRepository<Refund>
    {
        // =========================================================
        // 1. REFUND IDENTIFICATION
        // =========================================================

        // Retrieves a refund using its unique refund reference.
        // Useful for tracking a refund throughout the refund lifecycle.
        Task<Refund?> GetByRefundReferenceAsync(string refundReference);

        // Retrieves complete refund details with original transaction, refund transaction, dispute and users.
        // Useful for investigating the refund and its associated banking records.
        Task<Refund?> GetRefundDetailsAsync(int refundId);

        // Checks whether a refund reference already exists.
        // Useful for preventing duplicate refund identifiers.
        Task<bool> ExistsByRefundReferenceAsync(string refundReference);

        // =========================================================
        // 2. ORIGINAL TRANSACTION HISTORY
        // =========================================================

        // Retrieves refunds linked to an original transaction.
        // Useful for reviewing all refund requests raised against a transaction.
        Task<IEnumerable<Refund>> GetByOriginalTransactionIdAsync(int originalTransactionId);

        // Retrieves refunds linked to a resulting refund transaction.
        // Useful for tracing a posted refund transaction back to its refund record.
        Task<Refund?> GetByRefundTransactionIdAsync(int refundTransactionId);

        // Retrieves refunds linked to a particular dispute.
        // Useful for reviewing refunds raised as part of dispute resolution.
        Task<IEnumerable<Refund>> GetByDisputeIdAsync(int disputeId);

        // Retrieves refunds with their original and resulting transactions.
        // Useful for investigating the complete financial movement of a refund.
        Task<IEnumerable<Refund>> GetRefundsWithTransactionsAsync(int originalTransactionId);

        // =========================================================
        // 3. REFUND STATUS AND TYPE
        // =========================================================

        // Retrieves refunds matching a particular lifecycle status.
        // Useful for monitoring requested, approved, rejected, processed and failed refunds.
        Task<IEnumerable<Refund>> GetByStatusAsync(string status);

        // Retrieves refund requests awaiting approval.
        // Useful for managing the refund approval queue.
        Task<IEnumerable<Refund>> GetRequestedRefundsAsync();

        // Retrieves approved refunds awaiting processing.
        // Useful for identifying refunds ready for financial execution.
        Task<IEnumerable<Refund>> GetApprovedRefundsAsync();

        // Retrieves successfully processed refunds.
        // Useful for reviewing completed refund operations.
        Task<IEnumerable<Refund>> GetProcessedRefundsAsync();

        // Retrieves rejected refund requests.
        // Useful for reviewing refund decisions and rejection reasons.
        Task<IEnumerable<Refund>> GetRejectedRefundsAsync();

        // Retrieves failed refund operations.
        // Useful for investigating refunds that could not be completed.
        Task<IEnumerable<Refund>> GetFailedRefundsAsync();

        // Retrieves refunds matching a particular refund type.
        // Useful for separating full refunds from partial refunds.
        Task<IEnumerable<Refund>> GetByRefundTypeAsync(string refundType);

        // =========================================================
        // 4. APPROVAL AND REQUEST TRACKING
        // =========================================================

        // Retrieves refunds requested by a particular user.
        // Useful for auditing refund initiation activity.
        Task<IEnumerable<Refund>> GetByRequestedByUserIdAsync(int userId);

        // Retrieves refunds approved by a particular user.
        // Useful for auditing refund approval activity and maker-checker controls.
        Task<IEnumerable<Refund>> GetByApprovedByUserIdAsync(int userId);

        // Retrieves refunds requested within the supplied date range.
        // Useful for monitoring refund request volumes over a reporting period.
        Task<IEnumerable<Refund>> GetRequestedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves refunds processed within the supplied date range.
        // Useful for reporting refunds by their actual processing date.
        Task<IEnumerable<Refund>> GetProcessedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves refunds that have not reached a final lifecycle status.
        // Useful for monitoring outstanding refund work queues.
        Task<IEnumerable<Refund>> GetOutstandingRefundsAsync();

        // =========================================================
        // 5. REFUND AMOUNT AND FINANCIAL REPORTING
        // =========================================================

        // Calculates the total processed refund amount for an original transaction.
        // Useful for monitoring how much has already been refunded against a transaction.
        Task<decimal> GetTotalProcessedAmountByOriginalTransactionIdAsync(int originalTransactionId);

        // Calculates the total processed refund amount linked to a dispute.
        // Useful for reviewing the refund amount granted during dispute resolution.
        Task<decimal> GetTotalProcessedAmountByDisputeIdAsync(int disputeId);

        // Calculates the total processed refund amount within a date range.
        // Useful for reporting completed refund volumes.
        Task<decimal> GetTotalProcessedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Counts refunds linked to an original transaction.
        // Useful for monitoring the number of refund requests against a transaction.
        Task<int> CountByOriginalTransactionIdAsync(int originalTransactionId);

        // =========================================================
        // 6. SEARCH
        // =========================================================

        // Searches refunds by refund reference, reason, rejection reason or original transaction reference.
        // Useful for locating refund records during customer support and investigation.
        Task<IEnumerable<Refund>> SearchRefundsAsync(string searchTerm);
    }
}
