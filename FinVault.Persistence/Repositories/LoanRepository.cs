
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class LoanRepository : GenericRepository<Loan>, ILoanRepository
    {
        public LoanRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. LOAN IDENTIFICATION
        // =========================================================

        // Retrieves a loan using its unique loan number.
        // Useful for tracking a loan during customer service and loan operations.
        public async Task<Loan?> GetByLoanNumberAsync(string loanNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.LoanNumber == loanNumber);
        }

        // Retrieves complete loan details with customer, branch, accounts, interest rate and related records.
        // Useful for reviewing the complete loan profile and its financial history.
        public async Task<Loan?> GetLoanDetailsAsync(int loanId)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.Branch)
                .Include(x => x.DisbursementAccount)
                .Include(x => x.RepaymentAccount)
                .Include(x => x.InterestRate)
                .Include(x => x.ApprovedByEmployee)
                .Include(x => x.LoanSchedules)
                .Include(x => x.LoanRepayments)
                .Include(x => x.InterestCalculations)
                .Include(x => x.Documents)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.LoanId == loanId);
        }

        // Checks whether a loan number already exists.
        // Useful for preventing duplicate loan references during application creation.
        public async Task<bool> ExistsByLoanNumberAsync(string loanNumber)
        {
            return await _dbSet.AnyAsync(x => x.LoanNumber == loanNumber);
        }

        // =========================================================
        // 2. CUSTOMER AND BRANCH PORTFOLIO
        // =========================================================

        // Retrieves all loans belonging to a customer.
        // Useful for reviewing a customer's borrowing history and total loan exposure.
        public async Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // Retrieves loans belonging to a particular branch.
        // Useful for branch-level loan portfolio monitoring.
        public async Task<IEnumerable<Loan>> GetByBranchIdAsync(int branchId)
        {
            return await _dbSet
                .Where(x => x.BranchId == branchId)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // Retrieves loans of a particular loan type.
        // Useful for separating personal, home, vehicle, education and business loan portfolios.
        public async Task<IEnumerable<Loan>> GetByLoanTypeAsync(string loanType)
        {
            return await _dbSet
                .Where(x => x.LoanType == loanType)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // Retrieves loans associated with a particular disbursement account.
        // Useful for tracing loan disbursements to customer accounts.
        public async Task<IEnumerable<Loan>> GetByDisbursementAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.DisbursementAccountId == accountId)
                .OrderByDescending(x => x.DisbursementDate)
                .ToListAsync();
        }

        // Retrieves loans associated with a particular repayment account.
        // Useful for reviewing loans linked to an account for EMI collection.
        public async Task<IEnumerable<Loan>> GetByRepaymentAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.RepaymentAccountId == accountId)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // =========================================================
        // 3. LOAN STATUS AND LIFECYCLE
        // =========================================================

        // Retrieves loans matching a particular lifecycle status.
        // Useful for monitoring loan applications, approvals, disbursements and closures.
        public async Task<IEnumerable<Loan>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // Retrieves loan applications awaiting a decision.
        // Useful for managing the loan application review queue.
        public async Task<IEnumerable<Loan>> GetAppliedLoansAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Applied")
                .OrderBy(x => x.ApplicationDate)
                .ToListAsync();
        }

        // Retrieves approved loans that have not yet been disbursed.
        // Useful for tracking sanctioned loans awaiting disbursement.
        public async Task<IEnumerable<Loan>> GetApprovedUndisbursedLoansAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Approved" && x.DisbursementDate == null)
                .OrderBy(x => x.SanctionDate)
                .ToListAsync();
        }

        // Retrieves loans that have been disbursed and are active.
        // Useful for monitoring the bank's active lending portfolio.
        public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Active")
                .OrderByDescending(x => x.DisbursementDate)
                .ToListAsync();
        }

        // Retrieves loans that have been closed.
        // Useful for reviewing completed loan accounts and historical lending activity.
        public async Task<IEnumerable<Loan>> GetClosedLoansAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Closed")
                .OrderByDescending(x => x.ClosedOn)
                .ToListAsync();
        }

        // =========================================================
        // 4. SANCTION AND DISBURSEMENT
        // =========================================================

        // Retrieves loans approved by a particular employee.
        // Useful for auditing loan sanction decisions and employee approval activity.
        public async Task<IEnumerable<Loan>> GetByApprovedByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(x => x.ApprovedByEmployeeId == employeeId)
                .OrderByDescending(x => x.SanctionDate)
                .ToListAsync();
        }

        // Retrieves loans disbursed within the supplied date range.
        // Useful for monitoring loan disbursement volumes and financial reporting.
        public async Task<IEnumerable<Loan>> GetDisbursedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.DisbursementDate.HasValue &&
                            x.DisbursementDate.Value >= startDate &&
                            x.DisbursementDate.Value <= endDate)
                .OrderByDescending(x => x.DisbursementDate)
                .ToListAsync();
        }

        // Retrieves loans applied for within the supplied date range.
        // Useful for monitoring loan application trends and processing volumes.
        public async Task<IEnumerable<Loan>> GetApplicationsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ApplicationDate >= startDate && x.ApplicationDate <= endDate)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // =========================================================
        // 5. OUTSTANDING BALANCE AND RISK
        // =========================================================

        // Retrieves active loans whose outstanding principal is above the supplied amount.
        // Useful for identifying large outstanding exposures.
        public async Task<IEnumerable<Loan>> GetLoansWithOutstandingPrincipalAboveAsync(decimal minimumAmount)
        {
            return await _dbSet
                .Where(x => x.Status == "Active" && x.OutstandingPrincipal > minimumAmount)
                .OrderByDescending(x => x.OutstandingPrincipal)
                .ToListAsync();
        }

        // Retrieves loans matching a particular RBI asset classification.
        // Useful for monitoring standard, SMA and non-performing asset categories.
        public async Task<IEnumerable<Loan>> GetByAssetClassificationAsync(string assetClassification)
        {
            return await _dbSet
                .Where(x => x.AssetClassification == assetClassification)
                .OrderByDescending(x => x.DaysPastDue)
                .ToListAsync();
        }

        // Retrieves active loans with days past due greater than the supplied threshold.
        // Useful for identifying delinquent loans requiring collection or risk review.
        public async Task<IEnumerable<Loan>> GetLoansPastDueAsync(int minimumDaysPastDue)
        {
            return await _dbSet
                .Where(x => x.Status == "Active" && x.DaysPastDue > minimumDaysPastDue)
                .OrderByDescending(x => x.DaysPastDue)
                .ToListAsync();
        }

        // Retrieves loans whose maturity date falls within the supplied date range.
        // Useful for monitoring upcoming loan maturity and closure requirements.
        public async Task<IEnumerable<Loan>> GetLoansMaturingWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.MaturityDate.HasValue &&
                            x.MaturityDate.Value >= startDate &&
                            x.MaturityDate.Value <= endDate)
                .OrderBy(x => x.MaturityDate)
                .ToListAsync();
        }

        // =========================================================
        // 6. RELATED SCHEDULES AND REPAYMENTS
        // =========================================================

        // Retrieves loans together with their repayment schedules.
        // Useful for reviewing the EMI schedule and repayment obligations.
        public async Task<IEnumerable<Loan>> GetLoansWithSchedulesAsync(int customerId)
        {
            return await _dbSet
                .Include(x => x.LoanSchedules)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // Retrieves loans together with their repayment records.
        // Useful for reviewing actual repayments against loan obligations.
        public async Task<IEnumerable<Loan>> GetLoansWithRepaymentsAsync(int customerId)
        {
            return await _dbSet
                .Include(x => x.LoanRepayments)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }

        // Retrieves a loan with its interest calculation history.
        // Useful for reviewing interest accruals and loan interest accounting.
        public async Task<Loan?> GetLoanWithInterestCalculationsAsync(int loanId)
        {
            return await _dbSet
                .Include(x => x.InterestCalculations)
                .FirstOrDefaultAsync(x => x.LoanId == loanId);
        }

        // =========================================================
        // 7. REPORTING AND SEARCH
        // =========================================================

        // Calculates the total outstanding principal for a branch's active loans.
        // Useful for branch-level lending exposure reporting.
        public async Task<decimal> GetTotalOutstandingPrincipalByBranchIdAsync(int branchId)
        {
            return await _dbSet
                .Where(x => x.BranchId == branchId && x.Status == "Active")
                .SumAsync(x => (decimal?)x.OutstandingPrincipal) ?? 0m;
        }

        // Calculates the total outstanding principal for a customer's active loans.
        // Useful for customer-level credit exposure assessment.
        public async Task<decimal> GetTotalOutstandingPrincipalByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId && x.Status == "Active")
                .SumAsync(x => (decimal?)x.OutstandingPrincipal) ?? 0m;
        }

        // Searches loans by loan number, purpose or customer name.
        // Useful for locating loan records during customer service and loan operations.
        public async Task<IEnumerable<Loan>> SearchLoansAsync(string searchTerm)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Where(x => x.LoanNumber.Contains(searchTerm) ||
                            (x.Purpose != null && x.Purpose.Contains(searchTerm)) ||
                            x.Customer.FirstName.Contains(searchTerm) ||
                            x.Customer.LastName.Contains(searchTerm))
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();
        }
    }
}
