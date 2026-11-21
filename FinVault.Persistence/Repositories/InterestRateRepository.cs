
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class InterestRateRepository : GenericRepository<InterestRate>, IInterestRateRepository
    {
        public InterestRateRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. RATE IDENTIFICATION
        // =========================================================

        // Retrieves an interest rate using its database identifier.
        // Useful for selecting a specific rate configuration during account or loan processing.
        public async Task<InterestRate?> GetInterestRateDetailsAsync(int interestRateId)
        {
            return await _dbSet
                .Include(x => x.AccountType)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.Loans)
                .Include(x => x.InterestCalculations)
                .FirstOrDefaultAsync(x => x.InterestRateId == interestRateId);
        }

        // Retrieves interest rates matching a particular name.
        // Useful for locating a bank's published interest rate configuration.
        public async Task<IEnumerable<InterestRate>> GetByRateNameAsync(string rateName)
        {
            return await _dbSet
                .Where(x => x.RateName.Contains(rateName))
                .OrderBy(x => x.RateName)
                .ToListAsync();
        }

        // =========================================================
        // 2. PRODUCT CATEGORY AND TYPE
        // =========================================================

        // Retrieves rates belonging to a particular product category.
        // Useful for separating savings, current, fixed deposit, recurring deposit and loan rates.
        public async Task<IEnumerable<InterestRate>> GetByProductCategoryAsync(string productCategory)
        {
            return await _dbSet
                .Where(x => x.ProductCategory == productCategory)
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves loan rates configured for a particular loan type.
        // Useful for selecting applicable rates for home, personal or other loan products.
        public async Task<IEnumerable<InterestRate>> GetByLoanTypeAsync(string loanType)
        {
            return await _dbSet
                .Where(x => x.ProductCategory == "Loan" && x.LoanType == loanType)
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves rates associated with a particular account type.
        // Useful for identifying interest rates applicable to a specific deposit product.
        public async Task<IEnumerable<InterestRate>> GetByAccountTypeIdAsync(int accountTypeId)
        {
            return await _dbSet
                .Where(x => x.AccountTypeId == accountTypeId)
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves rates using a particular compounding frequency.
        // Useful for reviewing simple, daily, monthly, quarterly, half-yearly and yearly compounding configurations.
        public async Task<IEnumerable<InterestRate>> GetByCompoundingFrequencyAsync(string compoundingFrequency)
        {
            return await _dbSet
                .Where(x => x.CompoundingFrequency == compoundingFrequency)
                .OrderBy(x => x.RateName)
                .ToListAsync();
        }

        // =========================================================
        // 3. RATE EFFECTIVE DATE MANAGEMENT
        // =========================================================

        // Retrieves active rates effective on the supplied date.
        // Useful for selecting the rate valid when an account or loan interest period begins.
        public async Task<IEnumerable<InterestRate>> GetEffectiveRatesAsync(DateTime effectiveDate)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            x.EffectiveFrom <= effectiveDate &&
                            (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveDate))
                .OrderBy(x => x.ProductCategory)
                .ThenByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves rates scheduled to become effective within the supplied date range.
        // Useful for reviewing upcoming interest rate changes.
        public async Task<IEnumerable<InterestRate>> GetUpcomingRatesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.EffectiveFrom >= startDate && x.EffectiveFrom <= endDate)
                .OrderBy(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves active interest rate configurations.
        // Useful for displaying currently enabled rate definitions.
        public async Task<IEnumerable<InterestRate>> GetActiveRatesAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.ProductCategory)
                .ThenBy(x => x.RateName)
                .ToListAsync();
        }

        // Retrieves inactive interest rate configurations.
        // Useful for reviewing discontinued or disabled rates.
        public async Task<IEnumerable<InterestRate>> GetInactiveRatesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves rates that are no longer effective.
        // Useful for historical rate administration and reporting.
        public async Task<IEnumerable<InterestRate>> GetExpiredRatesAsync(DateTime currentDate)
        {
            return await _dbSet
                .Where(x => x.EffectiveTo.HasValue && x.EffectiveTo.Value < currentDate)
                .OrderByDescending(x => x.EffectiveTo)
                .ToListAsync();
        }

        // =========================================================
        // 4. AMOUNT AND TENURE APPLICABILITY
        // =========================================================

        // Retrieves rates applicable to the supplied principal amount.
        // Useful for finding rate configurations whose amount limits include a deposit or loan principal.
        public async Task<IEnumerable<InterestRate>> GetRatesByAmountAsync(decimal amount, DateTime effectiveDate)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            x.EffectiveFrom <= effectiveDate &&
                            (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveDate) &&
                            (!x.MinAmount.HasValue || x.MinAmount.Value <= amount) &&
                            (!x.MaxAmount.HasValue || x.MaxAmount.Value >= amount))
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves rates applicable to the supplied tenure.
        // Useful for selecting deposit or loan rates based on the product duration.
        public async Task<IEnumerable<InterestRate>> GetRatesByTenureAsync(int tenureMonths, DateTime effectiveDate)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            x.EffectiveFrom <= effectiveDate &&
                            (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveDate) &&
                            (!x.MinTenureMonths.HasValue || x.MinTenureMonths.Value <= tenureMonths) &&
                            (!x.MaxTenureMonths.HasValue || x.MaxTenureMonths.Value >= tenureMonths))
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // Retrieves rates matching both principal amount and tenure.
        // Useful for finding rate configurations applicable to a specific banking product request.
        public async Task<IEnumerable<InterestRate>> GetApplicableRatesAsync(string productCategory, decimal amount, int tenureMonths, DateTime effectiveDate)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            x.ProductCategory == productCategory &&
                            x.EffectiveFrom <= effectiveDate &&
                            (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveDate) &&
                            (!x.MinAmount.HasValue || x.MinAmount.Value <= amount) &&
                            (!x.MaxAmount.HasValue || x.MaxAmount.Value >= amount) &&
                            (!x.MinTenureMonths.HasValue || x.MinTenureMonths.Value <= tenureMonths) &&
                            (!x.MaxTenureMonths.HasValue || x.MaxTenureMonths.Value >= tenureMonths))
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // =========================================================
        // 5. SPECIAL RATE CONFIGURATIONS
        // =========================================================

        // Retrieves rates that include an additional senior-citizen interest rate.
        // Useful for identifying deposit products with senior-citizen rate benefits.
        public async Task<IEnumerable<InterestRate>> GetSeniorCitizenRatesAsync(DateTime effectiveDate)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            x.ProductCategory != "Loan" &&
                            x.SeniorCitizenAdditionalRate.HasValue &&
                            x.SeniorCitizenAdditionalRate.Value > 0 &&
                            x.EffectiveFrom <= effectiveDate &&
                            (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveDate))
                .OrderBy(x => x.RateName)
                .ToListAsync();
        }

        // Retrieves rates linked to an external benchmark.
        // Useful for reviewing floating-rate loan configurations.
        public async Task<IEnumerable<InterestRate>> GetBenchmarkLinkedRatesAsync()
        {
            return await _dbSet
                .Where(x => x.ProductCategory == "Loan" &&
                            x.BenchmarkName != null &&
                            x.SpreadOverBenchmark.HasValue)
                .OrderBy(x => x.BenchmarkName)
                .ToListAsync();
        }

        // =========================================================
        // 6. RELATED BANKING RECORDS
        // =========================================================

        // Retrieves rates referenced by at least one loan.
        // Useful for reviewing rate configurations currently used by loan accounts.
        public async Task<IEnumerable<InterestRate>> GetRatesWithLoansAsync()
        {
            return await _dbSet
                .Include(x => x.Loans)
                .Where(x => x.Loans.Any())
                .OrderBy(x => x.RateName)
                .ToListAsync();
        }

        // Retrieves rates referenced by at least one interest calculation.
        // Useful for tracing rate configurations used in historical interest calculations.
        public async Task<IEnumerable<InterestRate>> GetRatesWithInterestCalculationsAsync()
        {
            return await _dbSet
                .Include(x => x.InterestCalculations)
                .Where(x => x.InterestCalculations.Any())
                .OrderBy(x => x.RateName)
                .ToListAsync();
        }

        // =========================================================
        // 7. AUDIT AND SEARCH
        // =========================================================

        // Retrieves rates created by a particular user.
        // Useful for tracking who introduced interest rate configurations.
        public async Task<IEnumerable<InterestRate>> GetRatesCreatedByUserAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Searches interest rates by name, product category, loan type or benchmark.
        // Useful for locating rate configurations in banking administration screens.
        public async Task<IEnumerable<InterestRate>> SearchInterestRatesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.RateName.Contains(searchTerm) ||
                            x.ProductCategory.Contains(searchTerm) ||
                            (x.LoanType != null && x.LoanType.Contains(searchTerm)) ||
                            (x.BenchmarkName != null && x.BenchmarkName.Contains(searchTerm)))
                .OrderBy(x => x.ProductCategory)
                .ThenBy(x => x.RateName)
                .ToListAsync();
        }
    }
}
