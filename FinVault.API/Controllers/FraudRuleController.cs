
using FinVault.Application.DTOs.FraudRuleDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class FraudRuleController : ControllerBase
{
    private readonly IFraudRuleService _fraudRuleService;

    public FraudRuleController(IFraudRuleService fraudRuleService)
    {
        _fraudRuleService = fraudRuleService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _fraudRuleService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("{fraudRuleId:int}")]
    public async Task<IActionResult> GetById(int fraudRuleId)
    {
        var data = await _fraudRuleService.GetByIdAsync(fraudRuleId);
        return Result(data);
    }

    [HttpGet("code/{ruleCode}")]
    public async Task<IActionResult> GetByRuleCode(string ruleCode)
    {
        var data = await _fraudRuleService.GetByRuleCodeAsync(ruleCode);
        return Result(data);
    }

    [HttpGet("{fraudRuleId:int}/details")]
    public async Task<IActionResult> GetFraudRuleDetails(int fraudRuleId)
    {
        var data = await _fraudRuleService.GetFraudRuleDetailsAsync(fraudRuleId);
        return Result(data);
    }

    [HttpGet("exists/code/{ruleCode}")]
    public async Task<IActionResult> ExistsByRuleCode(string ruleCode)
    {
        var result = await _fraudRuleService.ExistsByRuleCodeAsync(ruleCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Roles = "FraudAdministrator")]
    public async Task<IActionResult> Create([FromBody] CreateFraudRuleDto request)
    {
        var data = await _fraudRuleService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { fraudRuleId = data.FraudRuleId },
            ApiResponse<FraudRuleDto>.SuccessResponse(
                data, "Fraud rule created successfully.", 201));
    }

    [HttpPut]
    [Authorize(Roles = "FraudAdministrator")]
    public async Task<IActionResult> Update([FromBody] UpdateFraudRuleDto request)
    {
        var data = await _fraudRuleService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<FraudRuleDto>.FailureResponse(
                "Fraud rule not found.", 404))
            : Ok(ApiResponse<FraudRuleDto>.SuccessResponse(
                data, "Fraud rule updated successfully."));
    }

    [HttpDelete]
    [Authorize(Roles = "FraudAdministrator")]
    public async Task<IActionResult> Delete([FromBody] DeleteFraudRuleDto request)
    {
        var result = await _fraudRuleService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Fraud rule deleted successfully."))
            : NotFound(ApiResponse<object>.FailureResponse(
                "Fraud rule not found or could not be deleted.", 404));
    }

    // ==================== RULE CLASSIFICATION ====================

    [HttpGet("type/{ruleType}")]
    public async Task<IActionResult> GetByRuleType(string ruleType)
    {
        var data = await _fraudRuleService.GetByRuleTypeAsync(ruleType);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("severity/{severity}")]
    public async Task<IActionResult> GetBySeverity(string severity)
    {
        var data = await _fraudRuleService.GetBySeverityAsync(severity);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("action/{actionOnMatch}")]
    public async Task<IActionResult> GetByActionOnMatch(string actionOnMatch)
    {
        var data = await _fraudRuleService.GetByActionOnMatchAsync(actionOnMatch);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    // ==================== ACTIVATION AND EFFECTIVE DATES ====================

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRules()
    {
        var data = await _fraudRuleService.GetActiveRulesAsync();
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveRules()
    {
        var data = await _fraudRuleService.GetInactiveRulesAsync();
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("effective")]
    public async Task<IActionResult> GetEffectiveRules([FromQuery] DateTime effectiveDate)
    {
        var data = await _fraudRuleService.GetEffectiveRulesAsync(effectiveDate);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingRules(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var data = await _fraudRuleService.GetUpcomingRulesAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    // ==================== PRIORITY AND THRESHOLD MANAGEMENT ====================

    [HttpGet("active/priority")]
    public async Task<IActionResult> GetActiveRulesByPriority()
    {
        var data = await _fraudRuleService.GetActiveRulesByPriorityAsync();
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("priority/range")]
    public async Task<IActionResult> GetRulesByPriorityRange(
        [FromQuery] int minimumPriority,
        [FromQuery] int maximumPriority)
    {
        var data = await _fraudRuleService.GetRulesByPriorityRangeAsync(
            minimumPriority, maximumPriority);

        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("threshold/minimum")]
    public async Task<IActionResult> GetRulesByMinimumThreshold(
        [FromQuery] decimal minimumAmount)
    {
        var data = await _fraudRuleService.GetRulesByMinimumThresholdAsync(minimumAmount);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("time-window/{timeWindowMinutes:int}")]
    public async Task<IActionResult> GetRulesByTimeWindow(int timeWindowMinutes)
    {
        var data = await _fraudRuleService.GetRulesByTimeWindowAsync(timeWindowMinutes);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    // ==================== VERSION AND AUDIT MANAGEMENT ====================

    [HttpGet("version/{version:int}")]
    public async Task<IActionResult> GetRulesByVersion(int version)
    {
        var data = await _fraudRuleService.GetRulesByVersionAsync(version);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("created-by/{userId:int}")]
    public async Task<IActionResult> GetRulesCreatedByUser(int userId)
    {
        var data = await _fraudRuleService.GetRulesCreatedByUserAsync(userId);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("updated-by/{userId:int}")]
    public async Task<IActionResult> GetRulesUpdatedByUser(int userId)
    {
        var data = await _fraudRuleService.GetRulesUpdatedByUserAsync(userId);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    // ==================== ALERT HISTORY AND SEARCH ====================

    [HttpGet("with-alerts")]
    public async Task<IActionResult> GetRulesWithFraudAlerts()
    {
        var data = await _fraudRuleService.GetRulesWithFraudAlertsAsync();
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("without-alerts")]
    public async Task<IActionResult> GetRulesWithoutFraudAlerts()
    {
        var data = await _fraudRuleService.GetRulesWithoutFraudAlertsAsync();
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchFraudRules([FromQuery] string searchTerm)
    {
        var data = await _fraudRuleService.SearchFraudRulesAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<FraudRuleDto>>.SuccessResponse(data));
    }

    [HttpGet("{fraudRuleId:int}/alerts/count")]
    public async Task<IActionResult> CountFraudAlertsByRuleId(int fraudRuleId)
    {
        var count = await _fraudRuleService.CountFraudAlertsByRuleIdAsync(fraudRuleId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== RESPONSE HELPER ====================

    private IActionResult Result(FraudRuleDto? data)
    {
        return data is null
            ? NotFound(ApiResponse<FraudRuleDto>.FailureResponse(
                "Fraud rule not found.", 404))
            : Ok(ApiResponse<FraudRuleDto>.SuccessResponse(data));
    }
}
