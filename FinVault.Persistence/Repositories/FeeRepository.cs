
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class FeeRepository : GenericRepository<Fee>, IFeeRepository
    {
        public FeeRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. FEE IDENTIFICATION
        // =========================================================

        // Retrieves a fee using its unique fee code.
        // Useful for identifying a specific bank charge during transaction processing.
        public async Task<Fee?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Code == code);
        }

        // Retrieves a fee with its related transaction type, payment method and account type.
        // Useful for displaying the complete fee configuration.
        public async Task<Fee?> GetFeeDetailsAsync(int feeId)
        {
            return await _dbSet
                .Include(x => x.TransactionType)
                .Include(x => x.PaymentMethod)
                .Include(x => x.AccountType)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.FeeId == feeId);
        }

        // Retrieves fees matching a particular name.
        // Useful for locating bank charges by their business name.
        public async Task<IEnumerable<Fee>> GetByNameAsync(string name)
        {
            return await _dbSet
                .Where(x => x.Name.Contains(name))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 2. FEE CLASSIFICATION
        // =========================================================

        // Retrieves fees using their calculation method.
        // Useful for separating flat, percentage-based and slab-based charges.
        public async Task<IEnumerable<Fee>> GetByCalculationMethodAsync(string calculationMethod)
        {
            return await _dbSet
                .Where(x => x.CalculationMethod == calculationMethod)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves fees using their charging frequency.
        // Useful for identifying per-transaction, monthly, quarterly, yearly and one-time charges.
        public async Task<IEnumerable<Fee>> GetByFrequencyAsync(string frequency)
        {
            return await _dbSet
                .Where(x => x.Frequency == frequency)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves fees associated with a transaction type.
        // Useful for applying charges to transfers and other banking transactions.
        public async Task<IEnumerable<Fee>> GetByTransactionTypeIdAsync(int transactionTypeId)
        {
            return await _dbSet
                .Where(x => x.TransactionTypeId == transactionTypeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves fees associated with a payment method.
        // Useful for identifying charges applicable to a particular payment channel.
        public async Task<IEnumerable<Fee>> GetByPaymentMethodIdAsync(int paymentMethodId)
        {
            return await _dbSet
                .Where(x => x.PaymentMethodId == paymentMethodId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves fees associated with an account type.
        // Useful for applying account-specific banking charges.
        public async Task<IEnumerable<Fee>> GetByAccountTypeIdAsync(int accountTypeId)
        {
            return await _dbSet
                .Where(x => x.AccountTypeId == accountTypeId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 3. FEE EFFECTIVE DATE MANAGEMENT
        // =========================================================

        // Retrieves fees active within the supplied business date.
        // Useful for selecting fee configurations valid when a transaction is processed.
        public async Task<IEnumerable<Fee>> GetEffectiveFeesAsync(DateTime effectiveDate)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            x.EffectiveFrom <= effectiveDate &&
                            (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveDate))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves fees that will become effective within the supplied date range.
        // Useful for planning upcoming changes to bank charges.
        public async Task<IEnumerable<Fee>> GetUpcomingFeesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.EffectiveFrom >= startDate && x.EffectiveFrom <= endDate)
                .OrderBy(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves active fee configurations.
        // Useful for displaying fee definitions available for operational use.
        public async Task<IEnumerable<Fee>> GetActiveFeesAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves inactive fee configurations.
        // Useful for reviewing discontinued or temporarily disabled bank charges.
        public async Task<IEnumerable<Fee>> GetInactiveFeesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 4. TAX AND AMOUNT MANAGEMENT
        // =========================================================

        // Retrieves fees for which GST is applicable.
        // Useful for identifying charges that require tax calculation.
        public async Task<IEnumerable<Fee>> GetGstApplicableFeesAsync()
        {
            return await _dbSet
                .Where(x => x.IsGstApplicable)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves fees within the supplied minimum and maximum amount range.
        // Useful for reviewing fixed or percentage-based fee limits.
        public async Task<IEnumerable<Fee>> GetFeesWithinAmountRangeAsync(decimal minimumAmount, decimal maximumAmount)
        {
            return await _dbSet
                .Where(x => x.FlatAmount.HasValue &&
                            x.FlatAmount.Value >= minimumAmount &&
                            x.FlatAmount.Value <= maximumAmount)
                .OrderBy(x => x.FlatAmount)
                .ToListAsync();
        }

        // =========================================================
        // 5. TRANSACTION AND VALIDATION
        // =========================================================

        // Retrieves fees that have been used by at least one transaction.
        // Useful for reviewing fee usage and financial reporting.
        public async Task<IEnumerable<Fee>> GetFeesWithChargedTransactionsAsync()
        {
            return await _dbSet
                .Include(x => x.ChargedTransactions)
                .Where(x => x.ChargedTransactions.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Checks whether a fee code already exists.
        // Useful for preventing duplicate fee configuration records.
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _dbSet.AnyAsync(x => x.Code == code);
        }

        // Retrieves fees created by a particular user.
        // Useful for tracking fee configuration ownership and administration.
        public async Task<IEnumerable<Fee>> GetFeesCreatedByUserAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Searches fees by code or name.
        // Useful for locating fee configurations in administrative screens.
        public async Task<IEnumerable<Fee>> SearchFeesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.Code.Contains(searchTerm) || x.Name.Contains(searchTerm))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
