
using FinVault.Application.DTOs.RiskAssessmentDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RiskAssessmentController : ControllerBase
{
    private readonly IRiskAssessmentService _service;

    public RiskAssessmentController(IRiskAssessmentService service)
    {
        _service = service;
    }

    // ==================== IDENTIFICATION AND CRUD (7) ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("{riskAssessmentId:int}")]
    public async Task<IActionResult> GetById(int riskAssessmentId) =>
        Result(await _service.GetByIdAsync(riskAssessmentId));

    [HttpGet("{riskAssessmentId:int}/details")]
    public async Task<IActionResult> GetRiskAssessmentDetails(int riskAssessmentId) =>
        Result(await _service.GetRiskAssessmentDetailsAsync(riskAssessmentId));

    [HttpGet("{riskAssessmentId:int}/subject")]
    public async Task<IActionResult> GetByIdWithSubject(int riskAssessmentId) =>
        Result(await _service.GetByIdWithSubjectAsync(riskAssessmentId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRiskAssessmentDto request)
    {
        try
        {
            var data = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { riskAssessmentId = data.RiskAssessmentId },
                ApiResponse<RiskAssessmentDto>.SuccessResponse(
                    data, "Risk assessment created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRiskAssessmentDto request)
    {
        try
        {
            var data = await _service.UpdateAsync(request);
            return Result(data);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteRiskAssessmentDto request)
    {
        try
        {
            var result = await _service.DeleteAsync(request);

            return result
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Risk assessment deleted successfully."))
                : NotFound(ApiResponse<object>.FailureResponse("Risk assessment not found.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    // ==================== CUSTOMER ASSESSMENTS (3) ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var data = await _service.GetByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/latest")]
    public async Task<IActionResult> GetLatestByCustomerId(int customerId) =>
        Result(await _service.GetLatestByCustomerIdAsync(customerId));

    [HttpGet("customer/{customerId:int}/date-range")]
    public async Task<IActionResult> GetCustomerAssessmentsWithinRange(
        int customerId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        return await ExecuteList(() =>
            _service.GetCustomerAssessmentsWithinRangeAsync(customerId, startDate, endDate));
    }

    // ==================== ACCOUNT ASSESSMENTS (2) ====================

    [HttpGet("account/{accountId:int}")]
    public async Task<IActionResult> GetByAccountId(int accountId)
    {
        var data = await _service.GetByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/latest")]
    public async Task<IActionResult> GetLatestByAccountId(int accountId) =>
        Result(await _service.GetLatestByAccountIdAsync(accountId));

    // ==================== TRANSACTION ASSESSMENTS (2) ====================

    [HttpGet("transaction/{transactionId:int}")]
    public async Task<IActionResult> GetByTransactionId(int transactionId)
    {
        var data = await _service.GetByTransactionIdAsync(transactionId);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("transaction/{transactionId:int}/latest")]
    public async Task<IActionResult> GetLatestByTransactionId(int transactionId) =>
        Result(await _service.GetLatestByTransactionIdAsync(transactionId));

    // ==================== TYPE, RISK LEVEL AND DECISION (5) ====================

    [HttpGet("type/{assessmentType}")]
    public async Task<IActionResult> GetByAssessmentType(string assessmentType)
    {
        var data = await _service.GetByAssessmentTypeAsync(assessmentType);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("risk-level/{riskLevel}")]
    public async Task<IActionResult> GetByRiskLevel(string riskLevel)
    {
        var data = await _service.GetByRiskLevelAsync(riskLevel);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("decision/{decision}")]
    public async Task<IActionResult> GetByDecision(string decision)
    {
        var data = await _service.GetByDecisionAsync(decision);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("high-critical")]
    public async Task<IActionResult> GetHighAndCriticalRiskAssessments()
    {
        var data = await _service.GetHighAndCriticalRiskAssessmentsAsync();
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("minimum-score")]
    public async Task<IActionResult> GetByMinimumRiskScore([FromQuery] decimal minimumRiskScore)
    {
        var data = await _service.GetByMinimumRiskScoreAsync(minimumRiskScore);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    // ==================== MODEL VERSION AND VALIDITY (3) ====================

    [HttpGet("model-version/{modelVersion}")]
    public async Task<IActionResult> GetByModelVersion(string modelVersion)
    {
        var data = await _service.GetByModelVersionAsync(modelVersion);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("non-expired")]
    public async Task<IActionResult> GetNonExpiredAssessments([FromQuery] DateTime currentDateTime)
    {
        var data = await _service.GetNonExpiredAssessmentsAsync(currentDateTime);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredAssessments([FromQuery] DateTime currentDateTime)
    {
        var data = await _service.GetExpiredAssessmentsAsync(currentDateTime);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    // ==================== DATE AND AUDIT (3) ====================

    [HttpGet("assessed/date-range")]
    public async Task<IActionResult> GetAssessedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        return await ExecuteList(() =>
            _service.GetAssessedWithinRangeAsync(startDate, endDate));
    }

    [HttpGet("created-by/{userId:int}")]
    public async Task<IActionResult> GetByCreatedBy(int userId)
    {
        var data = await _service.GetByCreatedByAsync(userId);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("updated-by/{userId:int}")]
    public async Task<IActionResult> GetByUpdatedBy(int userId)
    {
        var data = await _service.GetByUpdatedByAsync(userId);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    // ==================== FRAUD ALERTS AND SEARCH (3) ====================

    [HttpGet("with-fraud-alerts")]
    public async Task<IActionResult> GetAssessmentsWithFraudAlerts()
    {
        var data = await _service.GetAssessmentsWithFraudAlertsAsync();
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    [HttpGet("{riskAssessmentId:int}/fraud-alerts")]
    public async Task<IActionResult> GetAssessmentWithFraudAlerts(int riskAssessmentId) =>
        Result(await _service.GetAssessmentWithFraudAlertsAsync(riskAssessmentId));

    [HttpGet("search")]
    public async Task<IActionResult> SearchRiskAssessments([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse("Search term is required.", 400));

        var data = await _service.SearchRiskAssessmentsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
    }

    // ==================== REPORTING (2) ====================

    [HttpGet("reports/count-by-risk-level")]
    public async Task<IActionResult> CountByRiskLevelWithinRange(
        [FromQuery] string riskLevel,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        try
        {
            var count = await _service.CountByRiskLevelWithinRangeAsync(
                riskLevel, startDate, endDate);

            return Ok(ApiResponse<int>.SuccessResponse(count));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("reports/average-score")]
    public async Task<IActionResult> GetAverageRiskScoreByAssessmentType(
        [FromQuery] string assessmentType)
    {
        try
        {
            var average = await _service.GetAverageRiskScoreByAssessmentTypeAsync(assessmentType);
            return Ok(ApiResponse<decimal>.SuccessResponse(average));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    // ==================== RESPONSE HELPERS ====================

    private IActionResult Result(RiskAssessmentDto? data)
    {
        return data is null
            ? NotFound(ApiResponse<RiskAssessmentDto>.FailureResponse(
                "Risk assessment not found.", 404))
            : Ok(ApiResponse<RiskAssessmentDto>.SuccessResponse(data));
    }

    private async Task<IActionResult> ExecuteList(
        Func<Task<IEnumerable<RiskAssessmentDto>>> operation)
    {
        try
        {
            var data = await operation();
            return Ok(ApiResponse<IEnumerable<RiskAssessmentDto>>.SuccessResponse(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    private static bool ValidRange(DateTime startDate, DateTime endDate) =>
        startDate != default &&
        endDate != default &&
        startDate <= endDate;

    private IActionResult InvalidRange() =>
        BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));
}
