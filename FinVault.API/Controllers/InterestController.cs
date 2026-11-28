
using FinVault.Application.DTOs.InterestCalculationDtos;
using FinVault.Application.DTOs.InterestRateDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InterestController : ControllerBase
{
    private readonly IInterestService _interestService;

    public InterestController(IInterestService interestService)
    {
        _interestService = interestService;
    }

    // ==================== INTEREST RATE IDENTIFICATION AND CRUD ====================

    [HttpGet("rates")]
    public async Task<IActionResult> GetAllRates()
    {
        var data = await _interestService.GetAllRatesAsync();
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/{interestRateId:int}")]
    public async Task<IActionResult> GetRateById(int interestRateId)
    {
        var data = await _interestService.GetRateByIdAsync(interestRateId);
        return data is null
            ? NotFound(ApiResponse<InterestRateDto>.FailureResponse("Interest rate not found.", 404))
            : Ok(ApiResponse<InterestRateDto>.SuccessResponse(data));
    }

    [HttpGet("rates/{interestRateId:int}/details")]
    public async Task<IActionResult> GetRateDetails(int interestRateId)
    {
        var data = await _interestService.GetRateDetailsAsync(interestRateId);
        return data is null
            ? NotFound(ApiResponse<InterestRateDto>.FailureResponse("Interest rate details not found.", 404))
            : Ok(ApiResponse<InterestRateDto>.SuccessResponse(data));
    }

    [HttpGet("rates/name")]
    public async Task<IActionResult> GetRatesByName([FromQuery] string rateName)
    {
        var data = await _interestService.GetRatesByNameAsync(rateName);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpPost("rates")]
    public async Task<IActionResult> CreateRate([FromBody] CreateInterestRateDto request)
    {
        var data = await _interestService.CreateRateAsync(request);
        return CreatedAtAction(
            nameof(GetRateById),
            new { interestRateId = data.InterestRateId },
            ApiResponse<InterestRateDto>.SuccessResponse(data, "Interest rate created successfully.", 201));
    }

    [HttpPut("rates")]
    public async Task<IActionResult> UpdateRate([FromBody] UpdateInterestRateDto request)
    {
        var data = await _interestService.UpdateRateAsync(request);
        return data is null
            ? NotFound(ApiResponse<InterestRateDto>.FailureResponse("Interest rate not found or could not be updated.", 404))
            : Ok(ApiResponse<InterestRateDto>.SuccessResponse(data, "Interest rate updated successfully."));
    }

    [HttpDelete("rates")]
    public async Task<IActionResult> DeleteRate([FromBody] DeleteInterestRateDto request)
    {
        var result = await _interestService.DeleteRateAsync(request);
        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Interest rate deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse("Interest rate could not be deleted. It may be referenced by financial records.", 409));
    }

    // ==================== PRODUCT CATEGORY AND TYPE ====================

    [HttpGet("rates/product-category/{productCategory}")]
    public async Task<IActionResult> GetRatesByProductCategory(string productCategory)
    {
        var data = await _interestService.GetRatesByProductCategoryAsync(productCategory);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/loan-type/{loanType}")]
    public async Task<IActionResult> GetRatesByLoanType(string loanType)
    {
        var data = await _interestService.GetRatesByLoanTypeAsync(loanType);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/account-type/{accountTypeId:int}")]
    public async Task<IActionResult> GetRatesByAccountTypeId(int accountTypeId)
    {
        var data = await _interestService.GetRatesByAccountTypeIdAsync(accountTypeId);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/compounding-frequency/{compoundingFrequency}")]
    public async Task<IActionResult> GetRatesByCompoundingFrequency(string compoundingFrequency)
    {
        var data = await _interestService.GetRatesByCompoundingFrequencyAsync(compoundingFrequency);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    // ==================== RATE EFFECTIVE DATE MANAGEMENT ====================

    [HttpGet("rates/effective")]
    public async Task<IActionResult> GetEffectiveRates([FromQuery] DateTime effectiveDate)
    {
        var data = await _interestService.GetEffectiveRatesAsync(effectiveDate);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/upcoming")]
    public async Task<IActionResult> GetUpcomingRates(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));

        var data = await _interestService.GetUpcomingRatesAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/active")]
    public async Task<IActionResult> GetActiveRates()
    {
        var data = await _interestService.GetActiveRatesAsync();
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/inactive")]
    public async Task<IActionResult> GetInactiveRates()
    {
        var data = await _interestService.GetInactiveRatesAsync();
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/expired")]
    public async Task<IActionResult> GetExpiredRates([FromQuery] DateTime currentDate)
    {
        var data = await _interestService.GetExpiredRatesAsync(currentDate);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    // ==================== AMOUNT AND TENURE APPLICABILITY ====================

    [HttpGet("rates/by-amount")]
    public async Task<IActionResult> GetRatesByAmount(
        [FromQuery] decimal amount,
        [FromQuery] DateTime effectiveDate)
    {
        var data = await _interestService.GetRatesByAmountAsync(amount, effectiveDate);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/by-tenure")]
    public async Task<IActionResult> GetRatesByTenure(
        [FromQuery] int tenureMonths,
        [FromQuery] DateTime effectiveDate)
    {
        var data = await _interestService.GetRatesByTenureAsync(tenureMonths, effectiveDate);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/applicable")]
    public async Task<IActionResult> GetApplicableRates(
        [FromQuery] string productCategory,
        [FromQuery] decimal amount,
        [FromQuery] int tenureMonths,
        [FromQuery] DateTime effectiveDate)
    {
        var data = await _interestService.GetApplicableRatesAsync(
            productCategory, amount, tenureMonths, effectiveDate);

        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    // ==================== SPECIAL RATE CONFIGURATIONS ====================

    [HttpGet("rates/senior-citizen")]
    public async Task<IActionResult> GetSeniorCitizenRates([FromQuery] DateTime effectiveDate)
    {
        var data = await _interestService.GetSeniorCitizenRatesAsync(effectiveDate);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/benchmark-linked")]
    public async Task<IActionResult> GetBenchmarkLinkedRates()
    {
        var data = await _interestService.GetBenchmarkLinkedRatesAsync();
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/with-loans")]
    public async Task<IActionResult> GetRatesWithLoans()
    {
        var data = await _interestService.GetRatesWithLoansAsync();
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/with-calculations")]
    public async Task<IActionResult> GetRatesWithInterestCalculations()
    {
        var data = await _interestService.GetRatesWithInterestCalculationsAsync();
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/created-by/{userId:int}")]
    public async Task<IActionResult> GetRatesCreatedByUser(int userId)
    {
        var data = await _interestService.GetRatesCreatedByUserAsync(userId);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    [HttpGet("rates/search")]
    public async Task<IActionResult> SearchInterestRates([FromQuery] string searchTerm)
    {
        var data = await _interestService.SearchInterestRatesAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<InterestRateDto>>.SuccessResponse(data));
    }

    // ==================== INTEREST CALCULATION IDENTIFICATION ====================

    [HttpGet("calculations")]
    public async Task<IActionResult> GetAllCalculations()
    {
        var data = await _interestService.GetAllCalculationsAsync();
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/{interestCalculationId:int}")]
    public async Task<IActionResult> GetCalculationById(int interestCalculationId)
    {
        var data = await _interestService.GetCalculationByIdAsync(interestCalculationId);
        return data is null
            ? NotFound(ApiResponse<InterestCalculationDto>.FailureResponse("Interest calculation not found.", 404))
            : Ok(ApiResponse<InterestCalculationDto>.SuccessResponse(data));
    }

    [HttpGet("calculations/{interestCalculationId:int}/details")]
    public async Task<IActionResult> GetCalculationDetails(int interestCalculationId)
    {
        var data = await _interestService.GetCalculationDetailsAsync(interestCalculationId);
        return data is null
            ? NotFound(ApiResponse<InterestCalculationDto>.FailureResponse("Interest calculation details not found.", 404))
            : Ok(ApiResponse<InterestCalculationDto>.SuccessResponse(data));
    }

    [HttpPost("calculations")]
    public async Task<IActionResult> CreateCalculation([FromBody] CreateInterestCalculationDto request)
    {
        var data = await _interestService.CreateCalculationAsync(request);
        return CreatedAtAction(
            nameof(GetCalculationById),
            new { interestCalculationId = data.InterestCalculationId },
            ApiResponse<InterestCalculationDto>.SuccessResponse(data, "Interest calculation created successfully.", 201));
    }

    [HttpPut("calculations")]
    public async Task<IActionResult> UpdateCalculation([FromBody] UpdateInterestCalculationDto request)
    {
        var data = await _interestService.UpdateCalculationAsync(request);
        return data is null
            ? NotFound(ApiResponse<InterestCalculationDto>.FailureResponse("Interest calculation not found or could not be updated.", 404))
            : Ok(ApiResponse<InterestCalculationDto>.SuccessResponse(data, "Interest calculation updated successfully."));
    }

    [HttpDelete("calculations")]
    public async Task<IActionResult> DeleteCalculation([FromBody] DeleteInterestCalculationDto request)
    {
        var result = await _interestService.DeleteCalculationAsync(request);
        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Interest calculation deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse("Interest calculation could not be deleted.", 409));
    }

    // ==================== ACCOUNT, LOAN AND RATE CALCULATIONS ====================

    [HttpGet("calculations/account/{accountId:int}")]
    public async Task<IActionResult> GetCalculationsByAccountId(int accountId)
    {
        var data = await _interestService.GetCalculationsByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/loan/{loanId:int}")]
    public async Task<IActionResult> GetCalculationsByLoanId(int loanId)
    {
        var data = await _interestService.GetCalculationsByLoanIdAsync(loanId);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/rate/{interestRateId:int}")]
    public async Task<IActionResult> GetCalculationsByInterestRateId(int interestRateId)
    {
        var data = await _interestService.GetCalculationsByInterestRateIdAsync(interestRateId);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/account/{accountId:int}/latest")]
    public async Task<IActionResult> GetLatestCalculationsByAccountId(
        int accountId,
        [FromQuery] int count = 10)
    {
        var data = await _interestService.GetLatestCalculationsByAccountIdAsync(accountId, count);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/loan/{loanId:int}/latest")]
    public async Task<IActionResult> GetLatestCalculationsByLoanId(
        int loanId,
        [FromQuery] int count = 10)
    {
        var data = await _interestService.GetLatestCalculationsByLoanIdAsync(loanId, count);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    // ==================== CALCULATION TYPE AND STATUS ====================

    [HttpGet("calculations/type/{calculationType}")]
    public async Task<IActionResult> GetCalculationsByType(string calculationType)
    {
        var data = await _interestService.GetCalculationsByTypeAsync(calculationType);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/status/{status}")]
    public async Task<IActionResult> GetCalculationsByStatus(string status)
    {
        var data = await _interestService.GetCalculationsByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/pending")]
    public async Task<IActionResult> GetPendingCalculations()
    {
        var data = await _interestService.GetPendingCalculationsAsync();
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/posted")]
    public async Task<IActionResult> GetPostedCalculations()
    {
        var data = await _interestService.GetPostedCalculationsAsync();
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/reversed")]
    public async Task<IActionResult> GetReversedCalculations()
    {
        var data = await _interestService.GetReversedCalculationsAsync();
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    // ==================== ACCOUNTING POSTING AND PERIOD MANAGEMENT ====================

    [HttpGet("calculations/journal-entry/{journalEntryId:int}")]
    public async Task<IActionResult> GetCalculationByJournalEntryId(int journalEntryId)
    {
        var data = await _interestService.GetCalculationByJournalEntryIdAsync(journalEntryId);
        return data is null
            ? NotFound(ApiResponse<InterestCalculationDto>.FailureResponse("Interest calculation not found.", 404))
            : Ok(ApiResponse<InterestCalculationDto>.SuccessResponse(data));
    }

    [HttpGet("calculations/transaction/{transactionId:int}")]
    public async Task<IActionResult> GetCalculationByTransactionId(int transactionId)
    {
        var data = await _interestService.GetCalculationByTransactionIdAsync(transactionId);
        return data is null
            ? NotFound(ApiResponse<InterestCalculationDto>.FailureResponse("Interest calculation not found.", 404))
            : Ok(ApiResponse<InterestCalculationDto>.SuccessResponse(data));
    }

    [HttpGet("calculations/posted-without-journal-entry")]
    public async Task<IActionResult> GetPostedCalculationsWithoutJournalEntry()
    {
        var data = await _interestService.GetPostedCalculationsWithoutJournalEntryAsync();
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/period-range")]
    public async Task<IActionResult> GetCalculationsByPeriodRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid period range is required.", 400));

        var data = await _interestService.GetCalculationsByPeriodRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/date-range")]
    public async Task<IActionResult> GetCalculationsWithinDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));

        var data = await _interestService.GetCalculationsWithinDateRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    // ==================== AMOUNT, TAX AND REPORTING ====================

    [HttpGet("calculations/interest-amount-range")]
    public async Task<IActionResult> GetCalculationsByInterestAmountRange(
        [FromQuery] decimal minimumAmount,
        [FromQuery] decimal maximumAmount)
    {
        if (minimumAmount < 0 || maximumAmount < minimumAmount)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid interest amount range is required.", 400));

        var data = await _interestService.GetCalculationsByInterestAmountRangeAsync(
            minimumAmount, maximumAmount);

        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/with-tds")]
    public async Task<IActionResult> GetCalculationsWithTds()
    {
        var data = await _interestService.GetCalculationsWithTdsAsync();
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    [HttpGet("calculations/account/{accountId:int}/total-interest")]
    public async Task<IActionResult> GetTotalInterestByAccountId(int accountId)
    {
        var total = await _interestService.GetTotalInterestByAccountIdAsync(accountId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("calculations/loan/{loanId:int}/total-interest")]
    public async Task<IActionResult> GetTotalInterestByLoanId(int loanId)
    {
        var total = await _interestService.GetTotalInterestByLoanIdAsync(loanId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("calculations/created-by/{userId:int}")]
    public async Task<IActionResult> GetCalculationsCreatedByUserId(int userId)
    {
        var data = await _interestService.GetCalculationsCreatedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<InterestCalculationDto>>.SuccessResponse(data));
    }

    // ==================== DUPLICATE CALCULATION VALIDATION ====================

    [HttpGet("calculations/exists-for-period")]
    public async Task<IActionResult> ExistsForPeriod(
        [FromQuery] int? accountId,
        [FromQuery] int? loanId,
        [FromQuery] DateTime periodStart,
        [FromQuery] DateTime periodEnd,
        [FromQuery] string calculationType)
    {
        if (accountId.HasValue == loanId.HasValue)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Specify exactly one of accountId or loanId.", 400));

        if (periodStart == default || periodEnd == default || periodEnd < periodStart)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid calculation period is required.", 400));

        var result = await _interestService.ExistsForPeriodAsync(
            accountId, loanId, periodStart, periodEnd, calculationType);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
