
using FinVault.Application.DTOs.LoanDtos;
using FinVault.Application.DTOs.LoanScheduleDtos;
using FinVault.Application.DTOs.LoanRepaymentDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class LoanController : ControllerBase
{
    private readonly ILoanService _service;

    public LoanController(ILoanService service) => _service = service;

    // ==================== LOAN IDENTIFICATION AND CRUD (8) ====================

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetAllAsync()));

    [HttpGet("{loanId:int}")]
    public async Task<IActionResult> GetById(int loanId) =>
        Result(await _service.GetByIdAsync(loanId));

    [HttpGet("number/{loanNumber}")]
    public async Task<IActionResult> GetByLoanNumber(string loanNumber) =>
        Result(await _service.GetByLoanNumberAsync(loanNumber));

    [HttpGet("{loanId:int}/details")]
    public async Task<IActionResult> GetLoanDetails(int loanId) =>
        Result(await _service.GetLoanDetailsAsync(loanId));

    [HttpGet("exists/number/{loanNumber}")]
    public async Task<IActionResult> ExistsByLoanNumber(string loanNumber) =>
        Ok(ApiResponse<bool>.SuccessResponse(await _service.ExistsByLoanNumberAsync(loanNumber)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoanDto request)
    {
        var data = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { loanId = data.LoanId },
            ApiResponse<LoanDto>.SuccessResponse(data, "Loan created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateLoanDto request) =>
        Result(await _service.UpdateAsync(request));

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteLoanDto request) =>
        BoolResult(await _service.DeleteAsync(request), "Loan deleted successfully.");

    // ==================== CUSTOMER AND BRANCH PORTFOLIO (5) ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByCustomerIdAsync(customerId)));

    [HttpGet("branch/{branchId:int}")]
    public async Task<IActionResult> GetByBranchId(int branchId) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByBranchIdAsync(branchId)));

    [HttpGet("type/{loanType}")]
    public async Task<IActionResult> GetByLoanType(string loanType) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByLoanTypeAsync(loanType)));

    [HttpGet("disbursement-account/{accountId:int}")]
    public async Task<IActionResult> GetByDisbursementAccountId(int accountId) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByDisbursementAccountIdAsync(accountId)));

    [HttpGet("repayment-account/{accountId:int}")]
    public async Task<IActionResult> GetByRepaymentAccountId(int accountId) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByRepaymentAccountIdAsync(accountId)));

    // ==================== LOAN STATUS AND LIFECYCLE (5) ====================

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByStatusAsync(status)));

    [HttpGet("applied")]
    public async Task<IActionResult> GetAppliedLoans() =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetAppliedLoansAsync()));

    [HttpGet("approved-undisbursed")]
    public async Task<IActionResult> GetApprovedUndisbursedLoans() =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetApprovedUndisbursedLoansAsync()));

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveLoans() =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetActiveLoansAsync()));

    [HttpGet("closed")]
    public async Task<IActionResult> GetClosedLoans() =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetClosedLoansAsync()));

    // ==================== SANCTION AND DISBURSEMENT (3) ====================

    [HttpGet("approved-by/{employeeId:int}")]
    public async Task<IActionResult> GetByApprovedByEmployeeId(int employeeId) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByApprovedByEmployeeIdAsync(employeeId)));

    [HttpGet("disbursed/date-range")]
    public async Task<IActionResult> GetDisbursedWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetDisbursedWithinRangeAsync(startDate, endDate)));
    }

    [HttpGet("applications/date-range")]
    public async Task<IActionResult> GetApplicationsWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetApplicationsWithinRangeAsync(startDate, endDate)));
    }

    // ==================== OUTSTANDING BALANCE AND RISK (4) ====================

    [HttpGet("outstanding-principal/above")]
    public async Task<IActionResult> GetLoansWithOutstandingPrincipalAbove([FromQuery] decimal minimumAmount) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetLoansWithOutstandingPrincipalAboveAsync(minimumAmount)));

    [HttpGet("asset-classification/{assetClassification}")]
    public async Task<IActionResult> GetByAssetClassification(string assetClassification) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetByAssetClassificationAsync(assetClassification)));

    [HttpGet("past-due")]
    public async Task<IActionResult> GetLoansPastDue([FromQuery] int minimumDaysPastDue) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetLoansPastDueAsync(minimumDaysPastDue)));

    [HttpGet("maturing/date-range")]
    public async Task<IActionResult> GetLoansMaturingWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetLoansMaturingWithinRangeAsync(startDate, endDate)));
    }

    // ==================== LOAN RELATIONSHIPS (3) ====================

    [HttpGet("customer/{customerId:int}/with-schedules")]
    public async Task<IActionResult> GetLoansWithSchedules(int customerId) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetLoansWithSchedulesAsync(customerId)));

    [HttpGet("customer/{customerId:int}/with-repayments")]
    public async Task<IActionResult> GetLoansWithRepayments(int customerId) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.GetLoansWithRepaymentsAsync(customerId)));

    [HttpGet("{loanId:int}/interest-calculations")]
    public async Task<IActionResult> GetLoanWithInterestCalculations(int loanId) =>
        Result(await _service.GetLoanWithInterestCalculationsAsync(loanId));

    // ==================== LOAN FINANCIAL REPORTING AND SEARCH (3) ====================

    [HttpGet("branch/{branchId:int}/total-outstanding-principal")]
    public async Task<IActionResult> GetTotalOutstandingPrincipalByBranchId(int branchId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalOutstandingPrincipalByBranchIdAsync(branchId)));

    [HttpGet("customer/{customerId:int}/total-outstanding-principal")]
    public async Task<IActionResult> GetTotalOutstandingPrincipalByCustomerId(int customerId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalOutstandingPrincipalByCustomerIdAsync(customerId)));

    [HttpGet("search")]
    public async Task<IActionResult> SearchLoans([FromQuery] string searchTerm) =>
        Ok(ApiResponse<IEnumerable<LoanDto>>.SuccessResponse(await _service.SearchLoansAsync(searchTerm)));

    // ==================== LOAN SCHEDULE IDENTIFICATION AND CRUD (8) ====================

    [HttpGet("schedules")]
    public async Task<IActionResult> GetAllSchedules() =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetAllSchedulesAsync()));

    [HttpGet("schedules/{loanScheduleId:int}")]
    public async Task<IActionResult> GetScheduleById(int loanScheduleId) =>
        Result(await _service.GetScheduleByIdAsync(loanScheduleId));

    [HttpGet("{loanId:int}/schedules/installment/{installmentNumber:int}")]
    public async Task<IActionResult> GetByLoanAndInstallment(int loanId, int installmentNumber) =>
        Result(await _service.GetByLoanAndInstallmentAsync(loanId, installmentNumber));

    [HttpGet("schedules/{loanScheduleId:int}/details")]
    public async Task<IActionResult> GetScheduleDetails(int loanScheduleId) =>
        Result(await _service.GetScheduleDetailsAsync(loanScheduleId));

    [HttpGet("{loanId:int}/schedules/exists/{installmentNumber:int}")]
    public async Task<IActionResult> ExistsByLoanAndInstallment(int loanId, int installmentNumber) =>
        Ok(ApiResponse<bool>.SuccessResponse(await _service.ExistsByLoanAndInstallmentAsync(loanId, installmentNumber)));

    [HttpPost("schedules")]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateLoanScheduleDto request)
    {
        var data = await _service.CreateScheduleAsync(request);
        return CreatedAtAction(nameof(GetScheduleById), new { loanScheduleId = data.LoanScheduleId },
            ApiResponse<LoanScheduleDto>.SuccessResponse(data, "Loan schedule created successfully.", 201));
    }

    [HttpPut("schedules")]
    public async Task<IActionResult> UpdateSchedule([FromBody] UpdateLoanScheduleDto request) =>
        Result(await _service.UpdateScheduleAsync(request));

    [HttpDelete("schedules")]
    public async Task<IActionResult> DeleteSchedule([FromBody] DeleteLoanScheduleDto request) =>
        BoolResult(await _service.DeleteScheduleAsync(request), "Loan schedule deleted successfully.");

    // ==================== LOAN SCHEDULE HISTORY AND STATUS (9) ====================

    [HttpGet("{loanId:int}/schedules")]
    public async Task<IActionResult> GetSchedulesByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetSchedulesByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/with-repayments")]
    public async Task<IActionResult> GetLoanSchedulesWithRepayments(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetLoanSchedulesWithRepaymentsAsync(loanId)));

    [HttpGet("schedules/{loanScheduleId:int}/with-repayments")]
    public async Task<IActionResult> GetScheduleWithRepayments(int loanScheduleId) =>
        Result(await _service.GetScheduleWithRepaymentsAsync(loanScheduleId));

    [HttpGet("schedules/status/{status}")]
    public async Task<IActionResult> GetSchedulesByStatus(string status) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetSchedulesByStatusAsync(status)));

    [HttpGet("{loanId:int}/schedules/pending")]
    public async Task<IActionResult> GetPendingSchedulesByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetPendingSchedulesByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/partially-paid")]
    public async Task<IActionResult> GetPartiallyPaidSchedulesByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetPartiallyPaidSchedulesByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/paid")]
    public async Task<IActionResult> GetPaidSchedulesByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetPaidSchedulesByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/overdue")]
    public async Task<IActionResult> GetOverdueSchedulesByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetOverdueSchedulesByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/waived")]
    public async Task<IActionResult> GetWaivedSchedulesByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetWaivedSchedulesByLoanIdAsync(loanId)));

    // ==================== SCHEDULE DUE DATE AND PAYMENT PROGRESS (6) ====================

    [HttpGet("schedules/due/date-range")]
    public async Task<IActionResult> GetSchedulesDueWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetSchedulesDueWithinRangeAsync(startDate, endDate)));
    }

    [HttpGet("schedules/unpaid/due-before")]
    public async Task<IActionResult> GetUnpaidSchedulesDueBefore([FromQuery] DateTime date) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetUnpaidSchedulesDueBeforeAsync(date)));

    [HttpGet("{loanId:int}/schedules/due-on-date")]
    public async Task<IActionResult> GetSchedulesDueOnDate(int loanId, [FromQuery] DateTime dueDate) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetSchedulesDueOnDateAsync(loanId, dueDate)));

    [HttpGet("schedules/overdue")]
    public async Task<IActionResult> GetAllOverdueSchedules() =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetAllOverdueSchedulesAsync()));

    [HttpGet("schedules/paid/date-range")]
    public async Task<IActionResult> GetSchedulesPaidWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetSchedulesPaidWithinRangeAsync(startDate, endDate)));
    }

    [HttpGet("schedules/outstanding/above")]
    public async Task<IActionResult> GetSchedulesWithOutstandingAmountAbove([FromQuery] decimal minimumAmount) =>
        Ok(ApiResponse<IEnumerable<LoanScheduleDto>>.SuccessResponse(await _service.GetSchedulesWithOutstandingAmountAboveAsync(minimumAmount)));

    // ==================== SCHEDULE FINANCIAL REPORTING (8) ====================

    [HttpGet("{loanId:int}/schedules/totals/principal-due")]
    public async Task<IActionResult> GetTotalPrincipalDueByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalPrincipalDueByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/totals/interest-due")]
    public async Task<IActionResult> GetTotalInterestDueByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalInterestDueByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/totals/principal-paid")]
    public async Task<IActionResult> GetTotalPrincipalPaidByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalPrincipalPaidByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/totals/interest-paid")]
    public async Task<IActionResult> GetTotalInterestPaidByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalInterestPaidByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/totals/penalty-accrued")]
    public async Task<IActionResult> GetTotalPenaltyAccruedByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalPenaltyAccruedByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/totals/penalty-paid")]
    public async Task<IActionResult> GetTotalPenaltyPaidByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalPenaltyPaidByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/schedules/count/status/{status}")]
    public async Task<IActionResult> CountSchedulesByLoanAndStatus(int loanId, string status) =>
        Ok(ApiResponse<int>.SuccessResponse(await _service.CountSchedulesByLoanAndStatusAsync(loanId, status)));

    [HttpGet("{loanId:int}/schedules/count/unpaid")]
    public async Task<IActionResult> CountUnpaidSchedulesByLoanId(int loanId) =>
        Ok(ApiResponse<int>.SuccessResponse(await _service.CountUnpaidSchedulesByLoanIdAsync(loanId)));

    // ==================== LOAN REPAYMENT IDENTIFICATION AND CRUD (8) ====================

    [HttpGet("repayments")]
    public async Task<IActionResult> GetAllRepayments() =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetAllRepaymentsAsync()));

    [HttpGet("repayments/{loanRepaymentId:int}")]
    public async Task<IActionResult> GetRepaymentById(int loanRepaymentId) =>
        Result(await _service.GetRepaymentByIdAsync(loanRepaymentId));

    [HttpGet("repayments/reference/{repaymentReference}")]
    public async Task<IActionResult> GetByRepaymentReference(string repaymentReference) =>
        Result(await _service.GetByRepaymentReferenceAsync(repaymentReference));

    [HttpGet("repayments/{loanRepaymentId:int}/details")]
    public async Task<IActionResult> GetRepaymentDetails(int loanRepaymentId) =>
        Result(await _service.GetRepaymentDetailsAsync(loanRepaymentId));

    [HttpGet("repayments/exists/reference/{repaymentReference}")]
    public async Task<IActionResult> ExistsByRepaymentReference(string repaymentReference) =>
        Ok(ApiResponse<bool>.SuccessResponse(await _service.ExistsByRepaymentReferenceAsync(repaymentReference)));

    [HttpPost("repayments")]
    public async Task<IActionResult> CreateRepayment([FromBody] CreateLoanRepaymentDto request)
    {
        var data = await _service.CreateRepaymentAsync(request);
        return CreatedAtAction(nameof(GetRepaymentById), new { loanRepaymentId = data.LoanRepaymentId },
            ApiResponse<LoanRepaymentDto>.SuccessResponse(data, "Loan repayment created successfully.", 201));
    }

    [HttpPut("repayments")]
    public async Task<IActionResult> UpdateRepayment([FromBody] UpdateLoanRepaymentDto request) =>
        Result(await _service.UpdateRepaymentAsync(request));

    [HttpDelete("repayments")]
    public async Task<IActionResult> DeleteRepayment([FromBody] DeleteLoanRepaymentDto request) =>
        BoolResult(await _service.DeleteRepaymentAsync(request), "Loan repayment deleted successfully.");

    // ==================== REPAYMENT HISTORY AND STATUS (9) ====================

    [HttpGet("{loanId:int}/repayments")]
    public async Task<IActionResult> GetRepaymentsByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetRepaymentsByLoanIdAsync(loanId)));

    [HttpGet("schedules/{loanScheduleId:int}/repayments")]
    public async Task<IActionResult> GetRepaymentsByLoanScheduleId(int loanScheduleId) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetRepaymentsByLoanScheduleIdAsync(loanScheduleId)));

    [HttpGet("repayments/transaction/{transactionId:int}")]
    public async Task<IActionResult> GetRepaymentByTransactionId(int transactionId) =>
        Result(await _service.GetRepaymentByTransactionIdAsync(transactionId));

    [HttpGet("{loanId:int}/repayments/with-transactions")]
    public async Task<IActionResult> GetLoanRepaymentsWithTransactions(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetLoanRepaymentsWithTransactionsAsync(loanId)));

    [HttpGet("repayments/status/{status}")]
    public async Task<IActionResult> GetRepaymentsByStatus(string status) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetRepaymentsByStatusAsync(status)));

    [HttpGet("{loanId:int}/repayments/successful")]
    public async Task<IActionResult> GetSuccessfulRepaymentsByLoanId(int loanId) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetSuccessfulRepaymentsByLoanIdAsync(loanId)));

    [HttpGet("repayments/pending")]
    public async Task<IActionResult> GetPendingRepayments() =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetPendingRepaymentsAsync()));

    [HttpGet("repayments/failed")]
    public async Task<IActionResult> GetFailedRepayments() =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetFailedRepaymentsAsync()));

    [HttpGet("repayments/reversed")]
    public async Task<IActionResult> GetReversedRepayments() =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetReversedRepaymentsAsync()));

    // ==================== REPAYMENT TYPE, MODE AND DATE REPORTING (5) ====================

    [HttpGet("repayments/type/{repaymentType}")]
    public async Task<IActionResult> GetRepaymentsByType(string repaymentType) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetRepaymentsByTypeAsync(repaymentType)));

    [HttpGet("repayments/payment-mode/{paymentMode}")]
    public async Task<IActionResult> GetRepaymentsByPaymentMode(string paymentMode) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetRepaymentsByPaymentModeAsync(paymentMode)));

    [HttpGet("repayments/date-range")]
    public async Task<IActionResult> GetRepaymentsWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetRepaymentsWithinRangeAsync(startDate, endDate)));
    }

    [HttpGet("repayments/successful/date-range")]
    public async Task<IActionResult> GetSuccessfulRepaymentsWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetSuccessfulRepaymentsWithinRangeAsync(startDate, endDate)));
    }

    [HttpGet("repayments/recent")]
    public async Task<IActionResult> GetRecentRepayments([FromQuery] DateTime fromDate) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.GetRecentRepaymentsAsync(fromDate)));

    // ==================== REPAYMENT FINANCIAL REPORTING AND SEARCH (7) ====================

    [HttpGet("{loanId:int}/repayments/totals/successful")]
    public async Task<IActionResult> GetTotalSuccessfulAmountByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalSuccessfulAmountByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/repayments/totals/principal")]
    public async Task<IActionResult> GetTotalPrincipalCollectedByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalPrincipalCollectedByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/repayments/totals/interest")]
    public async Task<IActionResult> GetTotalInterestCollectedByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalInterestCollectedByLoanIdAsync(loanId)));

    [HttpGet("{loanId:int}/repayments/totals/penalty")]
    public async Task<IActionResult> GetTotalPenaltyCollectedByLoanId(int loanId) =>
        Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalPenaltyCollectedByLoanIdAsync(loanId)));

    [HttpGet("repayments/totals/successful/date-range")]
    public async Task<IActionResult> GetTotalSuccessfulCollectionsWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate)) return InvalidRange();
        return Ok(ApiResponse<decimal>.SuccessResponse(await _service.GetTotalSuccessfulCollectionsWithinRangeAsync(startDate, endDate)));
    }

    [HttpGet("repayments/search")]
    public async Task<IActionResult> SearchRepayments([FromQuery] string searchTerm) =>
        Ok(ApiResponse<IEnumerable<LoanRepaymentDto>>.SuccessResponse(await _service.SearchRepaymentsAsync(searchTerm)));

    [HttpGet("{loanId:int}/repayments/count/successful")]
    public async Task<IActionResult> CountSuccessfulRepaymentsByLoanId(int loanId) =>
        Ok(ApiResponse<int>.SuccessResponse(await _service.CountSuccessfulRepaymentsByLoanIdAsync(loanId)));

    // ==================== RESPONSE HELPERS ====================

    private IActionResult Result<T>(T? data) where T : class =>
        data is null
            ? NotFound(ApiResponse<T>.FailureResponse("Requested record was not found.", 404))
            : Ok(ApiResponse<T>.SuccessResponse(data));

    private IActionResult BoolResult(bool result, string successMessage) =>
        result
            ? Ok(ApiResponse<object>.SuccessResponse(null, successMessage))
            : Conflict(ApiResponse<object>.FailureResponse("The requested operation could not be completed.", 409));

    private static bool ValidRange(DateTime startDate, DateTime endDate) =>
        startDate != default && endDate != default && endDate >= startDate;

    private IActionResult InvalidRange() =>
        BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));
}
