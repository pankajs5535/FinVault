
using FinVault.Application.DTOs.AuditLogDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    // 1. AUDIT LOG IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetAllAsync()));

    [HttpGet("{auditLogId:int}")]
    public async Task<IActionResult> GetById(int auditLogId)
    {
        var result = await _auditLogService.GetByIdAsync(auditLogId);
        return result == null
            ? NotFound(ApiResponse<AuditLogDto>.FailureResponse("Audit log not found.", 404))
            : Ok(ApiResponse<AuditLogDto>.SuccessResponse(result));
    }

    [HttpGet("{auditLogId:int}/details")]
    public async Task<IActionResult> GetAuditLogDetails(int auditLogId)
    {
        var result = await _auditLogService.GetAuditLogDetailsAsync(auditLogId);
        return result == null
            ? NotFound(ApiResponse<AuditLogDto>.FailureResponse("Audit log not found.", 404))
            : Ok(ApiResponse<AuditLogDto>.SuccessResponse(result));
    }

    [HttpGet("hash/{entryHash}")]
    public async Task<IActionResult> GetByEntryHash(string entryHash)
    {
        var result = await _auditLogService.GetByEntryHashAsync(entryHash);
        return result == null
            ? NotFound(ApiResponse<AuditLogDto>.FailureResponse("Audit log not found.", 404))
            : Ok(ApiResponse<AuditLogDto>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuditLogDto dto)
    {
        try
        {
            var result = await _auditLogService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { auditLogId = result.AuditLogId },
                ApiResponse<AuditLogDto>.SuccessResponse(
                    result, "Audit log created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AuditLogDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AuditLogDto>.FailureResponse(ex.Message, 400));
        }
    }

    // 2. ENTITY CHANGE HISTORY

    [HttpGet("entity/{entityName}/{entityId:int}/history")]
    public async Task<IActionResult> GetEntityHistory(string entityName, int entityId)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetEntityHistoryAsync(entityName, entityId)));

    [HttpGet("entity/{entityName}")]
    public async Task<IActionResult> GetLogsByEntityName(string entityName)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetLogsByEntityNameAsync(entityName)));

    [HttpGet("entity/{entityName}/action/{action}")]
    public async Task<IActionResult> GetLogsByEntityAndAction(string entityName, string action)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetLogsByEntityAndActionAsync(entityName, action)));

    [HttpGet("entity/{entityName}/{entityId:int}/count")]
    public async Task<IActionResult> CountEntityHistory(string entityName, int entityId)
        => Ok(ApiResponse<int>.SuccessResponse(
            await _auditLogService.CountEntityHistoryAsync(entityName, entityId)));

    // 3. USER ACTIVITY HISTORY

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetLogsByUserId(int userId)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetLogsByUserIdAsync(userId)));

    [HttpGet("correlation/{correlationId}")]
    public async Task<IActionResult> GetLogsByCorrelationId(string correlationId)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetLogsByCorrelationIdAsync(correlationId)));

    // 4. OUTCOME AND SECURITY MONITORING

    [HttpGet("outcome/{outcome}")]
    public async Task<IActionResult> GetLogsByOutcome(string outcome)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetLogsByOutcomeAsync(outcome)));

    [HttpGet("security-sensitive")]
    public async Task<IActionResult> GetSecuritySensitiveLogs()
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetSecuritySensitiveLogsAsync()));

    [HttpGet("security-sensitive/failed")]
    public async Task<IActionResult> GetFailedSecuritySensitiveLogs()
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetFailedSecuritySensitiveLogsAsync()));

    // 5. DATE RANGE AND INTEGRITY INVESTIGATION

    [HttpGet("date-range")]
    public async Task<IActionResult> GetLogsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetLogsByDateRangeAsync(startDate, endDate)));

    [HttpGet("hash-chain")]
    public async Task<IActionResult> GetLogsWithHashChain(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.GetLogsWithHashChainAsync(pageNumber, pageSize)));

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestAuditLog()
    {
        var result = await _auditLogService.GetLatestAuditLogAsync();
        return result == null
            ? NotFound(ApiResponse<AuditLogDto>.FailureResponse("No audit logs found.", 404))
            : Ok(ApiResponse<AuditLogDto>.SuccessResponse(result));
    }

    // 6. PAGINATED AUDIT SEARCH

    [HttpGet("search")]
    public async Task<IActionResult> SearchAuditLogs(
        [FromQuery] string? entityName,
        [FromQuery] string? action,
        [FromQuery] string? outcome,
        [FromQuery] int? userId,
        [FromQuery] bool? isSecuritySensitive,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
        => Ok(ApiResponse<IEnumerable<AuditLogDto>>.SuccessResponse(
            await _auditLogService.SearchAuditLogsAsync(
                entityName,
                action,
                outcome,
                userId,
                isSecuritySensitive,
                startDate,
                endDate,
                pageNumber,
                pageSize)));
}
