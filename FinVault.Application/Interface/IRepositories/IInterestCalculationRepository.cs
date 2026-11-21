
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IInterestCalculationRepository : IGenericRepository<InterestCalculation>
    {
        // =========================================================
        // 1. CALCULATION IDENTIFICATION
        // =========================================================

        // Retrieves a calculation with its account, loan, interest rate and posting records.
        // Useful for reviewing the complete interest calculation and its accounting impact.
        Task<InterestCalculation?> GetCalculationDetailsAsync(int interestCalculationId);

        // Retrieves calculations associated with a particular account.
        // Useful for reviewing deposit interest accruals, payouts and capitalisations.
        Task<IEnumerable<InterestCalculation>> GetByAccountIdAsync(int accountId);

        // Retrieves calculations associated with a particular loan.
        // Useful for reviewing loan interest accruals and postings.
        Task<IEnumerable<InterestCalculation>> GetByLoanIdAsync(int loanId);

        // Retrieves calculations using a particular interest rate configuration.
        // Useful for auditing which rate configuration was used for interest calculations.
        Task<IEnumerable<InterestCalculation>> GetByInterestRateIdAsync(int interestRateId);

        // =========================================================
        // 2. CALCULATION TYPE AND STATUS
        // =========================================================

        // Retrieves calculations matching a particular calculation type.
        // Useful for separating accrual, payout and capitalisation operations.
        Task<IEnumerable<InterestCalculation>> GetByCalculationTypeAsync(string calculationType);

        // Retrieves calculations matching a particular processing status.
        // Useful for monitoring calculated, posted and reversed interest records.
        Task<IEnumerable<InterestCalculation>> GetByStatusAsync(string status);

        // Retrieves calculations that have been calculated but not yet posted.
        // Useful for identifying interest amounts awaiting accounting approval or posting.
        Task<IEnumerable<InterestCalculation>> GetPendingCalculationsAsync();

        // Retrieves calculations that have been posted to accounting.
        // Useful for reviewing completed interest postings.
        Task<IEnumerable<InterestCalculation>> GetPostedCalculationsAsync();

        // Retrieves calculations that have been reversed.
        // Useful for reviewing corrections to previously recorded interest calculations.
        Task<IEnumerable<InterestCalculation>> GetReversedCalculationsAsync();

        // =========================================================
        // 3. ACCOUNTING POSTING
        // =========================================================

        // Retrieves a calculation using its associated journal entry.
        // Useful for tracing an interest calculation to its accounting entry.
        Task<InterestCalculation?> GetByJournalEntryIdAsync(int journalEntryId);

        // Retrieves a calculation using its associated banking transaction.
        // Useful for tracing an interest payout or capitalisation to its transaction.
        Task<InterestCalculation?> GetByTransactionIdAsync(int transactionId);

        // Retrieves posted calculations that have no associated journal entry.
        // Useful for identifying records that may require accounting reconciliation.
        Task<IEnumerable<InterestCalculation>> GetPostedCalculationsWithoutJournalEntryAsync();

        // =========================================================
        // 4. PERIOD AND DATE MANAGEMENT
        // =========================================================

        // Retrieves calculations whose interest periods overlap the supplied date range.
        // Useful for reviewing interest calculations covering a reporting or accounting period.
        Task<IEnumerable<InterestCalculation>> GetByPeriodRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves calculations created within the supplied date range.
        // Useful for monitoring calculation processing activity.
        Task<IEnumerable<InterestCalculation>> GetCalculatedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves the most recent calculations for a particular account.
        // Useful for reviewing the latest interest activity on a deposit account.
        Task<IEnumerable<InterestCalculation>> GetLatestCalculationsByAccountIdAsync(int accountId, int count);

        // Retrieves the most recent calculations for a particular loan.
        // Useful for reviewing the latest interest activity on a loan.
        Task<IEnumerable<InterestCalculation>> GetLatestCalculationsByLoanIdAsync(int loanId, int count);

        // =========================================================
        // 5. AMOUNT AND TAX REPORTING
        // =========================================================

        // Retrieves calculations with interest amounts within the supplied range.
        // Useful for reviewing unusually high or low interest calculations.
        Task<IEnumerable<InterestCalculation>> GetByInterestAmountRangeAsync(decimal minimumAmount, decimal maximumAmount);

        // Retrieves calculations that contain a positive TDS amount.
        // Useful for reviewing interest calculations involving tax deduction.
        Task<IEnumerable<InterestCalculation>> GetCalculationsWithTdsAsync();

        // Calculates the total interest amount for a particular account.
        // Useful for account-level interest reporting and reconciliation.
        Task<decimal> GetTotalInterestByAccountIdAsync(int accountId);

        // Calculates the total interest amount for a particular loan.
        // Useful for loan interest reporting and reconciliation.
        Task<decimal> GetTotalInterestByLoanIdAsync(int loanId);

        // =========================================================
        // 6. AUDIT AND VALIDATION
        // =========================================================

        // Retrieves calculations created by a particular user.
        // Useful for tracking who initiated interest calculation records.
        Task<IEnumerable<InterestCalculation>> GetByCreatedByUserIdAsync(int userId);

        // Checks whether a calculation already exists for the subject, period and calculation type.
        // Useful for preventing duplicate interest calculations before insertion.
        Task<bool> ExistsForPeriodAsync(int? accountId, int? loanId, DateTime periodStart, DateTime periodEnd, string calculationType);
    }
}
