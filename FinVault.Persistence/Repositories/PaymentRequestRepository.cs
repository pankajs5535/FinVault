
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class PaymentRequestRepository : GenericRepository<PaymentRequest>, IPaymentRequestRepository
    {
        public PaymentRequestRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. PAYMENT REQUEST IDENTIFICATION
        // =========================================================

        // Retrieves a payment request using its unique request reference.
        // Useful for tracking a payment instruction across banking channels.
        public async Task<PaymentRequest?> GetByRequestReferenceAsync(string requestReference)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.RequestReference == requestReference);
        }

        // Retrieves a payment request using its unique idempotency key.
        // Useful for preventing duplicate payment processing when clients retry requests.
        public async Task<PaymentRequest?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey);
        }

        // Retrieves a payment request using its payment network UTR or reference number.
        // Useful for tracing payments during settlement and reconciliation.
        public async Task<PaymentRequest?> GetByUtrNumberAsync(string utrNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UtrNumber == utrNumber);
        }

        // Retrieves complete payment request details with customer, account, beneficiary, currency, transaction and gateway logs.
        // Useful for investigating the full payment lifecycle.
        public async Task<PaymentRequest?> GetPaymentRequestDetailsAsync(int paymentRequestId)
        {
            return await _dbSet
                .Include(x => x.PaymentMethod)
                .Include(x => x.Customer)
                .Include(x => x.SourceAccount)
                .Include(x => x.Beneficiary)
                .Include(x => x.Currency)
                .Include(x => x.Transaction)
                .Include(x => x.GatewayLogs)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.PaymentRequestId == paymentRequestId);
        }

        // Checks whether a request reference already exists.
        // Useful for preventing duplicate payment request identifiers.
        public async Task<bool> ExistsByRequestReferenceAsync(string requestReference)
        {
            return await _dbSet.AnyAsync(x => x.RequestReference == requestReference);
        }

        // Checks whether an idempotency key already exists.
        // Useful for ensuring repeated client submissions do not create duplicate payment instructions.
        public async Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _dbSet.AnyAsync(x => x.IdempotencyKey == idempotencyKey);
        }

        // =========================================================
        // 2. CUSTOMER AND ACCOUNT PAYMENT HISTORY
        // =========================================================

        // Retrieves payment requests belonging to a customer.
        // Useful for displaying a customer's payment history.
        public async Task<IEnumerable<PaymentRequest>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests initiated from a source account.
        // Useful for reviewing outgoing payment instructions from an account.
        public async Task<IEnumerable<PaymentRequest>> GetBySourceAccountIdAsync(int sourceAccountId)
        {
            return await _dbSet
                .Where(x => x.SourceAccountId == sourceAccountId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests sent to a saved beneficiary.
        // Useful for reviewing payments made to a particular beneficiary.
        public async Task<IEnumerable<PaymentRequest>> GetByBeneficiaryIdAsync(int beneficiaryId)
        {
            return await _dbSet
                .Where(x => x.BeneficiaryId == beneficiaryId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests created through a particular payment method.
        // Useful for reviewing UPI, NEFT, RTGS, IMPS and other payment rail activity.
        public async Task<IEnumerable<PaymentRequest>> GetByPaymentMethodIdAsync(int paymentMethodId)
        {
            return await _dbSet
                .Where(x => x.PaymentMethodId == paymentMethodId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests in a particular currency.
        // Useful for reviewing payment activity by currency.
        public async Task<IEnumerable<PaymentRequest>> GetByCurrencyIdAsync(int currencyId)
        {
            return await _dbSet
                .Where(x => x.CurrencyId == currencyId)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. PAYMENT LIFECYCLE AND STATUS
        // =========================================================

        // Retrieves payment requests matching a particular lifecycle status.
        // Useful for monitoring payment processing queues and outcomes.
        public async Task<IEnumerable<PaymentRequest>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests awaiting authorization.
        // Useful for managing payment approval workflows.
        public async Task<IEnumerable<PaymentRequest>> GetAuthorizedPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Authorized")
                .OrderBy(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves payment requests submitted to the payment network.
        // Useful for monitoring requests handed over to the payment rail.
        public async Task<IEnumerable<PaymentRequest>> GetSubmittedPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Submitted")
                .OrderBy(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests currently being processed.
        // Useful for tracking payments awaiting a final network response.
        public async Task<IEnumerable<PaymentRequest>> GetProcessingPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Processing")
                .OrderBy(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves completed payment requests.
        // Useful for reviewing payment instructions that reached a completed state.
        public async Task<IEnumerable<PaymentRequest>> GetCompletedPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Completed")
                .OrderByDescending(x => x.CompletedAt)
                .ToListAsync();
        }

        // Retrieves failed payment requests.
        // Useful for investigating unsuccessful payment instructions.
        public async Task<IEnumerable<PaymentRequest>> GetFailedPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Failed")
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves expired payment requests.
        // Useful for identifying payment instructions that were not completed before expiry.
        public async Task<IEnumerable<PaymentRequest>> GetExpiredPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Expired")
                .OrderByDescending(x => x.ExpiresAt)
                .ToListAsync();
        }

        // Retrieves payment requests that were cancelled.
        // Useful for reviewing customer-cancelled payment instructions.
        public async Task<IEnumerable<PaymentRequest>> GetCancelledPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Cancelled")
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. CHANNEL AND SCHEDULING
        // =========================================================

        // Retrieves payment requests created through a particular channel.
        // Useful for comparing Branch, NetBanking, MobileApp, UPI and API activity.
        public async Task<IEnumerable<PaymentRequest>> GetByChannelAsync(string channel)
        {
            return await _dbSet
                .Where(x => x.Channel == channel)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests scheduled for a particular date range.
        // Useful for monitoring future-dated payment instructions.
        public async Task<IEnumerable<PaymentRequest>> GetScheduledPaymentsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ScheduledFor.HasValue &&
                            x.ScheduledFor.Value >= startDate &&
                            x.ScheduledFor.Value <= endDate)
                .OrderBy(x => x.ScheduledFor)
                .ToListAsync();
        }

        // Retrieves scheduled payment requests that are due for processing.
        // Useful for identifying scheduled instructions whose execution time has arrived.
        public async Task<IEnumerable<PaymentRequest>> GetScheduledPaymentsDueAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.ScheduledFor.HasValue &&
                            x.ScheduledFor.Value <= currentDateTime &&
                            (x.Status == "Created" || x.Status == "Authorized"))
                .OrderBy(x => x.ScheduledFor)
                .ToListAsync();
        }

        // Retrieves payment requests that have passed their expiry time but are not in a final state.
        // Useful for identifying requests that may require expiry processing.
        public async Task<IEnumerable<PaymentRequest>> GetExpiredPendingRequestsAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.ExpiresAt.HasValue &&
                            x.ExpiresAt.Value <= currentDateTime &&
                            (x.Status == "Created" ||
                             x.Status == "Authorized" ||
                             x.Status == "Submitted" ||
                             x.Status == "Processing"))
                .OrderBy(x => x.ExpiresAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. DATE-BASED REPORTING
        // =========================================================

        // Retrieves payment requests created within the supplied date range.
        // Useful for daily and monthly payment volume reporting.
        public async Task<IEnumerable<PaymentRequest>> GetRequestedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.RequestedAt >= startDate && x.RequestedAt <= endDate)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests completed within the supplied date range.
        // Useful for reporting completed payment activity by completion date.
        public async Task<IEnumerable<PaymentRequest>> GetCompletedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Completed" &&
                            x.CompletedAt.HasValue &&
                            x.CompletedAt.Value >= startDate &&
                            x.CompletedAt.Value <= endDate)
                .OrderByDescending(x => x.CompletedAt)
                .ToListAsync();
        }

        // Retrieves recently created payment requests for a customer.
        // Useful for displaying the customer's latest payment activity.
        public async Task<IEnumerable<PaymentRequest>> GetRecentRequestsByCustomerIdAsync(int customerId, int count)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.RequestedAt)
                .Take(Math.Max(0, count))
                .ToListAsync();
        }

        // =========================================================
        // 6. TRANSACTION AND GATEWAY TRACKING
        // =========================================================

        // Retrieves a payment request with its resulting banking transaction.
        // Useful for tracing the payment instruction to its financial transaction record.
        public async Task<PaymentRequest?> GetWithTransactionAsync(int paymentRequestId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .FirstOrDefaultAsync(x => x.PaymentRequestId == paymentRequestId);
        }

        // Retrieves a payment request with its payment gateway logs.
        // Useful for investigating gateway communication and response history.
        public async Task<PaymentRequest?> GetWithGatewayLogsAsync(int paymentRequestId)
        {
            return await _dbSet
                .Include(x => x.GatewayLogs)
                .FirstOrDefaultAsync(x => x.PaymentRequestId == paymentRequestId);
        }

        // Retrieves payment requests that do not yet have a linked transaction.
        // Useful for identifying payment instructions awaiting transaction creation or completion.
        public async Task<IEnumerable<PaymentRequest>> GetRequestsWithoutTransactionAsync()
        {
            return await _dbSet
                .Where(x => x.TransactionId == null)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Retrieves payment requests with a particular failure code.
        // Useful for analyzing recurring payment and network failure reasons.
        public async Task<IEnumerable<PaymentRequest>> GetByFailureCodeAsync(string failureCode)
        {
            return await _dbSet
                .Where(x => x.FailureCode == failureCode)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // =========================================================
        // 7. FINANCIAL REPORTING AND SEARCH
        // =========================================================

        // Calculates the total amount of completed payment requests for a customer.
        // Useful for reviewing completed outgoing payment volume.
        public async Task<decimal> GetTotalCompletedAmountByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId && x.Status == "Completed")
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Calculates the total amount of completed payment requests within a date range.
        // Useful for reporting completed payment volumes for a reporting period.
        public async Task<decimal> GetTotalCompletedAmountWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Completed" &&
                            x.CompletedAt.HasValue &&
                            x.CompletedAt.Value >= startDate &&
                            x.CompletedAt.Value <= endDate)
                .SumAsync(x => (decimal?)x.Amount) ?? 0m;
        }

        // Searches payment requests by reference, UTR, payee details or customer name.
        // Useful for locating payment instructions during support and investigation.
        public async Task<IEnumerable<PaymentRequest>> SearchPaymentRequestsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Where(x => x.RequestReference.Contains(searchTerm) ||
                            (x.UtrNumber != null && x.UtrNumber.Contains(searchTerm)) ||
                            (x.PayeeName != null && x.PayeeName.Contains(searchTerm)) ||
                            (x.PayeeAccountNumber != null && x.PayeeAccountNumber.Contains(searchTerm)) ||
                            (x.PayeeVpa != null && x.PayeeVpa.Contains(searchTerm)) ||
                            x.Customer.FirstName.Contains(searchTerm) ||
                            x.Customer.LastName.Contains(searchTerm))
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        // Counts payment requests for a customer matching the supplied status.
        // Useful for monitoring payment processing activity by customer.
        public async Task<int> CountByCustomerAndStatusAsync(int customerId, string status)
        {
            return await _dbSet.CountAsync(x => x.CustomerId == customerId && x.Status == status);
        }
    }
}
