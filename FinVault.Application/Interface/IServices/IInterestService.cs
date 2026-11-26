using FinVault.Application.DTOs.InterestCalculationDtos;
using FinVault.Application.DTOs.InterestRateDtos;
using Microsoft.VisualBasic;

namespace FinVault.Application.Interfaces.Services
{
    public interface IInterestService
    {
        // 1. INTEREST RATE IDENTIFICATION AND CRUD
        Task<IEnumerable<InterestRateDto>> GetAllRatesAsync();
        Task<InterestRateDto?> GetRateByIdAsync(int interestRateId);
        Task<InterestRateDto?> GetRateDetailsAsync(int interestRateId);
        Task<IEnumerable<InterestRateDto>> GetRatesByNameAsync(string rateName);
        Task<InterestRateDto> CreateRateAsync(CreateInterestRateDto dto);
        Task<InterestRateDto?> UpdateRateAsync(UpdateInterestRateDto dto);
        Task<bool> DeleteRateAsync(DeleteInterestRateDto dto);

        // 2. PRODUCT CATEGORY AND TYPE
        Task<IEnumerable<InterestRateDto>> GetRatesByProductCategoryAsync(string productCategory);
        Task<IEnumerable<InterestRateDto>> GetRatesByLoanTypeAsync(string loanType);
        Task<IEnumerable<InterestRateDto>> GetRatesByAccountTypeIdAsync(int accountTypeId);
        Task<IEnumerable<InterestRateDto>> GetRatesByCompoundingFrequencyAsync(string compoundingFrequency);

        // 3. RATE EFFECTIVE DATE MANAGEMENT
        Task<IEnumerable<InterestRateDto>> GetEffectiveRatesAsync(DateTime effectiveDate);
        Task<IEnumerable<InterestRateDto>> GetUpcomingRatesAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InterestRateDto>> GetActiveRatesAsync();
        Task<IEnumerable<InterestRateDto>> GetInactiveRatesAsync();
        Task<IEnumerable<InterestRateDto>> GetExpiredRatesAsync(DateTime currentDate);

        // 4. AMOUNT AND TENURE APPLICABILITY
        Task<IEnumerable<InterestRateDto>> GetRatesByAmountAsync(decimal amount, DateTime effectiveDate);
        Task<IEnumerable<InterestRateDto>> GetRatesByTenureAsync(int tenureMonths, DateTime effectiveDate);
        Task<IEnumerable<InterestRateDto>> GetApplicableRatesAsync(
            string productCategory,
            decimal amount,
            int tenureMonths,
            DateTime effectiveDate);

        // 5. SPECIAL RATE CONFIGURATIONS
        Task<IEnumerable<InterestRateDto>> GetSeniorCitizenRatesAsync(DateTime effectiveDate);
        Task<IEnumerable<InterestRateDto>> GetBenchmarkLinkedRatesAsync();
        Task<IEnumerable<InterestRateDto>> GetRatesWithLoansAsync();
        Task<IEnumerable<InterestRateDto>> GetRatesWithInterestCalculationsAsync();
        Task<IEnumerable<InterestRateDto>> GetRatesCreatedByUserAsync(int userId);
        Task<IEnumerable<InterestRateDto>> SearchInterestRatesAsync(string searchTerm);

        // 6. INTEREST CALCULATION IDENTIFICATION
        Task<IEnumerable<InterestCalculationDto>> GetAllCalculationsAsync();
        Task<InterestCalculationDto?> GetCalculationByIdAsync(int interestCalculationId);
        Task<InterestCalculationDto?> GetCalculationDetailsAsync(int interestCalculationId);
        Task<InterestCalculationDto> CreateCalculationAsync(CreateInterestCalculationDto dto);
        Task<InterestCalculationDto?> UpdateCalculationAsync(UpdateInterestCalculationDto dto);
        Task<bool> DeleteCalculationAsync(DeleteInterestCalculationDto dto);

        // 7. ACCOUNT, LOAN AND RATE CALCULATIONS
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsByAccountIdAsync(int accountId);
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsByLoanIdAsync(int loanId);
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsByInterestRateIdAsync(int interestRateId);
        Task<IEnumerable<InterestCalculationDto>> GetLatestCalculationsByAccountIdAsync(int accountId, int count);
        Task<IEnumerable<InterestCalculationDto>> GetLatestCalculationsByLoanIdAsync(int loanId, int count);

        // 8. CALCULATION TYPE AND STATUS
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsByTypeAsync(string calculationType);
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsByStatusAsync(string status);
        Task<IEnumerable<InterestCalculationDto>> GetPendingCalculationsAsync();
        Task<IEnumerable<InterestCalculationDto>> GetPostedCalculationsAsync();
        Task<IEnumerable<InterestCalculationDto>> GetReversedCalculationsAsync();

        // 9. ACCOUNTING POSTING AND PERIOD MANAGEMENT
        Task<InterestCalculationDto?> GetCalculationByJournalEntryIdAsync(int journalEntryId);
        Task<InterestCalculationDto?> GetCalculationByTransactionIdAsync(int transactionId);
        Task<IEnumerable<InterestCalculationDto>> GetPostedCalculationsWithoutJournalEntryAsync();
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsByPeriodRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsWithinDateRangeAsync(DateTime startDate, DateTime endDate);

        // 10. AMOUNT, TAX AND REPORTING
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsByInterestAmountRangeAsync(decimal minimumAmount, decimal maximumAmount);
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsWithTdsAsync();
        Task<decimal> GetTotalInterestByAccountIdAsync(int accountId);
        Task<decimal> GetTotalInterestByLoanIdAsync(int loanId);
        Task<IEnumerable<InterestCalculationDto>> GetCalculationsCreatedByUserIdAsync(int userId);

        // 11. VALIDATION
        Task<bool> ExistsForPeriodAsync(
            int? accountId,
            int? loanId,
            DateTime periodStart,
            DateTime periodEnd,
            string calculationType);
    }
}



//Banking rules:
//-Select rates using product category, amount, tenure and effective date.
//- Prevent duplicate calculations for the same account/loan, period and calculation type.
//- Preserve the applied rate and principal base used in each calculation.
//- A posted calculation must have its accounting journal entry.
//- Correct posted calculations through reversal; do not silently overwrite financial history.
//-Apply applicable TDS rules before posting interest.