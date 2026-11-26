using FinVault.Application.DTOs.RefundDtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinVault.Application.Interfaces.Services
{
    public interface IRefundService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<RefundDto>> GetAllAsync();
        Task<RefundDto?> GetByIdAsync(int refundId);
        Task<RefundDto?> GetByRefundReferenceAsync(string refundReference);
        Task<RefundDto?> GetRefundDetailsAsync(int refundId);
        Task<RefundDto> CreateAsync(CreateRefundDto dto);
        Task<RefundDto?> UpdateAsync(UpdateRefundDto dto);
        Task<bool> DeleteAsync(DeleteRefundDto dto);
        Task<bool> ExistsByRefundReferenceAsync(string refundReference);

        // 2. ORIGINAL TRANSACTION AND DISPUTE HISTORY
        Task<IEnumerable<RefundDto>> GetByOriginalTransactionIdAsync(int originalTransactionId);
        Task<RefundDto?> GetByRefundTransactionIdAsync(int refundTransactionId);
        Task<IEnumerable<RefundDto>> GetByDisputeIdAsync(int disputeId);
        Task<IEnumerable<RefundDto>> GetRefundsWithTransactionsAsync(int originalTransactionId);

        // 3. REFUND STATUS AND TYPE
        Task<IEnumerable<RefundDto>> GetByStatusAsync(string status);
        Task<IEnumerable<RefundDto>> GetRequestedRefundsAsync();
        Task<IEnumerable<RefundDto>> GetApprovedRefundsAsync();
        Task<IEnumerable<RefundDto>> GetProcessedRefundsAsync();
        Task<IEnumerable<RefundDto>> GetRejectedRefundsAsync();
        Task<IEnumerable<RefundDto>> GetFailedRefundsAsync();
        Task<IEnumerable<RefundDto>> GetByRefundTypeAsync(string refundType);
        Task<IEnumerable<RefundDto>> GetOutstandingRefundsAsync();

        // 4. APPROVAL AND REQUEST TRACKING
        Task<IEnumerable<RefundDto>> GetByRequestedByUserIdAsync(int userId);
        Task<IEnumerable<RefundDto>> GetByApprovedByUserIdAsync(int userId);
        Task<IEnumerable<RefundDto>> GetRequestedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<RefundDto>> GetProcessedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 5. FINANCIAL REPORTING
        Task<decimal> GetTotalProcessedAmountByOriginalTransactionIdAsync(int originalTransactionId);
        Task<decimal> GetTotalProcessedAmountByDisputeIdAsync(int disputeId);
        Task<decimal> GetTotalProcessedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> CountByOriginalTransactionIdAsync(int originalTransactionId);

        // 6. SEARCH
        Task<IEnumerable<RefundDto>> SearchRefundsAsync(string searchTerm);
    }
}


//Banking rules:
//-Enforce maker - checker approval; the requester must not approve their own refund.
//- Prevent cumulative processed refunds from exceeding the original transaction amount.
//- A processed refund must be linked to its resulting refund transaction.
//- Preserve refund records and audit history; avoid deleting financial records.
//- Use RowVersion for concurrency checks during updates.