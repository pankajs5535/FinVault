
using FinVault.Application.DTOs.AIUsageLogDtos;
using FinVault.Application.Interface.IServices;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIUsageLogController : ControllerBase
{
    private readonly IAIUsageLogService _usageLogService;

    public AIUsageLogController(IAIUsageLogService usageLogService)
    {
        _usageLogService = usageLogService;
    }

    // 1. USAGE LOG CRUD

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
            await _usageLogService.GetAllAsync()));

    [HttpGet("{usageLogId:int}")]
    public async Task<IActionResult> GetById(int usageLogId)
    {
        var result = await _usageLogService.GetByIdAsync(usageLogId);

        return result == null
            ? NotFound(ApiResponse<AIUsageLogDto>.FailureResponse("Usage log not found.", 404))
            : Ok(ApiResponse<AIUsageLogDto>.SuccessResponse(result));
    }

    [HttpGet("request/{requestId}")]
    public async Task<IActionResult> GetByRequestId(string requestId)
    {
        var result = await _usageLogService.GetByRequestIdAsync(requestId);

        return result == null
            ? NotFound(ApiResponse<AIUsageLogDto>.FailureResponse("Usage log not found.", 404))
            : Ok(ApiResponse<AIUsageLogDto>.SuccessResponse(result));
    }

    [HttpGet("{usageLogId:int}/details")]
    public async Task<IActionResult> GetUsageLogDetails(int usageLogId)
    {
        var result = await _usageLogService.GetUsageLogDetailsAsync(usageLogId);

        return result == null
            ? NotFound(ApiResponse<AIUsageLogDto>.FailureResponse("Usage log not found.", 404))
            : Ok(ApiResponse<AIUsageLogDto>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAIUsageLogDto dto)
    {
        try
        {
            var result = await _usageLogService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { usageLogId = result.AIUsageLogId },
                ApiResponse<AIUsageLogDto>.SuccessResponse(
                    result, "AI usage log created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AIUsageLogDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<AIUsageLogDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAIUsageLogDto dto)
    {
        try
        {
            var result = await _usageLogService.UpdateAsync(dto);

            return result
                ? Ok(ApiResponse<bool>.SuccessResponse(true))
                : NotFound(ApiResponse<bool>.FailureResponse("Usage log not found.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<bool>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAIUsageLogDto dto)
    {
        try
        {
            var result = await _usageLogService.DeleteAsync(dto);

            return result
                ? Ok(ApiResponse<bool>.SuccessResponse(true))
                : NotFound(ApiResponse<bool>.FailureResponse("Usage log not found.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<bool>.FailureResponse(ex.Message, 409));
        }
    }

    // 2. USER AND CONVERSATION USAGE

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUsageLogsByUserId(int userId)
        => Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
            await _usageLogService.GetUsageLogsByUserIdAsync(userId)));

    [HttpGet("conversation/{conversationId:int}")]
    public async Task<IActionResult> GetUsageLogsByConversationId(int conversationId)
        => Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
            await _usageLogService.GetUsageLogsByConversationIdAsync(conversationId)));

    [HttpGet("message/{messageId:int}")]
    public async Task<IActionResult> GetUsageLogsByMessageId(int messageId)
        => Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
            await _usageLogService.GetUsageLogsByMessageIdAsync(messageId)));

    // 3. PROVIDER, MODEL AND FEATURE USAGE

    [HttpGet("provider/{provider}")]
    public async Task<IActionResult> GetUsageLogsByProvider(string provider)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
                await _usageLogService.GetUsageLogsByProviderAsync(provider)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIUsageLogDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("model/{modelName}")]
    public async Task<IActionResult> GetUsageLogsByModel(string modelName)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
                await _usageLogService.GetUsageLogsByModelAsync(modelName)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIUsageLogDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("feature/{featureName}")]
    public async Task<IActionResult> GetUsageLogsByFeature(string featureName)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
                await _usageLogService.GetUsageLogsByFeatureAsync(featureName)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIUsageLogDto>>.FailureResponse(ex.Message, 400));
        }
    }

    // 4. SUCCESS AND FAILURE MONITORING

    [HttpGet("successful")]
    public async Task<IActionResult> GetSuccessfulUsageLogs()
        => Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
            await _usageLogService.GetSuccessfulUsageLogsAsync()));

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedUsageLogs()
        => Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
            await _usageLogService.GetFailedUsageLogsAsync()));

    [HttpGet("failed/date-range")]
    public async Task<IActionResult> GetFailedUsageLogsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
                await _usageLogService.GetFailedUsageLogsByDateRangeAsync(startDate, endDate)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIUsageLogDto>>.FailureResponse(ex.Message, 400));
        }
    }

    // 5. TOKEN, COST AND PERFORMANCE REPORTING

    [HttpGet("user/{userId:int}/total-tokens")]
    public async Task<IActionResult> GetTotalTokensByUserId(int userId)
    {
        try
        {
            return Ok(ApiResponse<int>.SuccessResponse(
                await _usageLogService.GetTotalTokensByUserIdAsync(userId)));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ApiResponse<int>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("user/{userId:int}/estimated-cost")]
    public async Task<IActionResult> GetEstimatedCostByUserId(int userId)
    {
        try
        {
            return Ok(ApiResponse<decimal>.SuccessResponse(
                await _usageLogService.GetEstimatedCostByUserIdAsync(userId)));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ApiResponse<decimal>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("feature/{featureName}/estimated-cost")]
    public async Task<IActionResult> GetEstimatedCostByFeature(string featureName)
    {
        try
        {
            return Ok(ApiResponse<decimal>.SuccessResponse(
                await _usageLogService.GetEstimatedCostByFeatureAsync(featureName)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<decimal>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("slow")]
    public async Task<IActionResult> GetSlowUsageLogs([FromQuery] int minimumLatencyMs)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
                await _usageLogService.GetSlowUsageLogsAsync(minimumLatencyMs)));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIUsageLogDto>>.FailureResponse(ex.Message, 400));
        }
    }

    // 6. DATE RANGE AND PAGINATED SEARCH

    [HttpGet("date-range")]
    public async Task<IActionResult> GetUsageLogsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(
                await _usageLogService.GetUsageLogsByDateRangeAsync(startDate, endDate)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIUsageLogDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsageLogs(
        [FromQuery] string? provider,
        [FromQuery] string? modelName,
        [FromQuery] string? featureName,
        [FromQuery] bool? isSuccess,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var result = await _usageLogService.SearchUsageLogsAsync(
                provider,
                modelName,
                featureName,
                isSuccess,
                startDate,
                endDate,
                pageNumber,
                pageSize);

            return Ok(ApiResponse<IEnumerable<AIUsageLogDto>>.SuccessResponse(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIUsageLogDto>>.FailureResponse(ex.Message, 400));
        }
    }
}
