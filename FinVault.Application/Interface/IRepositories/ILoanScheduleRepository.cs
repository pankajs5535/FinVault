
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface ILoanScheduleRepository : IGenericRepository<LoanSchedule>
    {
        // =========================================================
        // 1. SCHEDULE IDENTIFICATION
        // =========================================================

        // Retrieves a schedule by its loan and installment number.
        // Useful for locating a specific EMI without exposing internal database identifiers.
        Task<LoanSchedule?> GetByLoanAndInstallmentAsync(int loanId, int installmentNumber);

        // Retrieves complete schedule details with its loan and repayment records.
        // Useful for investigating an installment and its associated repayment history.
        Task<LoanSchedule?> GetScheduleDetailsAsync(int loanScheduleId);

        // Checks whether an installment number already exists for a loan.
        // Useful for preventing duplicate installments in a loan schedule.
        Task<bool> ExistsByLoanAndInstallmentAsync(int loanId, int installmentNumber);

        // =========================================================
        // 2. LOAN SCHEDULE HISTORY
        // =========================================================

        // Retrieves all installments for a particular loan in installment order.
        // Useful for displaying the complete repayment schedule to customers and bank staff.
        Task<IEnumerable<LoanSchedule>> GetByLoanIdAsync(int loanId);

        // Retrieves installments with their repayment records for a particular loan.
        // Useful for reconciling scheduled dues against actual repayment records.
        Task<IEnumerable<LoanSchedule>> GetLoanSchedulesWithRepaymentsAsync(int loanId);

        // Retrieves a particular installment with its repayment records.
        // Useful for investigating partial payments, full payments and reversals.
        Task<LoanSchedule?> GetScheduleWithRepaymentsAsync(int loanScheduleId);

        // =========================================================
        // 3. SCHEDULE STATUS
        // =========================================================

        // Retrieves installments matching a particular status.
        // Useful for monitoring pending, partially paid, paid, overdue and waived installments.
        Task<IEnumerable<LoanSchedule>> GetByStatusAsync(string status);

        // Retrieves pending installments for a particular loan.
        // Useful for identifying unpaid installments that have not yet been classified as overdue.
        Task<IEnumerable<LoanSchedule>> GetPendingSchedulesByLoanIdAsync(int loanId);

        // Retrieves partially paid installments for a particular loan.
        // Useful for tracking remaining dues after partial customer payments.
        Task<IEnumerable<LoanSchedule>> GetPartiallyPaidSchedulesByLoanIdAsync(int loanId);

        // Retrieves paid installments for a particular loan.
        // Useful for reviewing completed EMI obligations.
        Task<IEnumerable<LoanSchedule>> GetPaidSchedulesByLoanIdAsync(int loanId);

        // Retrieves overdue installments for a particular loan.
        // Useful for monitoring delinquent installments and collection follow-up.
        Task<IEnumerable<LoanSchedule>> GetOverdueSchedulesByLoanIdAsync(int loanId);

        // Retrieves waived installments for a particular loan.
        // Useful for reviewing installments where dues were formally waived.
        Task<IEnumerable<LoanSchedule>> GetWaivedSchedulesByLoanIdAsync(int loanId);

        // =========================================================
        // 4. DUE DATE MONITORING
        // =========================================================

        // Retrieves installments due within the supplied date range.
        // Useful for planning upcoming EMI collection activity.
        Task<IEnumerable<LoanSchedule>> GetSchedulesDueWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves unpaid or partially paid installments due before the supplied date.
        // Useful for identifying installments that may require overdue assessment.
        Task<IEnumerable<LoanSchedule>> GetUnpaidSchedulesDueBeforeAsync(DateTime date);

        // Retrieves installments for a loan due on a particular date.
        // Useful for daily EMI collection monitoring.
        Task<IEnumerable<LoanSchedule>> GetSchedulesDueOnDateAsync(int loanId, DateTime dueDate);

        // Retrieves installments marked overdue across all loans.
        // Useful for centralized delinquency monitoring and collection work queues.
        Task<IEnumerable<LoanSchedule>> GetAllOverdueSchedulesAsync();

        // =========================================================
        // 5. PAYMENT PROGRESS
        // =========================================================

        // Retrieves installments paid within the supplied date range.
        // Useful for reviewing completed installment collections by payment date.
        Task<IEnumerable<LoanSchedule>> GetSchedulesPaidWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves installments whose remaining amount is greater than the supplied threshold.
        // Useful for prioritizing installments with significant unpaid balances.
        Task<IEnumerable<LoanSchedule>> GetSchedulesWithOutstandingAmountAboveAsync(decimal minimumAmount);

        // =========================================================
        // 6. FINANCIAL TOTALS
        // =========================================================

        // Calculates the total principal due for a particular loan.
        // Useful for reviewing the principal obligation across its repayment schedule.
        Task<decimal> GetTotalPrincipalDueByLoanIdAsync(int loanId);

        // Calculates the total interest due for a particular loan.
        // Useful for reviewing the scheduled interest obligation.
        Task<decimal> GetTotalInterestDueByLoanIdAsync(int loanId);

        // Calculates the total principal paid for a particular loan.
        // Useful for tracking principal recovery through scheduled installments.
        Task<decimal> GetTotalPrincipalPaidByLoanIdAsync(int loanId);

        // Calculates the total interest paid for a particular loan.
        // Useful for tracking interest collections through scheduled installments.
        Task<decimal> GetTotalInterestPaidByLoanIdAsync(int loanId);

        // Calculates the total penalty accrued for a particular loan.
        // Useful for reviewing penalties assessed against scheduled installments.
        Task<decimal> GetTotalPenaltyAccruedByLoanIdAsync(int loanId);

        // Calculates the total penalty paid for a particular loan.
        // Useful for tracking penalty collections separately from principal and interest.
        Task<decimal> GetTotalPenaltyPaidByLoanIdAsync(int loanId);

        // =========================================================
        // 7. SCHEDULE COUNTS
        // =========================================================

        // Counts installments for a particular loan matching the supplied status.
        // Useful for tracking repayment progress and outstanding installment counts.
        Task<int> CountSchedulesByLoanAndStatusAsync(int loanId, string status);

        // Counts installments remaining unpaid or partially paid for a particular loan.
        // Useful for monitoring the number of outstanding repayment obligations.
        Task<int> CountUnpaidSchedulesByLoanIdAsync(int loanId);
    }
}
