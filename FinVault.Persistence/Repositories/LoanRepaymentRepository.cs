
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class LoanRepaymentRepository : GenericRepository<LoanRepayment>, ILoanRepaymentRepository
    {
        public LoanRepaymentRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. REPAYMENT IDENTIFICATION
        // =========================================================

        // Retrieves a repayment using its unique reference.
        // Useful for tracing an individual repayment during customer support and reconciliation.
        public async Task<LoanRepayment?> GetByRepaymentReferenceAsync(string repaymentReference)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.RepaymentReference == repaymentReference);
        }

        // Retrieves complete repayment details with loan, schedule, transaction and audit users.
        // Useful for investigating a repayment and its linked financial transaction.
        public async Task<LoanRepayment?> GetRepaymentDetailsAsync(int loanRepaymentId)
        {
            return await _dbSet
                .Include(x => x.Loan)
                .Include(x => x.LoanSchedule)
                .Include(x => x.Transaction)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.LoanRepaymentId == loanRepaymentId);
        }

        // Checks whether a repayment reference already exists.
        // Useful for preventing duplicate repayment references.
        public async Task<bool> ExistsByRepaymentReferenceAsync(string repaymentReference)
        {
            return await _dbSet.AnyAsync(x => x.RepaymentReference == repaymentReference);
        }

        // =========================================================
        // 2. LOAN AND SCHEDULE HISTORY
        // =========================================================

        // Retrieves repayments belonging to a particular loan.
        // Useful for reviewing the customer's loan repayment history.
        public async Task<IEnumerable<LoanRepayment>> GetByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves repayments linked to a particular repayment schedule.
        // Useful for checking whether a scheduled EMI has been paid.
        public async Task<IEnumerable<LoanRepayment>> GetByLoanScheduleIdAsync(int loanScheduleId)
        {
            return await _dbSet
                .Where(x => x.LoanScheduleId == loanScheduleId)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves a repayment linked to a particular banking transaction.
        // Useful for tracing loan repayments to ledger-backed transactions.
        public async Task<LoanRepayment?> GetByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .Include(x => x.Loan)
                .FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        // Retrieves repayments with their linked transaction details for a loan.
        // Useful for reconciling loan repayment records against transaction records.
        public async Task<IEnumerable<LoanRepayment>> GetLoanRepaymentsWithTransactionsAsync(int loanId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .Where(x => x.LoanId == loanId)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. REPAYMENT STATUS
        // =========================================================

        // Retrieves repayments matching a particular status.
        // Useful for monitoring pending, successful, failed and reversed repayments.
        public async Task<IEnumerable<LoanRepayment>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves successful repayments for a particular loan.
        // Useful for calculating payments that have actually been completed.
        public async Task<IEnumerable<LoanRepayment>> GetSuccessfulRepaymentsByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Successful")
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves pending repayments.
        // Useful for monitoring payments awaiting processing or confirmation.
        public async Task<IEnumerable<LoanRepayment>> GetPendingRepaymentsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Pending")
                .OrderBy(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves failed repayments.
        // Useful for identifying payment failures requiring investigation or retry decisions.
        public async Task<IEnumerable<LoanRepayment>> GetFailedRepaymentsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Failed")
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves reversed repayments.
        // Useful for reviewing repayment reversals and their financial impact.
        public async Task<IEnumerable<LoanRepayment>> GetReversedRepaymentsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Reversed")
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. REPAYMENT TYPE AND PAYMENT MODE
        // =========================================================

        // Retrieves repayments matching a particular repayment type.
        // Useful for separating EMI, part-payment, foreclosure and penalty collections.
        public async Task<IEnumerable<LoanRepayment>> GetByRepaymentTypeAsync(string repaymentType)
        {
            return await _dbSet
                .Where(x => x.RepaymentType == repaymentType)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves repayments processed through a particular payment mode.
        // Useful for monitoring AutoDebit, NACH, online and branch collections.
        public async Task<IEnumerable<LoanRepayment>> GetByPaymentModeAsync(string paymentMode)
        {
            return await _dbSet
                .Where(x => x.PaymentMode == paymentMode)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. DATE-BASED REPAYMENT REPORTING
        // =========================================================

        // Retrieves repayments received within the supplied date range.
        // Useful for daily, monthly and period-based repayment reporting.
        public async Task<IEnumerable<LoanRepayment>> GetRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.PaidAt >= startDate && x.PaidAt <= endDate)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves successful repayments received within the supplied date range.
        // Useful for reporting confirmed repayment collections.
        public async Task<IEnumerable<LoanRepayment>> GetSuccessfulRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Successful" && x.PaidAt >= startDate && x.PaidAt <= endDate)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Retrieves repayments recorded after the supplied date.
        // Useful for reviewing recent loan collection activity.
        public async Task<IEnumerable<LoanRepayment>> GetRecentRepaymentsAsync(DateTime fromDate)
        {
            return await _dbSet
                .Where(x => x.PaidAt >= fromDate)
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // =========================================================
        // 6. FINANCIAL TOTALS
        // =========================================================

        // Calculates the total amount of successful repayments for a loan.
        // Useful for reviewing the amount collected against a loan.
        public async Task<decimal> GetTotalSuccessfulAmountByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Successful")
                .SumAsync(x => (decimal?)x.AmountPaid) ?? 0m;
        }

        // Calculates the successful principal component collected for a loan.
        // Useful for tracking principal recovery separately from interest and penalties.
        public async Task<decimal> GetTotalPrincipalCollectedByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Successful")
                .SumAsync(x => (decimal?)x.PrincipalComponent) ?? 0m;
        }

        // Calculates the successful interest component collected for a loan.
        // Useful for reviewing interest income collected from a loan.
        public async Task<decimal> GetTotalInterestCollectedByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Successful")
                .SumAsync(x => (decimal?)x.InterestComponent) ?? 0m;
        }

        // Calculates the successful penalty component collected for a loan.
        // Useful for reporting penalty collections separately from regular repayments.
        public async Task<decimal> GetTotalPenaltyCollectedByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Successful")
                .SumAsync(x => (decimal?)x.PenaltyComponent) ?? 0m;
        }

        // Calculates successful repayment collections within a date range.
        // Useful for measuring confirmed collection performance over a reporting period.
        public async Task<decimal> GetTotalSuccessfulCollectionsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Successful" && x.PaidAt >= startDate && x.PaidAt <= endDate)
                .SumAsync(x => (decimal?)x.AmountPaid) ?? 0m;
        }

        // =========================================================
        // 7. SEARCH AND COUNTS
        // =========================================================

        // Searches repayments by reference, failure reason or linked loan number.
        // Useful for locating repayment records during support and investigation.
        public async Task<IEnumerable<LoanRepayment>> SearchRepaymentsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(x => x.Loan)
                .Where(x => x.RepaymentReference.Contains(searchTerm) ||
                            (x.FailureReason != null && x.FailureReason.Contains(searchTerm)) ||
                            x.Loan.LoanNumber.Contains(searchTerm))
                .OrderByDescending(x => x.PaidAt)
                .ToListAsync();
        }

        // Counts successful repayments recorded for a particular loan.
        // Useful for monitoring completed repayment activity.
        public async Task<int> CountSuccessfulRepaymentsByLoanIdAsync(int loanId)
        {
            return await _dbSet.CountAsync(x => x.LoanId == loanId && x.Status == "Successful");
        }
    }
}
