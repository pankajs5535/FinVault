
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        // =========================================================
        // 1. LOAN IDENTIFICATION
        // =========================================================

        // Retrieves a loan using its unique loan number.
        // Useful for tracking a loan during customer service and loan operations.
        Task<Loan?> GetByLoanNumberAsync(string loanNumber);

        // Retrieves complete loan details with customer, branch, accounts, interest rate and related records.
        // Useful for reviewing the complete loan profile and its financial history.
        Task<Loan?> GetLoanDetailsAsync(int loanId);

        // Checks whether a loan number already exists.
        // Useful for preventing duplicate loan references during application creation.
        Task<bool> ExistsByLoanNumberAsync(string loanNumber);

        // =========================================================
        // 2. CUSTOMER AND BRANCH PORTFOLIO
        // =========================================================

        // Retrieves all loans belonging to a customer.
        // Useful for reviewing a customer's borrowing history and total loan exposure.
        Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId);

        // Retrieves loans belonging to a particular branch.
        // Useful for branch-level loan portfolio monitoring.
        Task<IEnumerable<Loan>> GetByBranchIdAsync(int branchId);

        // Retrieves loans of a particular loan type.
        // Useful for separating personal, home, vehicle, education and business loan portfolios.
        Task<IEnumerable<Loan>> GetByLoanTypeAsync(string loanType);

        // Retrieves loans associated with a particular disbursement account.
        // Useful for tracing loan disbursements to customer accounts.
        Task<IEnumerable<Loan>> GetByDisbursementAccountIdAsync(int accountId);

        // Retrieves loans associated with a particular repayment account.
        // Useful for reviewing loans linked to an account for EMI collection.
        Task<IEnumerable<Loan>> GetByRepaymentAccountIdAsync(int accountId);

        // =========================================================
        // 3. LOAN STATUS AND LIFECYCLE
        // =========================================================

        // Retrieves loans matching a particular lifecycle status.
        // Useful for monitoring loan applications, approvals, disbursements and closures.
        Task<IEnumerable<Loan>> GetByStatusAsync(string status);

        // Retrieves loan applications awaiting a decision.
        // Useful for managing the loan application review queue.
        Task<IEnumerable<Loan>> GetAppliedLoansAsync();

        // Retrieves approved loans that have not yet been disbursed.
        // Useful for tracking sanctioned loans awaiting disbursement.
        Task<IEnumerable<Loan>> GetApprovedUndisbursedLoansAsync();

        // Retrieves loans that have been disbursed and are active.
        // Useful for monitoring the bank's active lending portfolio.
        Task<IEnumerable<Loan>> GetActiveLoansAsync();

        // Retrieves loans that have been closed.
        // Useful for reviewing completed loan accounts and historical lending activity.
        Task<IEnumerable<Loan>> GetClosedLoansAsync();

        // =========================================================
        // 4. SANCTION AND DISBURSEMENT
        // =========================================================

        // Retrieves loans approved by a particular employee.
        // Useful for auditing loan sanction decisions and employee approval activity.
        Task<IEnumerable<Loan>> GetByApprovedByEmployeeIdAsync(int employeeId);

        // Retrieves loans disbursed within the supplied date range.
        // Useful for monitoring loan disbursement volumes and financial reporting.
        Task<IEnumerable<Loan>> GetDisbursedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves loans applied for within the supplied date range.
        // Useful for monitoring loan application trends and processing volumes.
        Task<IEnumerable<Loan>> GetApplicationsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 5. OUTSTANDING BALANCE AND RISK
        // =========================================================

        // Retrieves active loans whose outstanding principal is above the supplied amount.
        // Useful for identifying large outstanding exposures.
        Task<IEnumerable<Loan>> GetLoansWithOutstandingPrincipalAboveAsync(decimal minimumAmount);

        // Retrieves loans matching a particular RBI asset classification.
        // Useful for monitoring standard, SMA and non-performing asset categories.
        Task<IEnumerable<Loan>> GetByAssetClassificationAsync(string assetClassification);

        // Retrieves active loans with days past due greater than the supplied threshold.
        // Useful for identifying delinquent loans requiring collection or risk review.
        Task<IEnumerable<Loan>> GetLoansPastDueAsync(int minimumDaysPastDue);

        // Retrieves loans whose maturity date falls within the supplied date range.
        // Useful for monitoring upcoming loan maturity and closure requirements.
        Task<IEnumerable<Loan>> GetLoansMaturingWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 6. RELATED SCHEDULES AND REPAYMENTS
        // =========================================================

        // Retrieves loans together with their repayment schedules.
        // Useful for reviewing the EMI schedule and repayment obligations.
        Task<IEnumerable<Loan>> GetLoansWithSchedulesAsync(int customerId);

        // Retrieves loans together with their repayment records.
        // Useful for reviewing actual repayments against loan obligations.
        Task<IEnumerable<Loan>> GetLoansWithRepaymentsAsync(int customerId);

        // Retrieves loans with their interest calculation history.
        // Useful for reviewing interest accruals and loan interest accounting.
        Task<Loan?> GetLoanWithInterestCalculationsAsync(int loanId);

        // =========================================================
        // 7. REPORTING AND SEARCH
        // =========================================================

        // Calculates the total outstanding principal for a branch's active loans.
        // Useful for branch-level lending exposure reporting.
        Task<decimal> GetTotalOutstandingPrincipalByBranchIdAsync(int branchId);

        // Calculates the total outstanding principal for a customer's active loans.
        // Useful for customer-level credit exposure assessment.
        Task<decimal> GetTotalOutstandingPrincipalByCustomerIdAsync(int customerId);

        // Searches loans by loan number, purpose or customer name.
        // Useful for locating loan records during customer service and loan operations.
        Task<IEnumerable<Loan>> SearchLoansAsync(string searchTerm);
    }
}
