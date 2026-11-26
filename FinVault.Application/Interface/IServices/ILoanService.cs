using FinVault.Application.DTOs.LoanDtos;
using FinVault.Application.DTOs.LoanRepaymentDtos;
using FinVault.Application.DTOs.LoanScheduleDtos;
using FinVault.Domain.Entities;
using System.Security.Principal;

namespace FinVault.Application.Interfaces.Services
{
    public interface ILoanService
    {
        // 1. LOAN IDENTIFICATION AND CRUD
        Task<IEnumerable<LoanDto>> GetAllAsync();
        Task<LoanDto?> GetByIdAsync(int loanId);
        Task<LoanDto?> GetByLoanNumberAsync(string loanNumber);
        Task<LoanDto?> GetLoanDetailsAsync(int loanId);
        Task<bool> ExistsByLoanNumberAsync(string loanNumber);
        Task<LoanDto> CreateAsync(CreateLoanDto dto);
        Task<LoanDto?> UpdateAsync(UpdateLoanDto dto);
        Task<bool> DeleteAsync(DeleteLoanDto dto);

        // 2. CUSTOMER AND BRANCH PORTFOLIO
        Task<IEnumerable<LoanDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<LoanDto>> GetByBranchIdAsync(int branchId);
        Task<IEnumerable<LoanDto>> GetByLoanTypeAsync(string loanType);
        Task<IEnumerable<LoanDto>> GetByDisbursementAccountIdAsync(int accountId);
        Task<IEnumerable<LoanDto>> GetByRepaymentAccountIdAsync(int accountId);

        // 3. LOAN STATUS AND LIFECYCLE
        Task<IEnumerable<LoanDto>> GetByStatusAsync(string status);
        Task<IEnumerable<LoanDto>> GetAppliedLoansAsync();
        Task<IEnumerable<LoanDto>> GetApprovedUndisbursedLoansAsync();
        Task<IEnumerable<LoanDto>> GetActiveLoansAsync();
        Task<IEnumerable<LoanDto>> GetClosedLoansAsync();

        // 4. SANCTION AND DISBURSEMENT
        Task<IEnumerable<LoanDto>> GetByApprovedByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<LoanDto>> GetDisbursedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanDto>> GetApplicationsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 5. OUTSTANDING BALANCE AND RISK
        Task<IEnumerable<LoanDto>> GetLoansWithOutstandingPrincipalAboveAsync(decimal minimumAmount);
        Task<IEnumerable<LoanDto>> GetByAssetClassificationAsync(string assetClassification);
        Task<IEnumerable<LoanDto>> GetLoansPastDueAsync(int minimumDaysPastDue);
        Task<IEnumerable<LoanDto>> GetLoansMaturingWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 6. LOAN RELATIONSHIPS
        Task<IEnumerable<LoanDto>> GetLoansWithSchedulesAsync(int customerId);
        Task<IEnumerable<LoanDto>> GetLoansWithRepaymentsAsync(int customerId);
        Task<LoanDto?> GetLoanWithInterestCalculationsAsync(int loanId);

        // 7. LOAN FINANCIAL REPORTING AND SEARCH
        Task<decimal> GetTotalOutstandingPrincipalByBranchIdAsync(int branchId);
        Task<decimal> GetTotalOutstandingPrincipalByCustomerIdAsync(int customerId);
        Task<IEnumerable<LoanDto>> SearchLoansAsync(string searchTerm);

        // 8. LOAN SCHEDULE IDENTIFICATION AND CRUD
        Task<IEnumerable<LoanScheduleDto>> GetAllSchedulesAsync();
        Task<LoanScheduleDto?> GetScheduleByIdAsync(int loanScheduleId);
        Task<LoanScheduleDto?> GetByLoanAndInstallmentAsync(int loanId, int installmentNumber);
        Task<LoanScheduleDto?> GetScheduleDetailsAsync(int loanScheduleId);
        Task<bool> ExistsByLoanAndInstallmentAsync(int loanId, int installmentNumber);
        Task<LoanScheduleDto> CreateScheduleAsync(CreateLoanScheduleDto dto);
        Task<LoanScheduleDto?> UpdateScheduleAsync(UpdateLoanScheduleDto dto);
        Task<bool> DeleteScheduleAsync(DeleteLoanScheduleDto dto);

        // 9. LOAN SCHEDULE HISTORY AND STATUS
        Task<IEnumerable<LoanScheduleDto>> GetSchedulesByLoanIdAsync(int loanId);
        Task<IEnumerable<LoanScheduleDto>> GetLoanSchedulesWithRepaymentsAsync(int loanId);
        Task<LoanScheduleDto?> GetScheduleWithRepaymentsAsync(int loanScheduleId);
        Task<IEnumerable<LoanScheduleDto>> GetSchedulesByStatusAsync(string status);
        Task<IEnumerable<LoanScheduleDto>> GetPendingSchedulesByLoanIdAsync(int loanId);
        Task<IEnumerable<LoanScheduleDto>> GetPartiallyPaidSchedulesByLoanIdAsync(int loanId);
        Task<IEnumerable<LoanScheduleDto>> GetPaidSchedulesByLoanIdAsync(int loanId);
        Task<IEnumerable<LoanScheduleDto>> GetOverdueSchedulesByLoanIdAsync(int loanId);
        Task<IEnumerable<LoanScheduleDto>> GetWaivedSchedulesByLoanIdAsync(int loanId);

