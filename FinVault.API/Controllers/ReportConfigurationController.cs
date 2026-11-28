
using FinVault.Application.DTOs.ReportConfigurationDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportConfigurationController : ControllerBase
{
    private readonly IReportConfigurationService _reportConfigurationService;

    public ReportConfigurationController(IReportConfigurationService reportConfigurationService)
    {
        _reportConfigurationService = reportConfigurationService;
    }

    // 1. REPORT IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetAllAsync()));

    [HttpGet("{reportConfigurationId:int}")]
    public async Task<IActionResult> GetById(int reportConfigurationId)
    {
        var result = await _reportConfigurationService.GetByIdAsync(reportConfigurationId);
        return result == null
            ? NotFound(ApiResponse<ReportConfigurationDto>.FailureResponse("Report configuration not found.", 404))
            : Ok(ApiResponse<ReportConfigurationDto>.SuccessResponse(result));
    }

    [HttpGet("code/{reportCode}")]
    public async Task<IActionResult> GetByReportCode(string reportCode)
    {
        var result = await _reportConfigurationService.GetByReportCodeAsync(reportCode);
        return result == null
            ? NotFound(ApiResponse<ReportConfigurationDto>.FailureResponse("Report configuration not found.", 404))
            : Ok(ApiResponse<ReportConfigurationDto>.SuccessResponse(result));
    }

    [HttpGet("exists")]
    public async Task<IActionResult> ExistsByReportCode([FromQuery] string reportCode)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _reportConfigurationService.ExistsByReportCodeAsync(reportCode)));

    // 2. CRUD AND DETAILS

    [HttpGet("{reportConfigurationId:int}/details")]
    public async Task<IActionResult> GetReportConfigurationDetails(int reportConfigurationId)
    {
        var result = await _reportConfigurationService.GetReportConfigurationDetailsAsync(reportConfigurationId);
        return result == null
            ? NotFound(ApiResponse<ReportConfigurationDto>.FailureResponse("Report configuration not found.", 404))
            : Ok(ApiResponse<ReportConfigurationDto>.SuccessResponse(result));
    }

    [HttpGet("with-owners")]
    public async Task<IActionResult> GetAllWithOwners()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetAllWithOwnersAsync()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReportConfigurationDto dto)
    {
        try
        {
            var result = await _reportConfigurationService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { reportConfigurationId = result.ReportConfigurationId },
                ApiResponse<ReportConfigurationDto>.SuccessResponse(
                    result, "Report configuration created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<ReportConfigurationDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<ReportConfigurationDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateReportConfigurationDto dto)
    {
        try
        {
            var result = await _reportConfigurationService.UpdateAsync(dto);

            return result == null
                ? NotFound(ApiResponse<ReportConfigurationDto>.FailureResponse("Report configuration not found.", 404))
                : Ok(ApiResponse<ReportConfigurationDto>.SuccessResponse(
                    result, "Report configuration updated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<ReportConfigurationDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<ReportConfigurationDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteReportConfigurationDto dto)
    {
        try
        {
            var result = await _reportConfigurationService.DeleteAsync(dto);

            return result
                ? Ok(ApiResponse<bool>.SuccessResponse(true, "Report configuration deleted successfully."))
                : NotFound(ApiResponse<bool>.FailureResponse("Report configuration not found.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<bool>.FailureResponse(ex.Message, 409));
        }
    }

    // 3. REPORT TYPE AND OUTPUT FORMAT

    [HttpGet("type/{reportType}")]
    public async Task<IActionResult> GetByReportType(string reportType)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetByReportTypeAsync(reportType)));

    [HttpGet("output-format/{outputFormat}")]
    public async Task<IActionResult> GetByOutputFormat(string outputFormat)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetByOutputFormatAsync(outputFormat)));

    [HttpGet("regulatory")]
    public async Task<IActionResult> GetRegulatoryReports()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetRegulatoryReportsAsync()));

    // 4. REPORT STATUS

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveReports()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetActiveReportsAsync()));

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveReports()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetInactiveReportsAsync()));

    // 5. SCHEDULED REPORTS

    [HttpGet("scheduled")]
    public async Task<IActionResult> GetScheduledReports()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetScheduledReportsAsync()));

    [HttpGet("due-for-execution")]
    public async Task<IActionResult> GetReportsDueForExecution([FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsDueForExecutionAsync(currentDateTime)));

    [HttpGet("scheduled/date-range")]
    public async Task<IActionResult> GetScheduledReportsWithinRange(
        [FromQuery] DateTime startDateTime,
        [FromQuery] DateTime endDateTime)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetScheduledReportsWithinRangeAsync(
                startDateTime, endDateTime)));

    [HttpGet("previously-executed")]
    public async Task<IActionResult> GetReportsPreviouslyExecuted()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsPreviouslyExecutedAsync()));

    [HttpGet("never-executed")]
    public async Task<IActionResult> GetReportsNeverExecuted()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsNeverExecutedAsync()));

    // 6. USER OWNERSHIP

    [HttpGet("owner/{ownerUserId:int}")]
    public async Task<IActionResult> GetReportsByOwnerUserId(int ownerUserId)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsByOwnerUserIdAsync(ownerUserId)));

    [HttpGet("without-owner")]
    public async Task<IActionResult> GetReportsWithoutOwner()
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsWithoutOwnerAsync()));

    // 7. REPORT EXECUTION HISTORY

    [HttpGet("last-run/date-range")]
    public async Task<IActionResult> GetReportsLastRunWithinRange(
        [FromQuery] DateTime startDateTime,
        [FromQuery] DateTime endDateTime)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsLastRunWithinRangeAsync(
                startDateTime, endDateTime)));

    [HttpGet("next-run/before")]
    public async Task<IActionResult> GetReportsWithNextRunBefore([FromQuery] DateTime dateTime)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsWithNextRunBeforeAsync(dateTime)));

    // 8. RETENTION AND SEARCH

    [HttpGet("retention")]
    public async Task<IActionResult> GetReportsByMinimumRetentionDays(
        [FromQuery] int minimumRetentionDays)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.GetReportsByMinimumRetentionDaysAsync(minimumRetentionDays)));

    [HttpGet("search")]
    public async Task<IActionResult> SearchReportConfigurations([FromQuery] string searchTerm)
        => Ok(ApiResponse<IEnumerable<ReportConfigurationDto>>.SuccessResponse(
            await _reportConfigurationService.SearchReportConfigurationsAsync(searchTerm)));

    // 9. REPORTING

    [HttpGet("count/by-type")]
    public async Task<IActionResult> CountByReportType([FromQuery] string reportType)
        => Ok(ApiResponse<int>.SuccessResponse(
            await _reportConfigurationService.CountByReportTypeAsync(reportType)));
}
