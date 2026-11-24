using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IPaymentRequestService
    {
        // 1 CRUD AND IDENTIFICATION
        Task<IEnumerable<PaymentRequestDto>> GetAllAsync();
        Task<PaymentRequestDto?> GetByIdAsync(int paymentRequestId);
        Task<PaymentRequestDto?> GetByRequestReferenceAsync(string requestReference);
        Task<PaymentRequestDto?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task<PaymentRequestDto?> GetByUtrNumberAsync(string utrNumber);
        Task<PaymentRequestDto?> GetPaymentRequestDetailsAsync(int paymentRequestId);
        Task<PaymentRequestDto> CreateAsync(CreatePaymentRequestDto dto);
        Task<PaymentRequestDto?> UpdateAsync(UpdatePaymentRequestDto dto);
        Task<bool> DeleteAsync(int paymentRequestId);

        // 2 CUSTOMER, ACCOUNT AND PAYMENT METHOD HISTORY
        Task<IEnumerable<PaymentRequestDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<PaymentRequestDto>> GetBySourceAccountIdAsync(int sourceAccountId);
        Task<IEnumerable<PaymentRequestDto>> GetByBeneficiaryIdAsync(int beneficiaryId);
        Task<IEnumerable<PaymentRequestDto>> GetByPaymentMethodIdAsync(int paymentMethodId);
        Task<IEnumerable<PaymentRequestDto>> GetByCurrencyIdAsync(int currencyId);
        Task<IEnumerable<PaymentRequestDto>> GetRecentRequestsByCustomerIdAsync(int customerId, int count);

        // 3 PAYMENT LIFECYCLE AND STATUS
        Task<IEnumerable<PaymentRequestDto>> GetByStatusAsync(string status);
        Task<IEnumerable<PaymentRequestDto>> GetAuthorizedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetSubmittedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetProcessingPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetCompletedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetFailedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetExpiredPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetCancelledPaymentRequestsAsync();
        Task<bool> AuthorizeAsync(int paymentRequestId);
        Task<bool> SubmitAsync(int paymentRequestId);
        Task<bool> MarkAsProcessingAsync(int paymentRequestId);
        Task<bool> CompleteAsync(int paymentRequestId, string utrNumber, int? transactionId);
        Task<bool> FailAsync(int paymentRequestId, string failureCode, string failureReason);
        Task<bool> RejectAsync(int paymentRequestId, string failureReason);
        Task<bool> CancelAsync(int paymentRequestId);
        Task<bool> ExpireAsync(int paymentRequestId);

        // 4 CHANNEL AND SCHEDULING
        Task<IEnumerable<PaymentRequestDto>> GetByChannelAsync(string channel);
        Task<IEnumerable<PaymentRequestDto>> GetScheduledPaymentsWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentRequestDto>> GetScheduledPaymentsDueAsync(DateTime currentDateTime);
        Task<IEnumerable<PaymentRequestDto>> GetExpiredPendingRequestsAsync(DateTime currentDateTime);

        // 5 DATE-BASED REPORTING
        Task<IEnumerable<PaymentRequestDto>> GetRequestedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentRequestDto>> GetCompletedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalCompletedAmountByCustomerIdAsync(int customerId);
        Task<decimal> GetTotalCompletedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> CountByCustomerAndStatusAsync(int customerId, string status);

        // 6 TRANSACTION AND GATEWAY TRACKING
        Task<PaymentRequestDto?> GetWithTransactionAsync(int paymentRequestId);
        Task<PaymentRequestDto?> GetWithGatewayLogsAsync(int paymentRequestId);
        Task<IEnumerable<PaymentRequestDto>> GetRequestsWithoutTransactionAsync();
        Task<IEnumerable<PaymentRequestDto>> GetByFailureCodeAsync(string failureCode);

        // 7 SEARCH AND VALIDATION
        Task<IEnumerable<PaymentRequestDto>> SearchAsync(string searchTerm);
        Task<bool> IsRequestReferenceExistsAsync(string requestReference);
        Task<bool> IsIdempotencyKeyExistsAsync(string idempotencyKey);
        Task<bool> CanBeAuthorizedAsync(int paymentRequestId);
        Task<bool> CanBeCancelledAsync(int paymentRequestId);
        Task<bool> CanBeDeletedAsync(int paymentRequestId);
    }
}