
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface ILoanRepaymentRepository : IGenericRepository<LoanRepayment>
    {
        // =========================================================
        // 1. REPAYMENT IDENTIFICATION
        // =========================================================

        // Retrieves a repayment using its unique reference.
        // Useful for tracing an individual repayment during customer support and reconciliation.
        Task<LoanRepayment?> GetByRepaymentReferenceAsync(string repaymentReference);

        // Retrieves complete repayment details with loan, schedule, transaction and audit users.
        // Useful for investigating a repayment and its linked financial transaction.
        Task<LoanRepayment?> GetRepaymentDetailsAsync(int loanRepaymentId);

        // Checks whether a repayment reference already exists.
        // Useful for preventing duplicate repayment references.
        Task<bool> ExistsByRepaymentReferenceAsync(string repaymentReference);

        // =========================================================
        // 2. LOAN AND SCHEDULE HISTORY
        // =========================================================

        // Retrieves repayments belonging to a particular loan.
        // Useful for reviewing the customer's loan repayment history.
        Task<IEnumerable<LoanRepayment>> GetByLoanIdAsync(int loanId);

        // Retrieves repayments linked to a particular repayment schedule.
        // Useful for checking whether a scheduled EMI has been paid.
        Task<IEnumerable<LoanRepayment>> GetByLoanScheduleIdAsync(int loanScheduleId);

        // Retrieves a repayment linked to a particular banking transaction.
        // Useful for tracing loan repayments to ledger-backed transactions.
        Task<LoanRepayment?> GetByTransactionIdAsync(int transactionId);

        // Retrieves repayments with their linked transaction details for a loan.
        // Useful for reconciling loan repayment records against transaction records.
        Task<IEnumerable<LoanRepayment>> GetLoanRepaymentsWithTransactionsAsync(int loanId);

        // =========================================================
        // 3. REPAYMENT STATUS
        // =========================================================

        // Retrieves repayments matching a particular status.
        // Useful for monitoring pending, successful, failed and reversed repayments.
        Task<IEnumerable<LoanRepayment>> GetByStatusAsync(string status);

        // Retrieves successful repayments for a particular loan.
        // Useful for calculating payments that have actually been completed.
        Task<IEnumerable<LoanRepayment>> GetSuccessfulRepaymentsByLoanIdAsync(int loanId);

        // Retrieves pending repayments.
        // Useful for monitoring payments awaiting processing or confirmation.
        Task<IEnumerable<LoanRepayment>> GetPendingRepaymentsAsync();

        // Retrieves failed repayments.
        // Useful for identifying payment failures requiring investigation or retry decisions.
        Task<IEnumerable<LoanRepayment>> GetFailedRepaymentsAsync();

        // Retrieves reversed repayments.
        // Useful for reviewing repayment reversals and their financial impact.
        Task<IEnumerable<LoanRepayment>> GetReversedRepaymentsAsync();

        // =========================================================
        // 4. REPAYMENT TYPE AND PAYMENT MODE
        // =========================================================

        // Retrieves repayments matching a particular repayment type.
        // Useful for separating EMI, part-payment, foreclosure and penalty collections.
        Task<IEnumerable<LoanRepayment>> GetByRepaymentTypeAsync(string repaymentType);

        // Retrieves repayments processed through a particular payment mode.
        // Useful for monitoring AutoDebit, NACH, online and branch collections.
        Task<IEnumerable<LoanRepayment>> GetByPaymentModeAsync(string paymentMode);

        // =========================================================
        // 5. DATE-BASED REPAYMENT REPORTING
        // =========================================================

        // Retrieves repayments received within the supplied date range.
        // Useful for daily, monthly and period-based repayment reporting.
        Task<IEnumerable<LoanRepayment>> GetRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves successful repayments received within the supplied date range.
        // Useful for reporting confirmed repayment collections.
        Task<IEnumerable<LoanRepayment>> GetSuccessfulRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves repayments recorded after the supplied date.
        // Useful for reviewing recent loan collection activity.
        Task<IEnumerable<LoanRepayment>> GetRecentRepaymentsAsync(DateTime fromDate);

        // =========================================================
        // 6. FINANCIAL TOTALS
        // =========================================================

        // Calculates the total amount of successful repayments for a loan.
        // Useful for reviewing the amount collected against a loan.
        Task<decimal> GetTotalSuccessfulAmountByLoanIdAsync(int loanId);

        // Calculates the successful principal component collected for a loan.
        // Useful for tracking principal recovery separately from interest and penalties.
        Task<decimal> GetTotalPrincipalCollectedByLoanIdAsync(int loanId);

        // Calculates the successful interest component collected for a loan.
        // Useful for reviewing interest income collected from a loan.
        Task<decimal> GetTotalInterestCollectedByLoanIdAsync(int loanId);

        // Calculates the successful penalty component collected for a loan.
        // Useful for reporting penalty collections separately from regular repayments.
        Task<decimal> GetTotalPenaltyCollectedByLoanIdAsync(int loanId);

        // Calculates successful repayment collections within a date range.
        // Useful for measuring confirmed collection performance over a reporting period.
        Task<decimal> GetTotalSuccessfulCollectionsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 7. SEARCH AND COUNTS
        // =========================================================

        // Searches repayments by reference, failure reason or linked loan number.
        // Useful for locating repayment records during support and investigation.
        Task<IEnumerable<LoanRepayment>> SearchRepaymentsAsync(string searchTerm);

        // Counts successful repayments recorded for a particular loan.
        // Useful for monitoring completed repayment activity.
        Task<int> CountSuccessfulRepaymentsByLoanIdAsync(int loanId);
    }
}
