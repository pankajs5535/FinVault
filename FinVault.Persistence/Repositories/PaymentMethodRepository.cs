
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class PaymentMethodRepository : GenericRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. PAYMENT METHOD IDENTIFICATION
        // =========================================================

        // Retrieves a payment method using its unique code.
        // Useful for resolving a payment rail selected during payment initiation.
        public async Task<PaymentMethod?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Code == code);
        }

        // Retrieves a payment method using its display name.
        // Useful for displaying and selecting payment methods in banking channels.
        public async Task<PaymentMethod?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }

        // Retrieves complete payment method details with fees, requests and reconciliation batches.
        // Useful for reviewing the configuration and operational usage of a payment method.
        public async Task<PaymentMethod?> GetPaymentMethodDetailsAsync(int paymentMethodId)
        {
            return await _dbSet
                .Include(x => x.PaymentRequests)
                .Include(x => x.Fees)
                .Include(x => x.ReconciliationBatches)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.PaymentMethodId == paymentMethodId);
        }

        // Checks whether a payment method code already exists.
        // Useful for preventing duplicate payment method definitions.
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _dbSet.AnyAsync(x => x.Code == code);
        }

        // =========================================================
        // 2. RAIL AND SETTLEMENT CLASSIFICATION
        // =========================================================

        // Retrieves payment methods belonging to a particular rail.
        // Useful for separating UPI, NEFT, RTGS, IMPS, NACH, cheque, card and internal transfers.
        public async Task<IEnumerable<PaymentMethod>> GetByRailAsync(string rail)
        {
            return await _dbSet
                .Where(x => x.Rail == rail)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves payment methods using a particular settlement type.
        // Useful for identifying real-time, deferred-net and batch settlement methods.
        public async Task<IEnumerable<PaymentMethod>> GetBySettlementTypeAsync(string settlementType)
        {
            return await _dbSet
                .Where(x => x.SettlementType == settlementType)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves payment methods that support 24x7 operation.
        // Useful for presenting payment options available outside normal banking hours.
        public async Task<IEnumerable<PaymentMethod>> Get24x7PaymentMethodsAsync()
        {
            return await _dbSet
                .Where(x => x.Is24x7)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 3. AVAILABILITY AND STATUS
        // =========================================================

        // Retrieves all active payment methods.
        // Useful for displaying currently enabled payment options to customers.
        public async Task<IEnumerable<PaymentMethod>> GetActivePaymentMethodsAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves all inactive payment methods.
        // Useful for administrative review of disabled payment rails.
        public async Task<IEnumerable<PaymentMethod>> GetInactivePaymentMethodsAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves active payment methods that support the requested amount.
        // Useful for filtering payment options based on configured minimum and maximum limits.
        public async Task<IEnumerable<PaymentMethod>> GetAvailableMethodsForAmountAsync(decimal amount)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            amount >= x.MinAmount &&
                            (!x.MaxAmount.HasValue || amount <= x.MaxAmount.Value))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 4. LIMIT AND SETTLEMENT CONFIGURATION
        // =========================================================

        // Retrieves payment methods whose minimum amount is within the supplied range.
        // Useful for reviewing payment options based on minimum transfer requirements.
        public async Task<IEnumerable<PaymentMethod>> GetByMinimumAmountRangeAsync(decimal minimumAmount, decimal maximumAmount)
        {
            return await _dbSet
                .Where(x => x.MinAmount >= minimumAmount && x.MinAmount <= maximumAmount)
                .OrderBy(x => x.MinAmount)
                .ToListAsync();
        }

        // Retrieves payment methods with a configured daily limit above the supplied amount.
        // Useful for identifying rails that support higher daily payment volumes.
        public async Task<IEnumerable<PaymentMethod>> GetWithDailyLimitAboveAsync(decimal minimumDailyLimit)
        {
            return await _dbSet
                .Where(x => x.DailyLimit.HasValue && x.DailyLimit.Value > minimumDailyLimit)
                .OrderByDescending(x => x.DailyLimit)
                .ToListAsync();
        }

        // Retrieves payment methods with no configured maximum transaction amount.
        // Useful for reviewing methods whose upper transaction limit is not defined in this configuration.
        public async Task<IEnumerable<PaymentMethod>> GetWithoutMaximumAmountAsync()
        {
            return await _dbSet
                .Where(x => x.MaxAmount == null)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 5. RELATED PAYMENT OPERATIONS
        // =========================================================

        // Retrieves a payment method together with its payment requests.
        // Useful for reviewing payment activity associated with a method.
        public async Task<PaymentMethod?> GetWithPaymentRequestsAsync(int paymentMethodId)
        {
            return await _dbSet
                .Include(x => x.PaymentRequests)
                .FirstOrDefaultAsync(x => x.PaymentMethodId == paymentMethodId);
        }

        // Retrieves a payment method together with its configured fees.
        // Useful for reviewing the fees associated with a payment rail.
        public async Task<PaymentMethod?> GetWithFeesAsync(int paymentMethodId)
        {
            return await _dbSet
                .Include(x => x.Fees)
                .FirstOrDefaultAsync(x => x.PaymentMethodId == paymentMethodId);
        }

        // Retrieves a payment method together with its reconciliation batches.
        // Useful for reviewing settlement and reconciliation activity by payment rail.
        public async Task<PaymentMethod?> GetWithReconciliationBatchesAsync(int paymentMethodId)
        {
            return await _dbSet
                .Include(x => x.ReconciliationBatches)
                .FirstOrDefaultAsync(x => x.PaymentMethodId == paymentMethodId);
        }

        // Retrieves payment methods that have at least one payment request.
        // Useful for identifying payment methods already used in customer payment activity.
        public async Task<IEnumerable<PaymentMethod>> GetMethodsWithPaymentRequestsAsync()
        {
            return await _dbSet
                .Where(x => x.PaymentRequests.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves payment methods that have at least one configured fee.
        // Useful for reviewing payment methods with fee configurations.
        public async Task<IEnumerable<PaymentMethod>> GetMethodsWithFeesAsync()
        {
            return await _dbSet
                .Where(x => x.Fees.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves payment methods that have at least one reconciliation batch.
        // Useful for identifying payment rails with recorded reconciliation activity.
        public async Task<IEnumerable<PaymentMethod>> GetMethodsWithReconciliationBatchesAsync()
        {
            return await _dbSet
                .Where(x => x.ReconciliationBatches.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 6. ADMINISTRATION AND SEARCH
        // =========================================================

        // Retrieves payment methods created within the supplied date range.
        // Useful for auditing newly introduced payment options.
        public async Task<IEnumerable<PaymentMethod>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Searches payment methods by code, name, rail or description.
        // Useful for locating payment configurations in administrative screens.
        public async Task<IEnumerable<PaymentMethod>> SearchPaymentMethodsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.Code.Contains(searchTerm) ||
                            x.Name.Contains(searchTerm) ||
                            x.Rail.Contains(searchTerm) ||
                            (x.Description != null && x.Description.Contains(searchTerm)))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Checks whether a payment method is referenced by payment requests, fees or reconciliation batches.
        // Useful for preventing unsafe deletion of payment methods already used in banking operations.
        public async Task<bool> IsPaymentMethodInUseAsync(int paymentMethodId)
        {
            return await _dbSet
                .AnyAsync(x => x.PaymentMethodId == paymentMethodId &&
                               (x.PaymentRequests.Any() ||
                                x.Fees.Any() ||
                                x.ReconciliationBatches.Any()));
        }
    }
}
