
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IPaymentMethodRepository : IGenericRepository<PaymentMethod>
    {
        // =========================================================
        // 1. PAYMENT METHOD IDENTIFICATION
        // =========================================================

        // Retrieves a payment method using its unique code.
        // Useful for resolving a payment rail selected during payment initiation.
        Task<PaymentMethod?> GetByCodeAsync(string code);

        // Retrieves a payment method using its display name.
        // Useful for displaying and selecting payment methods in banking channels.
        Task<PaymentMethod?> GetByNameAsync(string name);

        // Retrieves complete payment method details with fees, requests and reconciliation batches.
        // Useful for reviewing the configuration and operational usage of a payment method.
        Task<PaymentMethod?> GetPaymentMethodDetailsAsync(int paymentMethodId);

        // Checks whether a payment method code already exists.
        // Useful for preventing duplicate payment method definitions.
        Task<bool> ExistsByCodeAsync(string code);

        // =========================================================
        // 2. RAIL AND SETTLEMENT CLASSIFICATION
        // =========================================================

        // Retrieves payment methods belonging to a particular rail.
        // Useful for separating UPI, NEFT, RTGS, IMPS, NACH, cheque, card and internal transfers.
        Task<IEnumerable<PaymentMethod>> GetByRailAsync(string rail);

        // Retrieves payment methods using a particular settlement type.
        // Useful for identifying real-time, deferred-net and batch settlement methods.
        Task<IEnumerable<PaymentMethod>> GetBySettlementTypeAsync(string settlementType);

        // Retrieves payment methods that support 24x7 operation.
        // Useful for presenting payment options available outside normal banking hours.
        Task<IEnumerable<PaymentMethod>> Get24x7PaymentMethodsAsync();

        // =========================================================
        // 3. AVAILABILITY AND STATUS
        // =========================================================

        // Retrieves all active payment methods.
        // Useful for displaying currently enabled payment options to customers.
        Task<IEnumerable<PaymentMethod>> GetActivePaymentMethodsAsync();

        // Retrieves all inactive payment methods.
        // Useful for administrative review of disabled payment rails.
        Task<IEnumerable<PaymentMethod>> GetInactivePaymentMethodsAsync();

        // Retrieves active payment methods that support the requested amount.
        // Useful for filtering payment options based on configured minimum and maximum limits.
        Task<IEnumerable<PaymentMethod>> GetAvailableMethodsForAmountAsync(decimal amount);

        // =========================================================
        // 4. LIMIT AND SETTLEMENT CONFIGURATION
        // =========================================================

        // Retrieves payment methods whose minimum amount is within the supplied range.
        // Useful for reviewing payment options based on minimum transfer requirements.
        Task<IEnumerable<PaymentMethod>> GetByMinimumAmountRangeAsync(decimal minimumAmount, decimal maximumAmount);

        // Retrieves payment methods with a configured daily limit above the supplied amount.
        // Useful for identifying rails that support higher daily payment volumes.
        Task<IEnumerable<PaymentMethod>> GetWithDailyLimitAboveAsync(decimal minimumDailyLimit);

        // Retrieves payment methods with no configured maximum transaction amount.
        // Useful for reviewing methods whose upper transaction limit is not defined in this configuration.
        Task<IEnumerable<PaymentMethod>> GetWithoutMaximumAmountAsync();

        // =========================================================
        // 5. RELATED PAYMENT OPERATIONS
        // =========================================================

        // Retrieves payment methods together with their payment requests.
        // Useful for reviewing payment activity associated with each method.
        Task<PaymentMethod?> GetWithPaymentRequestsAsync(int paymentMethodId);

        // Retrieves payment methods together with their configured fees.
        // Useful for reviewing the fees associated with each payment rail.
        Task<PaymentMethod?> GetWithFeesAsync(int paymentMethodId);

        // Retrieves payment methods together with their reconciliation batches.
        // Useful for reviewing settlement and reconciliation activity by payment rail.
        Task<PaymentMethod?> GetWithReconciliationBatchesAsync(int paymentMethodId);

        // Retrieves payment methods that have at least one payment request.
        // Useful for identifying payment methods already used in customer payment activity.
        Task<IEnumerable<PaymentMethod>> GetMethodsWithPaymentRequestsAsync();

        // Retrieves payment methods that have at least one configured fee.
        // Useful for reviewing payment methods with fee configurations.
        Task<IEnumerable<PaymentMethod>> GetMethodsWithFeesAsync();

        // Retrieves payment methods that have at least one reconciliation batch.
        // Useful for identifying payment rails with recorded reconciliation activity.
        Task<IEnumerable<PaymentMethod>> GetMethodsWithReconciliationBatchesAsync();

        // =========================================================
        // 6. ADMINISTRATION AND SEARCH
        // =========================================================

        // Retrieves payment methods created within the supplied date range.
        // Useful for auditing newly introduced payment options.
        Task<IEnumerable<PaymentMethod>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Searches payment methods by code, name, rail or description.
        // Useful for locating payment configurations in administrative screens.
        Task<IEnumerable<PaymentMethod>> SearchPaymentMethodsAsync(string searchTerm);

        // Checks whether a payment method is referenced by payment requests, fees or reconciliation batches.
        // Useful for preventing unsafe deletion of payment methods already used in banking operations.
        Task<bool> IsPaymentMethodInUseAsync(int paymentMethodId);
    }
}
