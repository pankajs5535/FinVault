
using FinVault.Application.DTOs.ComplianceRecordDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ComplianceRecordController : ControllerBase
{
    private readonly IComplianceRecordService _complianceRecordService;

    public ComplianceRecordController(IComplianceRecordService complianceRecordService)
    {
        _complianceRecordService = complianceRecordService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _complianceRecordService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("{complianceRecordId:int}")]
    public async Task<IActionResult> GetById(int complianceRecordId)
    {
        var data = await _complianceRecordService.GetByIdAsync(complianceRecordId);

        return data is null
            ? NotFound(ApiResponse<ComplianceRecordDto>.FailureResponse("Compliance record not found.", 404))
            : Ok(ApiResponse<ComplianceRecordDto>.SuccessResponse(data));
    }

    [HttpGet("reference/{referenceNumber}")]
    public async Task<IActionResult> GetByReferenceNumber(string referenceNumber)
    {
        var data = await _complianceRecordService.GetByReferenceNumberAsync(referenceNumber);

        return data is null
            ? NotFound(ApiResponse<ComplianceRecordDto>.FailureResponse("Compliance record not found.", 404))
            : Ok(ApiResponse<ComplianceRecordDto>.SuccessResponse(data));
    }

    [HttpGet("{complianceRecordId:int}/details")]
    public async Task<IActionResult> GetComplianceRecordDetails(int complianceRecordId)
    {
        var data = await _complianceRecordService.GetComplianceRecordDetailsAsync(complianceRecordId);

        return data is null
            ? NotFound(ApiResponse<ComplianceRecordDto>.FailureResponse("Compliance record details not found.", 404))
            : Ok(ApiResponse<ComplianceRecordDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComplianceRecordDto request)
    {
        var data = await _complianceRecordService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { complianceRecordId = data.ComplianceRecordId },
            ApiResponse<ComplianceRecordDto>.SuccessResponse(
                data, "Compliance record created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateComplianceRecordDto request)
    {
        var data = await _complianceRecordService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<ComplianceRecordDto>.FailureResponse("Compliance record not found.", 404))
            : Ok(ApiResponse<ComplianceRecordDto>.SuccessResponse(
                data, "Compliance record updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteComplianceRecordDto request)
    {
        var result = await _complianceRecordService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Compliance record deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Compliance record could not be deleted.", 409));
    }

    [HttpGet("exists/reference/{referenceNumber}")]
    public async Task<IActionResult> ExistsByReferenceNumber(string referenceNumber)
    {
        var result = await _complianceRecordService.ExistsByReferenceNumberAsync(referenceNumber);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== CUSTOMER AND BANKING RELATIONSHIPS ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetRecordsByCustomerId(int customerId)
    {
        var data = await _complianceRecordService.GetRecordsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}")]
    public async Task<IActionResult> GetRecordsByAccountId(int accountId)
    {
        var data = await _complianceRecordService.GetRecordsByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("transaction/{transactionId:int}")]
    public async Task<IActionResult> GetRecordsByTransactionId(int transactionId)
    {
        var data = await _complianceRecordService.GetRecordsByTransactionIdAsync(transactionId);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("fraud-alert/{fraudAlertId:int}")]
    public async Task<IActionResult> GetRecordsByFraudAlertId(int fraudAlertId)
    {
        var data = await _complianceRecordService.GetRecordsByFraudAlertIdAsync(fraudAlertId);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    // ==================== TYPE AND RISK CLASSIFICATION ====================

    [HttpGet("type/{recordType}")]
    public async Task<IActionResult> GetRecordsByType(string recordType)
    {
        var data = await _complianceRecordService.GetRecordsByTypeAsync(recordType);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("risk-level/{riskLevel}")]
    public async Task<IActionResult> GetRecordsByRiskLevel(string riskLevel)
    {
        var data = await _complianceRecordService.GetRecordsByRiskLevelAsync(riskLevel);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetRecordsByStatus(string status)
    {
        var data = await _complianceRecordService.GetRecordsByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    // ==================== REVIEW AND FILING MANAGEMENT ====================

    [HttpGet("filed-by-employee/{employeeId:int}")]
    public async Task<IActionResult> GetRecordsByFiledByEmployeeId(int employeeId)
    {
        var data = await _complianceRecordService.GetRecordsByFiledByEmployeeIdAsync(employeeId);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdueRecords([FromQuery] DateTime currentDate)
    {
        var data = await _complianceRecordService.GetOverdueRecordsAsync(currentDate);
        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("due-within-range")]
    public async Task<IActionResult> GetRecordsDueWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _complianceRecordService.GetRecordsDueWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("filed-within-range")]
    public async Task<IActionResult> GetFiledRecordsWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _complianceRecordService.GetFiledRecordsWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND REPORTING ====================

    [HttpGet("search")]
    public async Task<IActionResult> SearchComplianceRecords(
        [FromQuery] string? searchTerm,
        [FromQuery] string? recordType,
        [FromQuery] string? status,
        [FromQuery] string? riskLevel,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 500)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 500.", 400));

        if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "End date cannot be earlier than start date.", 400));

        var data = await _complianceRecordService.SearchComplianceRecordsAsync(
            searchTerm,
            recordType,
            status,
            riskLevel,
            startDate,
            endDate,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<ComplianceRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/count")]
    public async Task<IActionResult> CountRecordsByCustomerId(int customerId)
    {
        var count = await _complianceRecordService.CountRecordsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }
}
