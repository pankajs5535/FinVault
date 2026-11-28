
using FinVault.Application.DTOs.FraudAlertDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class FraudAlertController : ControllerBase
{
    private readonly IFraudAlertService _fraudAlertService;

    public FraudAlertController(IFraudAlertService fraudAlertService)
    {
        _fraudAlertService = fraudAlertService;
    }

    // ==================== ALERT IDENTIFICATION AND CRUD (8) ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _fraudAlertService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("{fraudAlertId:int}")]
    public async Task<IActionResult> GetById(int fraudAlertId)
    {
        var data = await _fraudAlertService.GetByIdAsync(fraudAlertId);
        return Result(data);
    }

    [HttpGet("number/{alertNumber}")]
    public async Task<IActionResult> GetByAlertNumber(string alertNumber)
    {
        var data = await _fraudAlertService.GetByAlertNumberAsync(alertNumber);
        return Result(data);
    }

    [HttpGet("{fraudAlertId:int}/details")]
    public async Task<IActionResult> GetFraudAlertDetails(int fraudAlertId)
    {
        var data = await _fraudAlertService.GetFraudAlertDetailsAsync(fraudAlertId);
        return Result(data);
    }

    [HttpGet("exists/number/{alertNumber}")]
    public async Task<IActionResult> ExistsByAlertNumber(string alertNumber)
    {
        var result = await _fraudAlertService.ExistsByAlertNumberAsync(alertNumber);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFraudAlertDto request)
    {
        var data = await _fraudAlertService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { fraudAlertId = data.FraudAlertId },
            ApiResponse<FraudAlertDto>.SuccessResponse(
                data, "Fraud alert created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateFraudAlertDto request)
    {
        var data = await _fraudAlertService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<FraudAlertDto>.FailureResponse(
                "Fraud alert not found.", 404))
            : Ok(ApiResponse<FraudAlertDto>.SuccessResponse(
                data, "Fraud alert updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteFraudAlertDto request)
    {
        var result = await _fraudAlertService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Fraud alert deleted successfully."))
            : NotFound(ApiResponse<object>.FailureResponse(
                "Fraud alert not found.", 404));
    }

    // ==================== ALERT STATUS AND SEVERITY (4) ====================

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _fraudAlertService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("severity/{severity}")]
    public async Task<IActionResult> GetBySeverity(string severity)
    {
        var data = await _fraudAlertService.GetBySeverityAsync(severity);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("score/above")]
    public async Task<IActionResult> GetAlertsAboveScore([FromQuery] decimal minimumScore)
    {
        var data = await _fraudAlertService.GetAlertsAboveScoreAsync(minimumScore);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveFraudAlerts()
    {
        var data = await _fraudAlertService.GetActiveFraudAlertsAsync();
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    // ==================== BANKING ENTITY ASSOCIATIONS (6) ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var data = await _fraudAlertService.GetByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}")]
    public async Task<IActionResult> GetByAccountId(int accountId)
    {
        var data = await _fraudAlertService.GetByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("transaction/{transactionId:int}")]
    public async Task<IActionResult> GetByTransactionId(int transactionId)
    {
        var data = await _fraudAlertService.GetByTransactionIdAsync(transactionId);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetByCardId(int cardId)
    {
        var data = await _fraudAlertService.GetByCardIdAsync(cardId);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("rule/{fraudRuleId:int}")]
    public async Task<IActionResult> GetByFraudRuleId(int fraudRuleId)
    {
        var data = await _fraudAlertService.GetByFraudRuleIdAsync(fraudRuleId);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("risk-assessment/{riskAssessmentId:int}")]
    public async Task<IActionResult> GetByRiskAssessmentId(int riskAssessmentId)
    {
        var data = await _fraudAlertService.GetByRiskAssessmentIdAsync(riskAssessmentId);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    // ==================== INVESTIGATION ASSIGNMENT (2) ====================

    [HttpGet("reviewed-by/{employeeId:int}")]
    public async Task<IActionResult> GetByReviewedByEmployeeId(int employeeId)
    {
        var data = await _fraudAlertService.GetByReviewedByEmployeeIdAsync(employeeId);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("unassigned")]
    public async Task<IActionResult> GetUnassignedFraudAlerts()
    {
        var data = await _fraudAlertService.GetUnassignedFraudAlertsAsync();
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    // ==================== INVESTIGATION OUTCOMES (3) ====================

    [HttpGet("confirmed-fraud")]
    public async Task<IActionResult> GetConfirmedFraudAlerts()
    {
        var data = await _fraudAlertService.GetConfirmedFraudAlertsAsync();
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("false-positive")]
    public async Task<IActionResult> GetFalsePositiveAlerts()
    {
        var data = await _fraudAlertService.GetFalsePositiveAlertsAsync();
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("with-compliance-records")]
    public async Task<IActionResult> GetAlertsWithComplianceRecords()
    {
        var data = await _fraudAlertService.GetAlertsWithComplianceRecordsAsync();
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    // ==================== ALERT TIMELINE AND SEARCH (3) ====================

    [HttpGet("raised/date-range")]
    public async Task<IActionResult> GetAlertsRaisedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _fraudAlertService.GetAlertsRaisedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("reviewed/date-range")]
    public async Task<IActionResult> GetAlertsReviewedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _fraudAlertService.GetAlertsReviewedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchFraudAlerts([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        var data = await _fraudAlertService.SearchFraudAlertsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<FraudAlertDto>>.SuccessResponse(data));
    }

    // ==================== REPORTING (1) ====================

    [HttpGet("customer/{customerId:int}/count")]
    public async Task<IActionResult> CountAlertsByCustomerId(int customerId)
    {
        var count = await _fraudAlertService.CountAlertsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== RESPONSE HELPERS ====================

    private IActionResult Result(FraudAlertDto? data)
    {
        return data is null
            ? NotFound(ApiResponse<FraudAlertDto>.FailureResponse(
                "Fraud alert not found.", 404))
            : Ok(ApiResponse<FraudAlertDto>.SuccessResponse(data));
    }

    private static bool ValidRange(DateTime startDate, DateTime endDate)
    {
        return startDate != default &&
               endDate != default &&
               endDate >= startDate;
    }
}
