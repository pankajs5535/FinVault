
using FinVault.Application.DTOs.PaymentRequestDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PaymentRequestController : ControllerBase
{
    private readonly IPaymentRequestService _paymentRequestService;

    public PaymentRequestController(IPaymentRequestService paymentRequestService)
    {
        _paymentRequestService = paymentRequestService;
    }

    // ==================== PAYMENT REQUEST IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _paymentRequestService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("{paymentRequestId:int}")]
    public async Task<IActionResult> GetById(int paymentRequestId)
    {
        var data = await _paymentRequestService.GetByIdAsync(paymentRequestId);

        return data is null
            ? NotFound(ApiResponse<PaymentRequestDto>.FailureResponse("Payment request not found.", 404))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(data));
    }

    [HttpGet("reference/{requestReference}")]
    public async Task<IActionResult> GetByRequestReference(string requestReference)
    {
        var data = await _paymentRequestService.GetByRequestReferenceAsync(requestReference);

        return data is null
            ? NotFound(ApiResponse<PaymentRequestDto>.FailureResponse("Payment request not found.", 404))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(data));
    }

    [HttpGet("idempotency/{idempotencyKey}")]
    public async Task<IActionResult> GetByIdempotencyKey(string idempotencyKey)
    {
        var data = await _paymentRequestService.GetByIdempotencyKeyAsync(idempotencyKey);

        return data is null
            ? NotFound(ApiResponse<PaymentRequestDto>.FailureResponse("Payment request not found.", 404))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(data));
    }

    [HttpGet("utr/{utrNumber}")]
    public async Task<IActionResult> GetByUtrNumber(string utrNumber)
    {
        var data = await _paymentRequestService.GetByUtrNumberAsync(utrNumber);

        return data is null
            ? NotFound(ApiResponse<PaymentRequestDto>.FailureResponse("Payment request not found.", 404))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(data));
    }

    [HttpGet("{paymentRequestId:int}/details")]
    public async Task<IActionResult> GetPaymentRequestDetails(int paymentRequestId)
    {
        var data = await _paymentRequestService.GetPaymentRequestDetailsAsync(paymentRequestId);

        return data is null
            ? NotFound(ApiResponse<PaymentRequestDto>.FailureResponse("Payment request details not found.", 404))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(data));
    }

    [HttpGet("exists/reference/{requestReference}")]
    public async Task<IActionResult> ExistsByRequestReference(string requestReference)
    {
        var result = await _paymentRequestService.ExistsByRequestReferenceAsync(requestReference);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/idempotency/{idempotencyKey}")]
    public async Task<IActionResult> ExistsByIdempotencyKey(string idempotencyKey)
    {
        var result = await _paymentRequestService.ExistsByIdempotencyKeyAsync(idempotencyKey);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto request)
    {
        var data = await _paymentRequestService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { paymentRequestId = data.PaymentRequestId },
            ApiResponse<PaymentRequestDto>.SuccessResponse(
                data, "Payment request created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePaymentRequestDto request)
    {
        var data = await _paymentRequestService.UpdateAsync(request);

        return data is null
            ? Conflict(ApiResponse<PaymentRequestDto>.FailureResponse(
                "Payment request could not be updated. It may not exist, may already be processing, or its RowVersion may be outdated.", 409))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(
                data, "Payment request updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletePaymentRequestDto request)
    {
        var result = await _paymentRequestService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Payment request deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Payment request could not be deleted.", 409));
    }

    // ==================== CUSTOMER AND ACCOUNT PAYMENT HISTORY ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var data = await _paymentRequestService.GetByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("source-account/{sourceAccountId:int}")]
    public async Task<IActionResult> GetBySourceAccountId(int sourceAccountId)
    {
        var data = await _paymentRequestService.GetBySourceAccountIdAsync(sourceAccountId);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("beneficiary/{beneficiaryId:int}")]
    public async Task<IActionResult> GetByBeneficiaryId(int beneficiaryId)
    {
        var data = await _paymentRequestService.GetByBeneficiaryIdAsync(beneficiaryId);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("payment-method/{paymentMethodId:int}")]
    public async Task<IActionResult> GetByPaymentMethodId(int paymentMethodId)
    {
        var data = await _paymentRequestService.GetByPaymentMethodIdAsync(paymentMethodId);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("currency/{currencyId:int}")]
    public async Task<IActionResult> GetByCurrencyId(int currencyId)
    {
        var data = await _paymentRequestService.GetByCurrencyIdAsync(currencyId);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    // ==================== PAYMENT LIFECYCLE AND STATUS ====================

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _paymentRequestService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("authorized")]
    public async Task<IActionResult> GetAuthorizedPaymentRequests()
    {
        var data = await _paymentRequestService.GetAuthorizedPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("submitted")]
    public async Task<IActionResult> GetSubmittedPaymentRequests()
    {
        var data = await _paymentRequestService.GetSubmittedPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("processing")]
    public async Task<IActionResult> GetProcessingPaymentRequests()
    {
        var data = await _paymentRequestService.GetProcessingPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedPaymentRequests()
    {
        var data = await _paymentRequestService.GetCompletedPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedPaymentRequests()
    {
        var data = await _paymentRequestService.GetFailedPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredPaymentRequests()
    {
        var data = await _paymentRequestService.GetExpiredPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("cancelled")]
    public async Task<IActionResult> GetCancelledPaymentRequests()
    {
        var data = await _paymentRequestService.GetCancelledPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    // ==================== CHANNEL AND SCHEDULING ====================

    [HttpGet("channel/{channel}")]
    public async Task<IActionResult> GetByChannel(string channel)
    {
        var data = await _paymentRequestService.GetByChannelAsync(channel);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("scheduled/date-range")]
    public async Task<IActionResult> GetScheduledPaymentsWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));

        var data = await _paymentRequestService.GetScheduledPaymentsWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("scheduled/due")]
    public async Task<IActionResult> GetScheduledPaymentsDue(
        [FromQuery] DateTime currentDateTime)
    {
        if (currentDateTime == default)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid current date and time is required.", 400));

        var data = await _paymentRequestService.GetScheduledPaymentsDueAsync(currentDateTime);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("expired-pending")]
    public async Task<IActionResult> GetExpiredPendingRequests(
        [FromQuery] DateTime currentDateTime)
    {
        if (currentDateTime == default)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid current date and time is required.", 400));

        var data = await _paymentRequestService.GetExpiredPendingRequestsAsync(currentDateTime);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    // ==================== DATE-BASED REPORTING ====================

    [HttpGet("requested/date-range")]
    public async Task<IActionResult> GetRequestedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));

        var data = await _paymentRequestService.GetRequestedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("completed/date-range")]
    public async Task<IActionResult> GetCompletedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));

        var data = await _paymentRequestService.GetCompletedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/recent")]
    public async Task<IActionResult> GetRecentRequestsByCustomerId(
        int customerId,
        [FromQuery] int count = 10)
    {
        if (count < 1 || count > 500)
            return BadRequest(ApiResponse<object>.FailureResponse("Count must be between 1 and 500.", 400));

        var data = await _paymentRequestService.GetRecentRequestsByCustomerIdAsync(
            customerId, count);

        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    // ==================== TRANSACTION AND GATEWAY TRACKING ====================

    [HttpGet("{paymentRequestId:int}/transaction")]
    public async Task<IActionResult> GetWithTransaction(int paymentRequestId)
    {
        var data = await _paymentRequestService.GetWithTransactionAsync(paymentRequestId);

        return data is null
            ? NotFound(ApiResponse<PaymentRequestDto>.FailureResponse("Payment request or linked transaction not found.", 404))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(data));
    }

    [HttpGet("{paymentRequestId:int}/gateway-logs")]
    public async Task<IActionResult> GetWithGatewayLogs(int paymentRequestId)
    {
        var data = await _paymentRequestService.GetWithGatewayLogsAsync(paymentRequestId);

        return data is null
            ? NotFound(ApiResponse<PaymentRequestDto>.FailureResponse("Payment request or gateway logs not found.", 404))
            : Ok(ApiResponse<PaymentRequestDto>.SuccessResponse(data));
    }

    [HttpGet("without-transaction")]
    public async Task<IActionResult> GetRequestsWithoutTransaction()
    {
        var data = await _paymentRequestService.GetRequestsWithoutTransactionAsync();
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("failure-code/{failureCode}")]
    public async Task<IActionResult> GetByFailureCode(string failureCode)
    {
        var data = await _paymentRequestService.GetByFailureCodeAsync(failureCode);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    // ==================== FINANCIAL REPORTING AND SEARCH ====================

    [HttpGet("customer/{customerId:int}/total-completed")]
    public async Task<IActionResult> GetTotalCompletedAmountByCustomerId(int customerId)
    {
        var total = await _paymentRequestService.GetTotalCompletedAmountByCustomerIdAsync(customerId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("total-completed/date-range")]
    public async Task<IActionResult> GetTotalCompletedAmountWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));

        var total = await _paymentRequestService.GetTotalCompletedAmountWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchPaymentRequests([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse("Search term is required.", 400));

        var data = await _paymentRequestService.SearchPaymentRequestsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<PaymentRequestDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/count-by-status")]
    public async Task<IActionResult> CountByCustomerAndStatus(
        int customerId,
        [FromQuery] string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest(ApiResponse<object>.FailureResponse("Status is required.", 400));

        var count = await _paymentRequestService.CountByCustomerAndStatusAsync(customerId, status);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== UPI PAYMENT OPERATIONS ====================

    [HttpPost("upi/transfer")]
    public async Task<IActionResult> InitiateUpiTransfer([FromBody] InitiateUpiTransferDto request)
    {
        var data = await _paymentRequestService.InitiateUpiTransferAsync(request);

        return Ok(ApiResponse<UpiTransferResponseDto>.SuccessResponse(
            data, "UPI transfer request processed successfully."));
    }

    [HttpPost("upi/provider-callback")]
    public async Task<IActionResult> ProcessUpiProviderCallback(
        [FromBody] UpiProviderCallbackDto request)
    {
        var result = await _paymentRequestService.ProcessUpiProviderCallbackAsync(request);

        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "UPI provider callback processed successfully."))
            : Conflict(ApiResponse<bool>.FailureResponse(
                "UPI provider callback could not be processed.", 409));
    }

    [HttpGet("upi/{paymentRequestId:int}/status")]
    public async Task<IActionResult> GetUpiTransactionStatus(int paymentRequestId)
    {
        var data = await _paymentRequestService.GetUpiTransactionStatusAsync(paymentRequestId);

        return data is null
            ? NotFound(ApiResponse<UpiTransactionStatusDto>.FailureResponse(
                "UPI transaction status not found.", 404))
            : Ok(ApiResponse<UpiTransactionStatusDto>.SuccessResponse(data));
    }

    [HttpPost("upi/verify-vpa")]
    public async Task<IActionResult> VerifyUpiVpa([FromBody] VerifyUpiVpaDto request)
    {
        var data = await _paymentRequestService.VerifyUpiVpaAsync(request);

        return Ok(ApiResponse<UpiVpaVerificationResponseDto>.SuccessResponse(data));
    }
}
