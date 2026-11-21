
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IPaymentRequestRepository : IGenericRepository<PaymentRequest>
    {
        // =========================================================
        // 1. PAYMENT REQUEST IDENTIFICATION
        // =========================================================

        // Retrieves a payment request using its unique request reference.
        // Useful for tracking a payment instruction across banking channels.
        Task<PaymentRequest?> GetByRequestReferenceAsync(string requestReference);

        // Retrieves a payment request using its unique idempotency key.
        // Useful for preventing duplicate payment processing when clients retry requests.
        Task<PaymentRequest?> GetByIdempotencyKeyAsync(string idempotencyKey);

        // Retrieves a payment request using its payment network UTR or reference number.
        // Useful for tracing payments during settlement and reconciliation.
        Task<PaymentRequest?> GetByUtrNumberAsync(string utrNumber);

        // Retrieves complete payment request details with customer, account, beneficiary, currency, transaction and gateway logs.
        // Useful for investigating the full payment lifecycle.
        Task<PaymentRequest?> GetPaymentRequestDetailsAsync(int paymentRequestId);

        // Checks whether a request reference already exists.
        // Useful for preventing duplicate payment request identifiers.
        Task<bool> ExistsByRequestReferenceAsync(string requestReference);

        // Checks whether an idempotency key already exists.
        // Useful for ensuring repeated client submissions do not create duplicate payment instructions.
        Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey);

        // =========================================================
        // 2. CUSTOMER AND ACCOUNT PAYMENT HISTORY
        // =========================================================

        // Retrieves payment requests belonging to a customer.
        // Useful for displaying a customer's payment history.
        Task<IEnumerable<PaymentRequest>> GetByCustomerIdAsync(int customerId);

        // Retrieves payment requests initiated from a source account.
        // Useful for reviewing outgoing payment instructions from an account.
        Task<IEnumerable<PaymentRequest>> GetBySourceAccountIdAsync(int sourceAccountId);

        // Retrieves payment requests sent to a saved beneficiary.
        // Useful for reviewing payments made to a particular beneficiary.
        Task<IEnumerable<PaymentRequest>> GetByBeneficiaryIdAsync(int beneficiaryId);

        // Retrieves payment requests created through a particular payment method.
        // Useful for reviewing UPI, NEFT, RTGS, IMPS and other payment rail activity.
        Task<IEnumerable<PaymentRequest>> GetByPaymentMethodIdAsync(int paymentMethodId);

        // Retrieves payment requests in a particular currency.
        // Useful for reviewing payment activity by currency.
        Task<IEnumerable<PaymentRequest>> GetByCurrencyIdAsync(int currencyId);

        // =========================================================
        // 3. PAYMENT LIFECYCLE AND STATUS
        // =========================================================

        // Retrieves payment requests matching a particular lifecycle status.
        // Useful for monitoring payment processing queues and outcomes.
        Task<IEnumerable<PaymentRequest>> GetByStatusAsync(string status);

        // Retrieves payment requests awaiting authorization.
        // Useful for managing payment approval workflows.
        Task<IEnumerable<PaymentRequest>> GetAuthorizedPaymentRequestsAsync();

        // Retrieves payment requests submitted to the payment network.
        // Useful for monitoring requests handed over to the payment rail.
        Task<IEnumerable<PaymentRequest>> GetSubmittedPaymentRequestsAsync();

        // Retrieves payment requests currently being processed.
        // Useful for tracking payments awaiting a final network response.
        Task<IEnumerable<PaymentRequest>> GetProcessingPaymentRequestsAsync();

        // Retrieves completed payment requests.
        // Useful for reviewing payment instructions that reached a completed state.
        Task<IEnumerable<PaymentRequest>> GetCompletedPaymentRequestsAsync();

        // Retrieves failed payment requests.
        // Useful for investigating unsuccessful payment instructions.
        Task<IEnumerable<PaymentRequest>> GetFailedPaymentRequestsAsync();

        // Retrieves payment requests that expired.
        // Useful for identifying payment instructions that were not completed before expiry.
        Task<IEnumerable<PaymentRequest>> GetExpiredPaymentRequestsAsync();

        // Retrieves payment requests that were cancelled.
        // Useful for reviewing customer-cancelled payment instructions.
        Task<IEnumerable<PaymentRequest>> GetCancelledPaymentRequestsAsync();

        // =========================================================
        // 4. CHANNEL AND SCHEDULING
        // =========================================================

        // Retrieves payment requests created through a particular channel.
        // Useful for comparing Branch, NetBanking, MobileApp, UPI and API activity.
        Task<IEnumerable<PaymentRequest>> GetByChannelAsync(string channel);

        // Retrieves payment requests scheduled for a particular date range.
        // Useful for monitoring future-dated payment instructions.
        Task<IEnumerable<PaymentRequest>> GetScheduledPaymentsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves scheduled payment requests that are due for processing.
        // Useful for identifying scheduled instructions whose execution time has arrived.
        Task<IEnumerable<PaymentRequest>> GetScheduledPaymentsDueAsync(DateTime currentDateTime);

        // Retrieves payment requests that have passed their expiry time but are not in a final state.
        // Useful for identifying requests that may require expiry processing.
        Task<IEnumerable<PaymentRequest>> GetExpiredPendingRequestsAsync(DateTime currentDateTime);

        // =========================================================
        // 5. DATE-BASED REPORTING
        // =========================================================

        // Retrieves payment requests created within the supplied date range.
        // Useful for daily and monthly payment volume reporting.
        Task<IEnumerable<PaymentRequest>> GetRequestedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves payment requests completed within the supplied date range.
        // Useful for reporting completed payment activity by completion date.
        Task<IEnumerable<PaymentRequest>> GetCompletedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves recently created payment requests for a customer.
        // Useful for displaying the customer's latest payment activity.
        Task<IEnumerable<PaymentRequest>> GetRecentRequestsByCustomerIdAsync(int customerId, int count);

        // =========================================================
        // 6. TRANSACTION AND GATEWAY TRACKING
        // =========================================================

        // Retrieves a payment request with its resulting banking transaction.
        // Useful for tracing the payment instruction to its financial transaction record.
        Task<PaymentRequest?> GetWithTransactionAsync(int paymentRequestId);

        // Retrieves a payment request with its payment gateway logs.
        // Useful for investigating gateway communication and response history.
        Task<PaymentRequest?> GetWithGatewayLogsAsync(int paymentRequestId);

        // Retrieves payment requests that do not yet have a linked transaction.
        // Useful for identifying payment instructions awaiting transaction creation or completion.
        Task<IEnumerable<PaymentRequest>> GetRequestsWithoutTransactionAsync();

        // Retrieves payment requests with a particular failure code.
        // Useful for analyzing recurring payment and network failure reasons.
        Task<IEnumerable<PaymentRequest>> GetByFailureCodeAsync(string failureCode);

        // =========================================================
        // 7. FINANCIAL REPORTING AND SEARCH
        // =========================================================

        // Calculates the total amount of completed payment requests for a customer.
        // Useful for reviewing completed outgoing payment volume.
        Task<decimal> GetTotalCompletedAmountByCustomerIdAsync(int customerId);

        // Calculates the total amount of completed payment requests within a date range.
        // Useful for reporting completed payment volumes for a reporting period.
        Task<decimal> GetTotalCompletedAmountWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Searches payment requests by reference, UTR, payee details or customer name.
        // Useful for locating payment instructions during support and investigation.
        Task<IEnumerable<PaymentRequest>> SearchPaymentRequestsAsync(string searchTerm);

        // Counts payment requests for a customer matching the supplied status.
        // Useful for monitoring payment processing activity by customer.
        Task<int> CountByCustomerAndStatusAsync(int customerId, string status);
    }
}
