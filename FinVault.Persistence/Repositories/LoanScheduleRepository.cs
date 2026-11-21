
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class LoanScheduleRepository : GenericRepository<LoanSchedule>, ILoanScheduleRepository
    {
        public LoanScheduleRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. SCHEDULE IDENTIFICATION
        // =========================================================

        // Retrieves a schedule by its loan and installment number.
        // Useful for locating a specific EMI without exposing internal database identifiers.
        public async Task<LoanSchedule?> GetByLoanAndInstallmentAsync(int loanId, int installmentNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.LoanId == loanId && x.InstallmentNumber == installmentNumber);
        }

        // Retrieves complete schedule details with its loan and repayment records.
        // Useful for investigating an installment and its associated repayment history.
        public async Task<LoanSchedule?> GetScheduleDetailsAsync(int loanScheduleId)
        {
            return await _dbSet
                .Include(x => x.Loan)
                .Include(x => x.LoanRepayments)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.LoanScheduleId == loanScheduleId);
        }

        // Checks whether an installment number already exists for a loan.
        // Useful for preventing duplicate installments in a loan schedule.
        public async Task<bool> ExistsByLoanAndInstallmentAsync(int loanId, int installmentNumber)
        {
            return await _dbSet.AnyAsync(x => x.LoanId == loanId && x.InstallmentNumber == installmentNumber);
        }

        // =========================================================
        // 2. LOAN SCHEDULE HISTORY
        // =========================================================

        // Retrieves all installments for a particular loan in installment order.
        // Useful for displaying the complete repayment schedule to customers and bank staff.
        public async Task<IEnumerable<LoanSchedule>> GetByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId)
                .OrderBy(x => x.InstallmentNumber)
                .ToListAsync();
        }

        // Retrieves installments with their repayment records for a particular loan.
        // Useful for reconciling scheduled dues against actual repayment records.
        public async Task<IEnumerable<LoanSchedule>> GetLoanSchedulesWithRepaymentsAsync(int loanId)
        {
            return await _dbSet
                .Include(x => x.LoanRepayments)
                .Where(x => x.LoanId == loanId)
                .OrderBy(x => x.InstallmentNumber)
                .ToListAsync();
        }

        // Retrieves a particular installment with its repayment records.
        // Useful for investigating partial payments, full payments and reversals.
        public async Task<LoanSchedule?> GetScheduleWithRepaymentsAsync(int loanScheduleId)
        {
            return await _dbSet
                .Include(x => x.LoanRepayments)
                .FirstOrDefaultAsync(x => x.LoanScheduleId == loanScheduleId);
        }

        // =========================================================
        // 3. SCHEDULE STATUS
        // =========================================================

        // Retrieves installments matching a particular status.
        // Useful for monitoring pending, partially paid, paid, overdue and waived installments.
        public async Task<IEnumerable<LoanSchedule>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves pending installments for a particular loan.
        // Useful for identifying unpaid installments that have not yet been classified as overdue.
        public async Task<IEnumerable<LoanSchedule>> GetPendingSchedulesByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Pending")
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves partially paid installments for a particular loan.
        // Useful for tracking remaining dues after partial customer payments.
        public async Task<IEnumerable<LoanSchedule>> GetPartiallyPaidSchedulesByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "PartiallyPaid")
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves paid installments for a particular loan.
        // Useful for reviewing completed EMI obligations.
        public async Task<IEnumerable<LoanSchedule>> GetPaidSchedulesByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Paid")
                .OrderBy(x => x.InstallmentNumber)
                .ToListAsync();
        }

        // Retrieves overdue installments for a particular loan.
        // Useful for monitoring delinquent installments and collection follow-up.
        public async Task<IEnumerable<LoanSchedule>> GetOverdueSchedulesByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Overdue")
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves waived installments for a particular loan.
        // Useful for reviewing installments where dues were formally waived.
        public async Task<IEnumerable<LoanSchedule>> GetWaivedSchedulesByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status == "Waived")
                .OrderBy(x => x.InstallmentNumber)
                .ToListAsync();
        }

        // =========================================================
        // 4. DUE DATE MONITORING
        // =========================================================

        // Retrieves installments due within the supplied date range.
        // Useful for planning upcoming EMI collection activity.
        public async Task<IEnumerable<LoanSchedule>> GetSchedulesDueWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.DueDate >= startDate.Date && x.DueDate <= endDate.Date)
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves unpaid or partially paid installments due before the supplied date.
        // Useful for identifying installments that may require overdue assessment.
        public async Task<IEnumerable<LoanSchedule>> GetUnpaidSchedulesDueBeforeAsync(DateTime date)
        {
            return await _dbSet
                .Where(x => x.DueDate < date.Date &&
                            (x.Status == "Pending" || x.Status == "PartiallyPaid" || x.Status == "Overdue"))
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves installments for a loan due on a particular date.
        // Useful for daily EMI collection monitoring.
        public async Task<IEnumerable<LoanSchedule>> GetSchedulesDueOnDateAsync(int loanId, DateTime dueDate)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId &&
                            x.DueDate == dueDate.Date &&
                            (x.Status == "Pending" || x.Status == "PartiallyPaid" || x.Status == "Overdue"))
                .OrderBy(x => x.InstallmentNumber)
                .ToListAsync();
        }

        // Retrieves installments marked overdue across all loans.
        // Useful for centralized delinquency monitoring and collection work queues.
        public async Task<IEnumerable<LoanSchedule>> GetAllOverdueSchedulesAsync()
        {
            return await _dbSet
                .Include(x => x.Loan)
                .Where(x => x.Status == "Overdue")
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // =========================================================
        // 5. PAYMENT PROGRESS
        // =========================================================

        // Retrieves installments paid within the supplied date range.
        // Useful for reviewing completed installment collections by payment date.
        public async Task<IEnumerable<LoanSchedule>> GetSchedulesPaidWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.Status == "Paid" &&
                            x.PaidOn.HasValue &&
                            x.PaidOn.Value >= startDate.Date &&
                            x.PaidOn.Value <= endDate.Date)
                .OrderByDescending(x => x.PaidOn)
                .ToListAsync();
        }

        // Retrieves installments whose remaining amount is greater than the supplied threshold.
        // Useful for prioritizing installments with significant unpaid balances.
        public async Task<IEnumerable<LoanSchedule>> GetSchedulesWithOutstandingAmountAboveAsync(decimal minimumAmount)
        {
            return await _dbSet
                .Where(x => x.Status != "Waived" &&
                            (x.PrincipalDue - x.PrincipalPaid +
                             x.InterestDue - x.InterestPaid +
                             x.PenaltyAccrued - x.PenaltyPaid) > minimumAmount)
                .OrderByDescending(x => (x.PrincipalDue - x.PrincipalPaid +
                                         x.InterestDue - x.InterestPaid +
                                         x.PenaltyAccrued - x.PenaltyPaid))
                .ToListAsync();
        }

        // =========================================================
        // 6. FINANCIAL TOTALS
        // =========================================================

        // Calculates the total principal due for a particular loan.
        // Useful for reviewing the principal obligation across its repayment schedule.
        public async Task<decimal> GetTotalPrincipalDueByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status != "Waived")
                .SumAsync(x => (decimal?)x.PrincipalDue) ?? 0m;
        }

        // Calculates the total interest due for a particular loan.
        // Useful for reviewing the scheduled interest obligation.
        public async Task<decimal> GetTotalInterestDueByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status != "Waived")
                .SumAsync(x => (decimal?)x.InterestDue) ?? 0m;
        }

        // Calculates the total principal paid for a particular loan.
        // Useful for tracking principal recovery through scheduled installments.
        public async Task<decimal> GetTotalPrincipalPaidByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId)
                .SumAsync(x => (decimal?)x.PrincipalPaid) ?? 0m;
        }

        // Calculates the total interest paid for a particular loan.
        // Useful for tracking interest collections through scheduled installments.
        public async Task<decimal> GetTotalInterestPaidByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId)
                .SumAsync(x => (decimal?)x.InterestPaid) ?? 0m;
        }

        // Calculates the total penalty accrued for a particular loan.
        // Useful for reviewing penalties assessed against scheduled installments.
        public async Task<decimal> GetTotalPenaltyAccruedByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId && x.Status != "Waived")
                .SumAsync(x => (decimal?)x.PenaltyAccrued) ?? 0m;
        }

        // Calculates the total penalty paid for a particular loan.
        // Useful for tracking penalty collections separately from principal and interest.
        public async Task<decimal> GetTotalPenaltyPaidByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId)
                .SumAsync(x => (decimal?)x.PenaltyPaid) ?? 0m;
        }

        // =========================================================
        // 7. SCHEDULE COUNTS
        // =========================================================

        // Counts installments for a particular loan matching the supplied status.
        // Useful for tracking repayment progress and outstanding installment counts.
        public async Task<int> CountSchedulesByLoanAndStatusAsync(int loanId, string status)
        {
            return await _dbSet.CountAsync(x => x.LoanId == loanId && x.Status == status);
        }

        // Counts installments remaining unpaid or partially paid for a particular loan.
        // Useful for monitoring the number of outstanding repayment obligations.
        public async Task<int> CountUnpaidSchedulesByLoanIdAsync(int loanId)
        {
            return await _dbSet.CountAsync(x =>
                x.LoanId == loanId &&
                (x.Status == "Pending" || x.Status == "PartiallyPaid" || x.Status == "Overdue"));
        }
    }
}
