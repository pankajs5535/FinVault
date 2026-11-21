
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class InterestCalculationRepository : GenericRepository<InterestCalculation>, IInterestCalculationRepository
    {
        public InterestCalculationRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. CALCULATION IDENTIFICATION
        // =========================================================

        // Retrieves a calculation with its account, loan, interest rate and posting records.
        // Useful for reviewing the complete interest calculation and its accounting impact.
        public async Task<InterestCalculation?> GetCalculationDetailsAsync(int interestCalculationId)
        {
            return await _dbSet
                .Include(x => x.Account)
                .Include(x => x.Loan)
                .Include(x => x.InterestRate)
                .Include(x => x.JournalEntry)
                .Include(x => x.Transaction)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.InterestCalculationId == interestCalculationId);
        }

        // Retrieves calculations associated with a particular account.
        // Useful for reviewing deposit interest accruals, payouts and capitalisations.
        public async Task<IEnumerable<InterestCalculation>> GetByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Include(x => x.InterestRate)
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.PeriodEnd)
                .ToListAsync();
        }

        // Retrieves calculations associated with a particular loan.
        // Useful for reviewing loan interest accruals and postings.
        public async Task<IEnumerable<InterestCalculation>> GetByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Include(x => x.InterestRate)
                .Where(x => x.LoanId == loanId)
                .OrderByDescending(x => x.PeriodEnd)
                .ToListAsync();
        }

        // Retrieves calculations using a particular interest rate configuration.
        // Useful for auditing which rate configuration was used for interest calculations.
        public async Task<IEnumerable<InterestCalculation>> GetByInterestRateIdAsync(int interestRateId)
        {
            return await _dbSet
                .Where(x => x.InterestRateId == interestRateId)
                .OrderByDescending(x => x.CalculatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 2. CALCULATION TYPE AND STATUS
        // =========================================================

        // Retrieves calculations matching a particular calculation type.
        // Useful for separating accrual, payout and capitalisation operations.
        public async Task<IEnumerable<InterestCalculation>> GetByCalculationTypeAsync(string calculationType)
        {
            return await _dbSet
                .Where(x => x.CalculationType == calculationType)
                .OrderByDescending(x => x.PeriodEnd)
                .ToListAsync();
        }

        // Retrieves calculations matching a particular processing status.
        // Useful for monitoring calculated, posted and reversed interest records.
        public async Task<IEnumerable<InterestCalculation>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CalculatedAt)
                .ToListAsync();
        }

        // Retrieves calculations that have been calculated but not yet posted.
        // Useful for identifying interest amounts awaiting accounting approval or posting.
        public async Task<IEnumerable<InterestCalculation>> GetPendingCalculationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Calculated")
                .OrderBy(x => x.PeriodEnd)
                .ToListAsync();
        }

        // Retrieves calculations that have been posted to accounting.
        // Useful for reviewing completed interest postings.
        public async Task<IEnumerable<InterestCalculation>> GetPostedCalculationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Posted")
                .OrderByDescending(x => x.PeriodEnd)
                .ToListAsync();
        }

        // Retrieves calculations that have been reversed.
        // Useful for reviewing corrections to previously recorded interest calculations.
        public async Task<IEnumerable<InterestCalculation>> GetReversedCalculationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Reversed")
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. ACCOUNTING POSTING
        // =========================================================

        // Retrieves a calculation using its associated journal entry.
        // Useful for tracing an interest calculation to its accounting entry.
        public async Task<InterestCalculation?> GetByJournalEntryIdAsync(int journalEntryId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.JournalEntryId == journalEntryId);
        }

        // Retrieves a calculation using its associated banking transaction.
        // Useful for tracing an interest payout or capitalisation to its transaction.
        public async Task<InterestCalculation?> GetByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        // Retrieves posted calculations that have no associated journal entry.
        // Useful for identifying records that may require accounting reconciliation.
        public async Task<IEnumerable<InterestCalculation>> GetPostedCalculationsWithoutJournalEntryAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Posted" && x.JournalEntryId == null)
                .OrderBy(x => x.PeriodEnd)
                .ToListAsync();
        }

        // =========================================================
        // 4. PERIOD AND DATE MANAGEMENT
        // =========================================================

        // Retrieves calculations whose interest periods overlap the supplied date range.
        // Useful for reviewing interest calculations covering a reporting or accounting period.
        public async Task<IEnumerable<InterestCalculation>> GetByPeriodRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.PeriodStart <= endDate && x.PeriodEnd >= startDate)
                .OrderByDescending(x => x.PeriodEnd)
                .ToListAsync();
        }

        // Retrieves calculations created within the supplied date range.
        // Useful for monitoring calculation processing activity.
        public async Task<IEnumerable<InterestCalculation>> GetCalculatedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CalculatedAt >= startDate && x.CalculatedAt <= endDate)
                .OrderByDescending(x => x.CalculatedAt)
                .ToListAsync();
        }

        // Retrieves the most recent calculations for a particular account.
        // Useful for reviewing the latest interest activity on a deposit account.
        public async Task<IEnumerable<InterestCalculation>> GetLatestCalculationsByAccountIdAsync(int accountId, int count)
        {
            return await _dbSet
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.PeriodEnd)
                .Take(count)
                .ToListAsync();
        }

        // Retrieves the most recent calculations for a particular loan.
        // Useful for reviewing the latest interest activity on a loan.
        public async Task<IEnumerable<InterestCalculation>> GetLatestCalculationsByLoanIdAsync(int loanId, int count)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId)
                .OrderByDescending(x => x.PeriodEnd)
                .Take(count)
                .ToListAsync();
        }

        // =========================================================
        // 5. AMOUNT AND TAX REPORTING
        // =========================================================

        // Retrieves calculations with interest amounts within the supplied range.
        // Useful for reviewing unusually high or low interest calculations.
        public async Task<IEnumerable<InterestCalculation>> GetByInterestAmountRangeAsync(decimal minimumAmount, decimal maximumAmount)
        {
            return await _dbSet
                .Where(x => x.InterestAmount >= minimumAmount && x.InterestAmount <= maximumAmount)
                .OrderBy(x => x.InterestAmount)
                .ToListAsync();
        }

        // Retrieves calculations that contain a positive TDS amount.
        // Useful for reviewing interest calculations involving tax deduction.
        public async Task<IEnumerable<InterestCalculation>> GetCalculationsWithTdsAsync()
        {
            return await _dbSet
                .Where(x => x.TdsAmount > 0)
                .OrderByDescending(x => x.CalculatedAt)
                .ToListAsync();
        }

        // Calculates the total interest amount for a particular account.
        // Useful for account-level interest reporting and reconciliation.
        public async Task<decimal> GetTotalInterestByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.AccountId == accountId && x.Status == "Posted")
                .SumAsync(x => (decimal?)x.InterestAmount) ?? 0m;
        }

        // Calculates the total interest amount for a particular loan.
        // Useful for loan interest reporting and reconciliation.
        public async Task<decimal> GetTotalInterestByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Posted")
                .SumAsync(x => (decimal?)x.InterestAmount) ?? 0m;
        }

        // =========================================================
        // 6. AUDIT AND VALIDATION
        // =========================================================

        // Retrieves calculations created by a particular user.
        // Useful for tracking who initiated interest calculation records.
        public async Task<IEnumerable<InterestCalculation>> GetByCreatedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Checks whether a calculation already exists for the subject, period and calculation type.
        // Useful for preventing duplicate interest calculations before insertion.
        public async Task<bool> ExistsForPeriodAsync(int? accountId, int? loanId, DateTime periodStart, DateTime periodEnd, string calculationType)
        {
            return await _dbSet.AnyAsync(x =>
                x.AccountId == accountId &&
                x.LoanId == loanId &&
                x.PeriodStart == periodStart &&
                x.PeriodEnd == periodEnd &&
                x.CalculationType == calculationType &&
                x.Status != "Reversed");
        }
    }
}