        // 10. SCHEDULE DUE DATE AND PAYMENT PROGRESS
        Task<IEnumerable<LoanScheduleDto>> GetSchedulesDueWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanScheduleDto>> GetUnpaidSchedulesDueBeforeAsync(DateTime date);
        Task<IEnumerable<LoanScheduleDto>> GetSchedulesDueOnDateAsync(int loanId, DateTime dueDate);
        Task<IEnumerable<LoanScheduleDto>> GetAllOverdueSchedulesAsync();
        Task<IEnumerable<LoanScheduleDto>> GetSchedulesPaidWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanScheduleDto>> GetSchedulesWithOutstandingAmountAboveAsync(decimal minimumAmount);

        // 11. SCHEDULE FINANCIAL REPORTING
        Task<decimal> GetTotalPrincipalDueByLoanIdAsync(int loanId);
        Task<decimal> GetTotalInterestDueByLoanIdAsync(int loanId);
        Task<decimal> GetTotalPrincipalPaidByLoanIdAsync(int loanId);
        Task<decimal> GetTotalInterestPaidByLoanIdAsync(int loanId);
        Task<decimal> GetTotalPenaltyAccruedByLoanIdAsync(int loanId);
        Task<decimal> GetTotalPenaltyPaidByLoanIdAsync(int loanId);
        Task<int> CountSchedulesByLoanAndStatusAsync(int loanId, string status);
        Task<int> CountUnpaidSchedulesByLoanIdAsync(int loanId);

        // 12. LOAN REPAYMENT IDENTIFICATION AND CRUD
        Task<IEnumerable<LoanRepaymentDto>> GetAllRepaymentsAsync();
        Task<LoanRepaymentDto?> GetRepaymentByIdAsync(int loanRepaymentId);
        Task<LoanRepaymentDto?> GetByRepaymentReferenceAsync(string repaymentReference);
        Task<LoanRepaymentDto?> GetRepaymentDetailsAsync(int loanRepaymentId);
        Task<bool> ExistsByRepaymentReferenceAsync(string repaymentReference);
        Task<LoanRepaymentDto> CreateRepaymentAsync(CreateLoanRepaymentDto dto);
        Task<LoanRepaymentDto?> UpdateRepaymentAsync(UpdateLoanRepaymentDto dto);
        Task<bool> DeleteRepaymentAsync(DeleteLoanRepaymentDto dto);

        // 13. REPAYMENT HISTORY AND STATUS
        Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByLoanIdAsync(int loanId);
        Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByLoanScheduleIdAsync(int loanScheduleId);
        Task<LoanRepaymentDto?> GetRepaymentByTransactionIdAsync(int transactionId);
        Task<IEnumerable<LoanRepaymentDto>> GetLoanRepaymentsWithTransactionsAsync(int loanId);
        Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByStatusAsync(string status);
        Task<IEnumerable<LoanRepaymentDto>> GetSuccessfulRepaymentsByLoanIdAsync(int loanId);
        Task<IEnumerable<LoanRepaymentDto>> GetPendingRepaymentsAsync();
        Task<IEnumerable<LoanRepaymentDto>> GetFailedRepaymentsAsync();
        Task<IEnumerable<LoanRepaymentDto>> GetReversedRepaymentsAsync();

        // 14. REPAYMENT TYPE, MODE AND DATE REPORTING
        Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByTypeAsync(string repaymentType);
        Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByPaymentModeAsync(string paymentMode);
        Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanRepaymentDto>> GetSuccessfulRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanRepaymentDto>> GetRecentRepaymentsAsync(DateTime fromDate);

        // 15. REPAYMENT FINANCIAL REPORTING AND SEARCH
        Task<decimal> GetTotalSuccessfulAmountByLoanIdAsync(int loanId);
        Task<decimal> GetTotalPrincipalCollectedByLoanIdAsync(int loanId);
        Task<decimal> GetTotalInterestCollectedByLoanIdAsync(int loanId);
        Task<decimal> GetTotalPenaltyCollectedByLoanIdAsync(int loanId);
        Task<decimal> GetTotalSuccessfulCollectionsWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanRepaymentDto>> SearchRepaymentsAsync(string searchTerm);
        Task<int> CountSuccessfulRepaymentsByLoanIdAsync(int loanId);
    }
}


//Banking rules:
//-Enforce loan sanction and approval controls before disbursement.
//- Disbursement must update the loan and linked account transaction consistently.
//- Generate schedules using the sanctioned amount, interest rate and tenure.
//- Reconcile repayment components: principal, interest and penalty.
//- Use RowVersion for loan and schedule concurrency checks.
//- Preserve posted repayment and financial history; use reversals for corrections.