
using FinVault.Application.DTOs.RefundDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RefundController : ControllerBase
{
    private readonly IRefundService _refundService;

    public RefundController(IRefundService refundService)
    {
        _refundService = refundService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _refundService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("{refundId:int}")]
    public async Task<IActionResult> GetById(int refundId)
    {
        var data = await _refundService.GetByIdAsync(refundId);

        return data is null
            ? NotFound(ApiResponse<RefundDto>.FailureResponse("Refund not found.", 404))
            : Ok(ApiResponse<RefundDto>.SuccessResponse(data));
    }

    [HttpGet("reference/{refundReference}")]
    public async Task<IActionResult> GetByRefundReference(string refundReference)
    {
        var data = await _refundService.GetByRefundReferenceAsync(refundReference);

        return data is null
            ? NotFound(ApiResponse<RefundDto>.FailureResponse("Refund not found.", 404))
            : Ok(ApiResponse<RefundDto>.SuccessResponse(data));
    }

    [HttpGet("{refundId:int}/details")]
    public async Task<IActionResult> GetRefundDetails(int refundId)
    {
        var data = await _refundService.GetRefundDetailsAsync(refundId);

        return data is null
            ? NotFound(ApiResponse<RefundDto>.FailureResponse("Refund details not found.", 404))
            : Ok(ApiResponse<RefundDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRefundDto request)
    {
        var data = await _refundService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { refundId = data.RefundId },
            ApiResponse<RefundDto>.SuccessResponse(
                data, "Refund request created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRefundDto request)
    {
        var data = await _refundService.UpdateAsync(request);

        return data is null
            ? Conflict(ApiResponse<RefundDto>.FailureResponse(
                "Refund could not be updated. It may not exist or its RowVersion may be outdated.", 409))
            : Ok(ApiResponse<RefundDto>.SuccessResponse(
                data, "Refund updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteRefundDto request)
    {
        var result = await _refundService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Refund deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Refund records are retained as financial records and cannot be physically deleted.", 409));
    }

    [HttpGet("exists/reference/{refundReference}")]
    public async Task<IActionResult> ExistsByRefundReference(string refundReference)
    {
        var result = await _refundService.ExistsByRefundReferenceAsync(refundReference);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== ORIGINAL TRANSACTION AND DISPUTE HISTORY ====================

    [HttpGet("original-transaction/{originalTransactionId:int}")]
    public async Task<IActionResult> GetByOriginalTransactionId(int originalTransactionId)
    {
        var data = await _refundService.GetByOriginalTransactionIdAsync(originalTransactionId);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("refund-transaction/{refundTransactionId:int}")]
    public async Task<IActionResult> GetByRefundTransactionId(int refundTransactionId)
    {
        var data = await _refundService.GetByRefundTransactionIdAsync(refundTransactionId);

        return data is null
            ? NotFound(ApiResponse<RefundDto>.FailureResponse(
                "Refund not found for the specified refund transaction.", 404))
            : Ok(ApiResponse<RefundDto>.SuccessResponse(data));
    }

    [HttpGet("dispute/{disputeId:int}")]
    public async Task<IActionResult> GetByDisputeId(int disputeId)
    {
        var data = await _refundService.GetByDisputeIdAsync(disputeId);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("original-transaction/{originalTransactionId:int}/with-transactions")]
    public async Task<IActionResult> GetRefundsWithTransactions(int originalTransactionId)
    {
        var data = await _refundService.GetRefundsWithTransactionsAsync(originalTransactionId);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    // ==================== REFUND STATUS AND TYPE ====================

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _refundService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("requested")]
    public async Task<IActionResult> GetRequestedRefunds()
    {
        var data = await _refundService.GetRequestedRefundsAsync();
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("approved")]
    public async Task<IActionResult> GetApprovedRefunds()
    {
        var data = await _refundService.GetApprovedRefundsAsync();
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("processed")]
    public async Task<IActionResult> GetProcessedRefunds()
    {
        var data = await _refundService.GetProcessedRefundsAsync();
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("rejected")]
    public async Task<IActionResult> GetRejectedRefunds()
    {
        var data = await _refundService.GetRejectedRefundsAsync();
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedRefunds()
    {
        var data = await _refundService.GetFailedRefundsAsync();
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("type/{refundType}")]
    public async Task<IActionResult> GetByRefundType(string refundType)
    {
        var data = await _refundService.GetByRefundTypeAsync(refundType);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("outstanding")]
    public async Task<IActionResult> GetOutstandingRefunds()
    {
        var data = await _refundService.GetOutstandingRefundsAsync();
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    // ==================== APPROVAL AND REQUEST TRACKING ====================

    [HttpGet("requested-by/{userId:int}")]
    public async Task<IActionResult> GetByRequestedByUserId(int userId)
    {
        var data = await _refundService.GetByRequestedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("approved-by/{userId:int}")]
    public async Task<IActionResult> GetByApprovedByUserId(int userId)
    {
        var data = await _refundService.GetByApprovedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("requested/date-range")]
    public async Task<IActionResult> GetRequestedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _refundService.GetRequestedWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    [HttpGet("processed/date-range")]
    public async Task<IActionResult> GetProcessedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _refundService.GetProcessedWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }

    // ==================== FINANCIAL REPORTING ====================

    [HttpGet("original-transaction/{originalTransactionId:int}/total-processed")]
    public async Task<IActionResult> GetTotalProcessedAmountByOriginalTransactionId(
        int originalTransactionId)
    {
        var total = await _refundService.GetTotalProcessedAmountByOriginalTransactionIdAsync(
            originalTransactionId);

        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("dispute/{disputeId:int}/total-processed")]
    public async Task<IActionResult> GetTotalProcessedAmountByDisputeId(int disputeId)
    {
        var total = await _refundService.GetTotalProcessedAmountByDisputeIdAsync(disputeId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("total-processed/date-range")]
    public async Task<IActionResult> GetTotalProcessedAmountWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var total = await _refundService.GetTotalProcessedAmountWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("original-transaction/{originalTransactionId:int}/count")]
    public async Task<IActionResult> CountByOriginalTransactionId(int originalTransactionId)
    {
        var count = await _refundService.CountByOriginalTransactionIdAsync(originalTransactionId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== SEARCH ====================

    [HttpGet("search")]
    public async Task<IActionResult> SearchRefunds([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        var data = await _refundService.SearchRefundsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<RefundDto>>.SuccessResponse(data));
    }
}
