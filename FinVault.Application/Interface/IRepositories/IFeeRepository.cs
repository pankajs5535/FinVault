
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IFeeRepository : IGenericRepository<Fee>
    {
        // =========================================================
        // 1. FEE IDENTIFICATION
        // =========================================================

        // Retrieves a fee using its unique fee code.
        // Useful for identifying a specific bank charge during transaction processing.
        Task<Fee?> GetByCodeAsync(string code);

        // Retrieves a fee with its related transaction type, payment method and account type.
        // Useful for displaying the complete fee configuration.
        Task<Fee?> GetFeeDetailsAsync(int feeId);

        // Retrieves fees matching a particular name.
        // Useful for locating bank charges by their business name.
        Task<IEnumerable<Fee>> GetByNameAsync(string name);

        // =========================================================
        // 2. FEE CLASSIFICATION
        // =========================================================

        // Retrieves fees using their calculation method.
        // Useful for separating flat, percentage-based and slab-based charges.
        Task<IEnumerable<Fee>> GetByCalculationMethodAsync(string calculationMethod);

        // Retrieves fees using their charging frequency.
        // Useful for identifying per-transaction, monthly, quarterly, yearly and one-time charges.
        Task<IEnumerable<Fee>> GetByFrequencyAsync(string frequency);

        // Retrieves fees associated with a transaction type.
        // Useful for applying charges to transfers and other banking transactions.
        Task<IEnumerable<Fee>> GetByTransactionTypeIdAsync(int transactionTypeId);

        // Retrieves fees associated with a payment method.
        // Useful for identifying charges applicable to a particular payment channel.
        Task<IEnumerable<Fee>> GetByPaymentMethodIdAsync(int paymentMethodId);

        // Retrieves fees associated with an account type.
        // Useful for applying account-specific banking charges.
        Task<IEnumerable<Fee>> GetByAccountTypeIdAsync(int accountTypeId);

        // =========================================================
        // 3. FEE EFFECTIVE DATE MANAGEMENT
        // =========================================================

        // Retrieves fees active within the supplied business date.
        // Useful for selecting fee configurations valid when a transaction is processed.
        Task<IEnumerable<Fee>> GetEffectiveFeesAsync(DateTime effectiveDate);

        // Retrieves fees that will become effective within the supplied date range.
        // Useful for planning upcoming changes to bank charges.
        Task<IEnumerable<Fee>> GetUpcomingFeesAsync(DateTime startDate, DateTime endDate);

        // Retrieves active fee configurations.
        // Useful for displaying fee definitions available for operational use.
        Task<IEnumerable<Fee>> GetActiveFeesAsync();

        // Retrieves inactive fee configurations.
        // Useful for reviewing discontinued or temporarily disabled bank charges.
        Task<IEnumerable<Fee>> GetInactiveFeesAsync();

        // =========================================================
        // 4. TAX AND AMOUNT MANAGEMENT
        // =========================================================

        // Retrieves fees for which GST is applicable.
        // Useful for identifying charges that require tax calculation.
        Task<IEnumerable<Fee>> GetGstApplicableFeesAsync();

        // Retrieves fees within the supplied minimum and maximum amount range.
        // Useful for reviewing fixed or percentage-based fee limits.
        Task<IEnumerable<Fee>> GetFeesWithinAmountRangeAsync(decimal minimumAmount, decimal maximumAmount);

        // =========================================================
        // 5. TRANSACTION AND VALIDATION
        // =========================================================

        // Retrieves fees that have been used by at least one transaction.
        // Useful for reviewing fee usage and financial reporting.
        Task<IEnumerable<Fee>> GetFeesWithChargedTransactionsAsync();

        // Checks whether a fee code already exists.
        // Useful for preventing duplicate fee configuration records.
        Task<bool> ExistsByCodeAsync(string code);

        // Retrieves fees created by a particular user.
        // Useful for tracking fee configuration ownership and administration.
        Task<IEnumerable<Fee>> GetFeesCreatedByUserAsync(int userId);

        // Searches fees by code or name.
        // Useful for locating fee configurations in administrative screens.
        Task<IEnumerable<Fee>> SearchFeesAsync(string searchTerm);
    }
}
