using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IPaymentRequestService
    {
        // 1. PAYMENT REQUEST IDENTIFICATION
        Task<IEnumerable<PaymentRequestDto>> GetAllAsync();
        Task<PaymentRequestDto?> GetByIdAsync(int paymentRequestId);
        Task<PaymentRequestDto?> GetByRequestReferenceAsync(string requestReference);
        Task<PaymentRequestDto?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task<PaymentRequestDto?> GetByUtrNumberAsync(string utrNumber);
        Task<PaymentRequestDto?> GetPaymentRequestDetailsAsync(int paymentRequestId);
        Task<bool> ExistsByRequestReferenceAsync(string requestReference);
        Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey);
        Task<PaymentRequestDto> CreateAsync(CreatePaymentRequestDto dto);
        Task<PaymentRequestDto?> UpdateAsync(UpdatePaymentRequestDto dto);
        Task<bool> DeleteAsync(DeletePaymentRequestDto dto);

        // 2. CUSTOMER AND ACCOUNT PAYMENT HISTORY
        Task<IEnumerable<PaymentRequestDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<PaymentRequestDto>> GetBySourceAccountIdAsync(int sourceAccountId);
        Task<IEnumerable<PaymentRequestDto>> GetByBeneficiaryIdAsync(int beneficiaryId);
        Task<IEnumerable<PaymentRequestDto>> GetByPaymentMethodIdAsync(int paymentMethodId);
        Task<IEnumerable<PaymentRequestDto>> GetByCurrencyIdAsync(int currencyId);

        // 3. PAYMENT LIFECYCLE AND STATUS
        Task<IEnumerable<PaymentRequestDto>> GetByStatusAsync(string status);
        Task<IEnumerable<PaymentRequestDto>> GetAuthorizedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetSubmittedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetProcessingPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetCompletedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetFailedPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetExpiredPaymentRequestsAsync();
        Task<IEnumerable<PaymentRequestDto>> GetCancelledPaymentRequestsAsync();

        // 4. CHANNEL AND SCHEDULING
        Task<IEnumerable<PaymentRequestDto>> GetByChannelAsync(string channel);
        Task<IEnumerable<PaymentRequestDto>> GetScheduledPaymentsWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<PaymentRequestDto>> GetScheduledPaymentsDueAsync(DateTime currentDateTime);
        Task<IEnumerable<PaymentRequestDto>> GetExpiredPendingRequestsAsync(DateTime currentDateTime);

        // 5. DATE-BASED REPORTING
        Task<IEnumerable<PaymentRequestDto>> GetRequestedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<PaymentRequestDto>> GetCompletedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<PaymentRequestDto>> GetRecentRequestsByCustomerIdAsync(int customerId, int count);

        // 6. TRANSACTION AND GATEWAY TRACKING
        Task<PaymentRequestDto?> GetWithTransactionAsync(int paymentRequestId);
        Task<PaymentRequestDto?> GetWithGatewayLogsAsync(int paymentRequestId);
        Task<IEnumerable<PaymentRequestDto>> GetRequestsWithoutTransactionAsync();
        Task<IEnumerable<PaymentRequestDto>> GetByFailureCodeAsync(string failureCode);

        // 7. FINANCIAL REPORTING AND SEARCH
        Task<decimal> GetTotalCompletedAmountByCustomerIdAsync(int customerId);
        Task<decimal> GetTotalCompletedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentRequestDto>> SearchPaymentRequestsAsync(string searchTerm);
        Task<int> CountByCustomerAndStatusAsync(int customerId, string status);

        // 8. UPI PAYMENT OPERATIONS
        Task<UpiTransferResponseDto> InitiateUpiTransferAsync(InitiateUpiTransferDto dto);
        Task<bool> ProcessUpiProviderCallbackAsync(UpiProviderCallbackDto dto);
        Task<UpiTransactionStatusDto?> GetUpiTransactionStatusAsync(int paymentRequestId);
        Task<UpiVpaVerificationResponseDto> VerifyUpiVpaAsync(VerifyUpiVpaDto dto);
    }
}

//Banking rules:
//-Enforce idempotency to prevent duplicate payment instructions.
//- Verify customer ownership, account status, available balance, payment limits and beneficiary details.
//- Allow edits only before payment processing begins.
//- Verify provider callbacks and prevent duplicate callback processing.
//- Update payment status, account balances and linked transactions atomically.
//- Preserve payment and gateway history for audit and reconciliation.