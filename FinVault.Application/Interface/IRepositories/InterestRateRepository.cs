
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IInterestRateRepository : IGenericRepository<InterestRate>
    {
        // =========================================================
        // 1. RATE IDENTIFICATION
        // =========================================================

        // Retrieves an interest rate using its database identifier.
        // Useful for selecting a specific rate configuration during account or loan processing.
        Task<InterestRate?> GetInterestRateDetailsAsync(int interestRateId);

        // Retrieves interest rates matching a particular name.
        // Useful for locating a bank's published interest rate configuration.
        Task<IEnumerable<InterestRate>> GetByRateNameAsync(string rateName);

        // =========================================================
        // 2. PRODUCT CATEGORY AND TYPE
        // =========================================================

        // Retrieves rates belonging to a particular product category.
        // Useful for separating savings, current, fixed deposit, recurring deposit and loan rates.
        Task<IEnumerable<InterestRate>> GetByProductCategoryAsync(string productCategory);

        // Retrieves loan rates configured for a particular loan type.
        // Useful for selecting applicable rates for home, personal or other loan products.
        Task<IEnumerable<InterestRate>> GetByLoanTypeAsync(string loanType);

        // Retrieves rates associated with a particular account type.
        // Useful for identifying interest rates applicable to a specific deposit product.
        Task<IEnumerable<InterestRate>> GetByAccountTypeIdAsync(int accountTypeId);

        // Retrieves rates using a particular compounding frequency.
        // Useful for reviewing simple, daily, monthly, quarterly, half-yearly and yearly compounding configurations.
        Task<IEnumerable<InterestRate>> GetByCompoundingFrequencyAsync(string compoundingFrequency);

        // =========================================================
        // 3. RATE EFFECTIVE DATE MANAGEMENT
        // =========================================================

        // Retrieves active rates effective on the supplied date.
        // Useful for selecting the rate valid when an account or loan interest period begins.
        Task<IEnumerable<InterestRate>> GetEffectiveRatesAsync(DateTime effectiveDate);

        // Retrieves rates scheduled to become effective within the supplied date range.
        // Useful for reviewing upcoming interest rate changes.
        Task<IEnumerable<InterestRate>> GetUpcomingRatesAsync(DateTime startDate, DateTime endDate);

        // Retrieves active interest rate configurations.
        // Useful for displaying currently enabled rate definitions.
        Task<IEnumerable<InterestRate>> GetActiveRatesAsync();

        // Retrieves inactive interest rate configurations.
        // Useful for reviewing discontinued or disabled rates.
        Task<IEnumerable<InterestRate>> GetInactiveRatesAsync();

        // Retrieves rates that are no longer effective.
        // Useful for historical rate administration and reporting.
        Task<IEnumerable<InterestRate>> GetExpiredRatesAsync(DateTime currentDate);

        // =========================================================
        // 4. AMOUNT AND TENURE APPLICABILITY
        // =========================================================

        // Retrieves rates applicable to the supplied principal amount.
        // Useful for finding rate configurations whose amount limits include a deposit or loan principal.
        Task<IEnumerable<InterestRate>> GetRatesByAmountAsync(decimal amount, DateTime effectiveDate);

        // Retrieves rates applicable to the supplied tenure.
        // Useful for selecting deposit or loan rates based on the product duration.
        Task<IEnumerable<InterestRate>> GetRatesByTenureAsync(int tenureMonths, DateTime effectiveDate);

        // Retrieves rates matching both principal amount and tenure.
        // Useful for finding rate configurations applicable to a specific banking product request.
        Task<IEnumerable<InterestRate>> GetApplicableRatesAsync(string productCategory, decimal amount, int tenureMonths, DateTime effectiveDate);

        // =========================================================
        // 5. SPECIAL RATE CONFIGURATIONS
        // =========================================================

        // Retrieves rates that include an additional senior-citizen interest rate.
        // Useful for identifying deposit products with senior-citizen rate benefits.
        Task<IEnumerable<InterestRate>> GetSeniorCitizenRatesAsync(DateTime effectiveDate);

        // Retrieves rates linked to an external benchmark.
        // Useful for reviewing floating-rate loan configurations.
        Task<IEnumerable<InterestRate>> GetBenchmarkLinkedRatesAsync();

        // =========================================================
        // 6. RELATED BANKING RECORDS
        // =========================================================

        // Retrieves rates referenced by at least one loan.
        // Useful for reviewing rate configurations currently used by loan accounts.
        Task<IEnumerable<InterestRate>> GetRatesWithLoansAsync();

        // Retrieves rates referenced by at least one interest calculation.
        // Useful for tracing rate configurations used in historical interest calculations.
        Task<IEnumerable<InterestRate>> GetRatesWithInterestCalculationsAsync();

        // =========================================================
        // 7. AUDIT AND SEARCH
        // =========================================================

        // Retrieves rates created by a particular user.
        // Useful for tracking who introduced interest rate configurations.
        Task<IEnumerable<InterestRate>> GetRatesCreatedByUserAsync(int userId);

        // Searches interest rates by name, product category, loan type or benchmark.
        // Useful for locating rate configurations in banking administration screens.
        Task<IEnumerable<InterestRate>> SearchInterestRatesAsync(string searchTerm);
    }
}
