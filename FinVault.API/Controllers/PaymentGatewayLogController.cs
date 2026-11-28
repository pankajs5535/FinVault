
using FinVault.Application.DTOs.PaymentGatewayLogDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PaymentGatewayLogController : ControllerBase
{
    private readonly IPaymentGatewayLogService _paymentGatewayLogService;

    public PaymentGatewayLogController(IPaymentGatewayLogService paymentGatewayLogService)
    {
        _paymentGatewayLogService = paymentGatewayLogService;
    }

    // ==================== LOG IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _paymentGatewayLogService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("{paymentGatewayLogId:int}")]
    public async Task<IActionResult> GetById(int paymentGatewayLogId)
    {
        var data = await _paymentGatewayLogService.GetByIdAsync(paymentGatewayLogId);

        return data is null
            ? NotFound(ApiResponse<PaymentGatewayLogDto>.FailureResponse("Payment gateway log not found.", 404))
            : Ok(ApiResponse<PaymentGatewayLogDto>.SuccessResponse(data));
    }

    [HttpGet("{paymentGatewayLogId:int}/details")]
    public async Task<IActionResult> GetLogDetails(int paymentGatewayLogId)
    {
        var data = await _paymentGatewayLogService.GetLogDetailsAsync(paymentGatewayLogId);

        return data is null
            ? NotFound(ApiResponse<PaymentGatewayLogDto>.FailureResponse("Payment gateway log details not found.", 404))
            : Ok(ApiResponse<PaymentGatewayLogDto>.SuccessResponse(data));
    }

    [HttpGet("correlation/{correlationId}")]
    public async Task<IActionResult> GetByCorrelationId(string correlationId)
    {
        var data = await _paymentGatewayLogService.GetByCorrelationIdAsync(correlationId);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("payment-request/{paymentRequestId:int}/latest")]
    public async Task<IActionResult> GetLatestLogByPaymentRequestId(int paymentRequestId)
    {
        var data = await _paymentGatewayLogService.GetLatestLogByPaymentRequestIdAsync(paymentRequestId);

        return data is null
            ? NotFound(ApiResponse<PaymentGatewayLogDto>.FailureResponse("No gateway log found for this payment request.", 404))
            : Ok(ApiResponse<PaymentGatewayLogDto>.SuccessResponse(data));
    }

    [HttpGet("payment-request/{paymentRequestId:int}/attempt/{attemptNumber:int}")]
    public async Task<IActionResult> GetByPaymentRequestAndAttempt(
        int paymentRequestId,
        int attemptNumber)
    {
        if (attemptNumber < 1)
            return BadRequest(ApiResponse<object>.FailureResponse("Attempt number must be positive.", 400));

        var data = await _paymentGatewayLogService.GetByPaymentRequestAndAttemptAsync(
            paymentRequestId, attemptNumber);

        return data is null
            ? NotFound(ApiResponse<PaymentGatewayLogDto>.FailureResponse("Gateway log attempt not found.", 404))
            : Ok(ApiResponse<PaymentGatewayLogDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentGatewayLogDto request)
    {
        var data = await _paymentGatewayLogService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { paymentGatewayLogId = data.PaymentGatewayLogId },
            ApiResponse<PaymentGatewayLogDto>.SuccessResponse(
                data, "Payment gateway log created successfully.", 201));
    }

    // ==================== PAYMENT REQUEST HISTORY ====================

    [HttpGet("payment-request/{paymentRequestId:int}")]
    public async Task<IActionResult> GetByPaymentRequestId(int paymentRequestId)
    {
        var data = await _paymentGatewayLogService.GetByPaymentRequestIdAsync(paymentRequestId);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("payment-request/{paymentRequestId:int}/with-payment-request")]
    public async Task<IActionResult> GetLogsWithPaymentRequest(int paymentRequestId)
    {
        var data = await _paymentGatewayLogService.GetLogsWithPaymentRequestAsync(paymentRequestId);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("payment-requests")]
    public async Task<IActionResult> GetByPaymentRequestIds(
        [FromQuery] IEnumerable<int> paymentRequestIds)
    {
        var ids = paymentRequestIds?.Distinct().ToArray() ?? Array.Empty<int>();

        if (ids.Length == 0 || ids.Any(id => id < 1))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Provide at least one valid payment request ID.", 400));

        var data = await _paymentGatewayLogService.GetByPaymentRequestIdsAsync(ids);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    // ==================== GATEWAY AND OPERATION TRACKING ====================

    [HttpGet("gateway/{gatewayName}")]
    public async Task<IActionResult> GetByGatewayName(string gatewayName)
    {
        var data = await _paymentGatewayLogService.GetByGatewayNameAsync(gatewayName);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("operation/{operation}")]
    public async Task<IActionResult> GetByOperation(string operation)
    {
        var data = await _paymentGatewayLogService.GetByOperationAsync(operation);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("gateway/{gatewayName}/operation/{operation}")]
    public async Task<IActionResult> GetByGatewayAndOperation(
        string gatewayName,
        string operation)
    {
        var data = await _paymentGatewayLogService.GetByGatewayAndOperationAsync(
            gatewayName, operation);

        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    // ==================== SUCCESS AND FAILURE MONITORING ====================

    [HttpGet("successful")]
    public async Task<IActionResult> GetSuccessfulLogs()
    {
        var data = await _paymentGatewayLogService.GetSuccessfulLogsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedLogs()
    {
        var data = await _paymentGatewayLogService.GetFailedLogsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("payment-request/{paymentRequestId:int}/failed")]
    public async Task<IActionResult> GetFailedLogsByPaymentRequestId(int paymentRequestId)
    {
        var data = await _paymentGatewayLogService.GetFailedLogsByPaymentRequestIdAsync(paymentRequestId);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("response-code/{responseCode}")]
    public async Task<IActionResult> GetByResponseCode(string responseCode)
    {
        var data = await _paymentGatewayLogService.GetByResponseCodeAsync(responseCode);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("http-status-code/{httpStatusCode:int}")]
    public async Task<IActionResult> GetByHttpStatusCode(int httpStatusCode)
    {
        if (httpStatusCode < 100 || httpStatusCode > 599)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "HTTP status code must be between 100 and 599.", 400));

        var data = await _paymentGatewayLogService.GetByHttpStatusCodeAsync(httpStatusCode);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    // ==================== DATE-BASED MONITORING ====================

    [HttpGet("date-range")]
    public async Task<IActionResult> GetLogsWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _paymentGatewayLogService.GetLogsWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("failed/date-range")]
    public async Task<IActionResult> GetFailedLogsWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _paymentGatewayLogService.GetFailedLogsWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    // ==================== PERFORMANCE AND RETRY ANALYSIS ====================

    [HttpGet("slow")]
    public async Task<IActionResult> GetSlowGatewayLogs(
        [FromQuery] int minimumDurationMs)
    {
        if (minimumDurationMs < 0)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Minimum duration cannot be negative.", 400));

        var data = await _paymentGatewayLogService.GetSlowGatewayLogsAsync(minimumDurationMs);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("average-duration")]
    public async Task<IActionResult> GetAverageDurationWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var average = await _paymentGatewayLogService.GetAverageDurationWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<double?>.SuccessResponse(average));
    }

    [HttpGet("payment-request/{paymentRequestId:int}/maximum-attempt")]
    public async Task<IActionResult> GetMaximumAttemptNumber(int paymentRequestId)
    {
        var maximumAttempt = await _paymentGatewayLogService.GetMaximumAttemptNumberAsync(
            paymentRequestId);

        return Ok(ApiResponse<int>.SuccessResponse(maximumAttempt));
    }

    // ==================== SEARCH AND COUNTS ====================

    [HttpGet("search")]
    public async Task<IActionResult> SearchGatewayLogs([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        var data = await _paymentGatewayLogService.SearchGatewayLogsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<PaymentGatewayLogDto>>.SuccessResponse(data));
    }

    [HttpGet("payment-request/{paymentRequestId:int}/count")]
    public async Task<IActionResult> CountLogsByPaymentRequestId(int paymentRequestId)
    {
        var count = await _paymentGatewayLogService.CountLogsByPaymentRequestIdAsync(
            paymentRequestId);

        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("gateway/{gatewayName}/failed/count")]
    public async Task<IActionResult> CountFailedLogsByGateway(string gatewayName)
    {
        var count = await _paymentGatewayLogService.CountFailedLogsByGatewayAsync(gatewayName);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }
}
