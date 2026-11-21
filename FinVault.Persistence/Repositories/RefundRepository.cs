
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class RefundRepository : GenericRepository<Refund>, IRefundRepository
    {
        public RefundRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. REFUND IDENTIFICATION
        // =========================================================

        // Retrieves a refund using its unique refund reference.
        // Useful for tracking a refund throughout the refund lifecycle.
        public async Task<Refund?> GetByRefundReferenceAsync(string refundReference)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.RefundReference == refundReference);
        }

        // Retrieves complete refund details with original transaction, refund transaction, dispute and users.
        // Useful for investigating the refund and its associated banking records.
        public async Task<Refund?> GetRefundDetailsAsync(int refundId)
        {
            return await _dbSet
                .Include(x => x.OriginalTransaction)
                .Include(x => x.RefundTransaction)
                .Include(x => x.Dispute)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.RefundId == refundId);
        }

        // Checks whether a refund reference already exists.
        // Useful for preventing duplicate refund identifiers.
        public async Task<bool> ExistsByRefundReferenceAsync(string refundReference)
        {
            return await _dbSet.AnyAsync(x => x.RefundReference == refundReference);
        }

        // =========================================================
        // 2. ORIGINAL TRANSACTION HISTORY
        // =========================================================

        // Retrieves refunds linked to an original transaction.
        // Useful for reviewing all refund requests raised against a transaction.
        public async Task<IEnumerable<Refund>> GetByOriginalTransactionIdAsync(int originalTransactionId)
        {
            return await _dbSet
                .Where(x => x.OriginalTransactionId == originalTransactionId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves a refund linked to a resulting refund transaction.
        // Useful for tracing a posted refund transaction back to its refund record.
        public async Task<Refund?> GetByRefundTransactionIdAsync(int refundTransactionId)
        {
            return await _dbSet
                .Include(x => x.OriginalTransaction)
                .FirstOrDefaultAsync(x => x.RefundTransactionId == refundTransactionId);
        }

        // Retrieves refunds linked to a particular dispute.
        // Useful for reviewing refunds raised as part of dispute resolution.
        public async Task<IEnumerable<Refund>> GetByDisputeIdAsync(int disputeId)
        {
            return await _dbSet
                .Where(x => x.DisputeId == disputeId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves refunds with their original and resulting transactions.
        // Useful for investigating the complete financial movement of a refund.
        public async Task<IEnumerable<Refund>> GetRefundsWithTransactionsAsync(int originalTransactionId)
        {
            return await _dbSet
                .Include(x => x.OriginalTransaction)
                .Include(x => x.RefundTransaction)
                .Where(x => x.OriginalTransactionId == originalTransactionId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. REFUND STATUS AND TYPE
        // =========================================================

        // Retrieves refunds matching a particular lifecycle status.
        // Useful for monitoring requested, approved, rejected, processed and failed refunds.
        public async Task<IEnumerable<Refund>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves refund requests awaiting approval.
        // Useful for managing the refund approval queue.
        public async Task<IEnumerable<Refund>> GetRequestedRefundsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Requested")
                .OrderBy(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves approved refunds awaiting processing.
        // Useful for identifying refunds ready for financial execution.
        public async Task<IEnumerable<Refund>> GetApprovedRefundsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Approved")
                .OrderBy(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves successfully processed refunds.
        // Useful for reviewing completed refund operations.
        public async Task<IEnumerable<Refund>> GetProcessedRefundsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Processed")
                .OrderByDescending(x => x.ProcessedAt)
                .ToListAsync();
        }

        // Retrieves rejected refund requests.
        // Useful for reviewing refund decisions and rejection reasons.
        public async Task<IEnumerable<Refund>> GetRejectedRefundsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Rejected")
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves failed refund operations.
        // Useful for investigating refunds that could not be completed.
        public async Task<IEnumerable<Refund>> GetFailedRefundsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Failed")
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves refunds matching a particular refund type.
        // Useful for separating full refunds from partial refunds.
        public async Task<IEnumerable<Refund>> GetByRefundTypeAsync(string refundType)
        {
            return await _dbSet
                .Where(x => x.RefundType == refundType)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. APPROVAL AND REQUEST TRACKING
        // =========================================================

        // Retrieves refunds requested by a particular user.
        // Useful for auditing refund initiation activity.
        public async Task<IEnumerable<Refund>> GetByRequestedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.RequestedByUserId == userId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves refunds approved by a particular user.
        // Useful for auditing refund approval activity and maker-checker controls.
        public async Task<IEnumerable<Refund>> GetByApprovedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.ApprovedByUserId == userId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves refunds requested within the supplied date range.
        // Useful for monitoring refund request volumes over a reporting period.
        public async Task<IEnumerable<Refund>> GetRequestedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.RequestedAt >= startDate && x.RequestedAt <= endDate)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves refunds processed within the supplied date range.
        // Useful for reporting refunds by their actual processing date.
        public async Task<IEnumerable<Refund>> GetProcessedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Processed" &&
                            x.ProcessedAt.HasValue &&
                            x.ProcessedAt.Value >= startDate &&
                            x.ProcessedAt.Value <= endDate)
                .OrderByDescending(x => x.ProcessedAt)
                .ToListAsync();
        }

        // Retrieves refunds that have not reached a final lifecycle status.
        // Useful for monitoring outstanding refund work queues.
        public async Task<IEnumerable<Refund>> GetOutstandingRefundsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Requested" || x.Status == "Approved")
                .OrderBy(x => x.RequestedAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. REFUND AMOUNT AND FINANCIAL REPORTING
        // =========================================================

        // Calculates the total processed refund amount for an original transaction.
        // Useful for monitoring how much has already been refunded against a transaction.
        public async Task<decimal> GetTotalProcessedAmountByOriginalTransactionIdAsync(int originalTransactionId)
        {
            return await _dbSet
                .Where(x => x.OriginalTransactionId == originalTransactionId && x.Status == "Processed")
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Calculates the total processed refund amount linked to a dispute.
        // Useful for reviewing the refund amount granted during dispute resolution.
        public async Task<decimal> GetTotalProcessedAmountByDisputeIdAsync(int disputeId)
        {
            return await _dbSet
                .Where(x => x.DisputeId == disputeId && x.Status == "Processed")
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Calculates the total processed refund amount within a date range.
        // Useful for reporting completed refund volumes.
        public async Task<decimal> GetTotalProcessedAmountWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Processed" &&
                            x.ProcessedAt.HasValue &&
                            x.ProcessedAt.Value >= startDate &&
                            x.ProcessedAt.Value <= endDate)
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Counts refunds linked to an original transaction.
        // Useful for monitoring the number of refund requests against a transaction.
        public async Task<int> CountByOriginalTransactionIdAsync(int originalTransactionId)
        {
            return await _dbSet.CountAsync(x => x.OriginalTransactionId == originalTransactionId);
        }

        // =========================================================
        // 6. SEARCH
        // =========================================================

        // Searches refunds by refund reference, reason, rejection reason or original transaction reference.
        // Useful for locating refund records during customer support and investigation.
        public async Task<IEnumerable<Refund>> SearchRefundsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(x => x.OriginalTransaction)
                .Where(x => x.RefundReference.Contains(searchTerm) ||
                            x.Reason.Contains(searchTerm) ||
                            (x.RejectionReason != null && x.RejectionReason.Contains(searchTerm)) ||
                            x.OriginalTransaction.TransactionReference.Contains(searchTerm))
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }
    }
}
