
using FinVault.Application.DTOs.PaymentMethodDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodController(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _paymentMethodService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("{paymentMethodId:int}")]
    public async Task<IActionResult> GetById(int paymentMethodId)
    {
        var data = await _paymentMethodService.GetByIdAsync(paymentMethodId);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(data));
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var data = await _paymentMethodService.GetByCodeAsync(code);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(data));
    }

    [HttpGet("name")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(ApiResponse<object>.FailureResponse("Name is required.", 400));

        var data = await _paymentMethodService.GetByNameAsync(name);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(data));
    }

    [HttpGet("{paymentMethodId:int}/details")]
    public async Task<IActionResult> GetPaymentMethodDetails(int paymentMethodId)
    {
        var data = await _paymentMethodService.GetPaymentMethodDetailsAsync(paymentMethodId);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method details not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentMethodDto request)
    {
        var data = await _paymentMethodService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { paymentMethodId = data.PaymentMethodId },
            ApiResponse<PaymentMethodDto>.SuccessResponse(
                data, "Payment method created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePaymentMethodDto request)
    {
        var data = await _paymentMethodService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(
                data, "Payment method updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletePaymentMethodDto request)
    {
        var result = await _paymentMethodService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Payment method deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Payment method cannot be deleted because it is referenced by payment operations.", 409));
    }

    [HttpGet("exists/code/{code}")]
    public async Task<IActionResult> ExistsByCode(string code)
    {
        var result = await _paymentMethodService.ExistsByCodeAsync(code);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== RAIL AND SETTLEMENT CLASSIFICATION ====================

    [HttpGet("rail/{rail}")]
    public async Task<IActionResult> GetByRail(string rail)
    {
        var data = await _paymentMethodService.GetByRailAsync(rail);
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("settlement-type/{settlementType}")]
    public async Task<IActionResult> GetBySettlementType(string settlementType)
    {
        var data = await _paymentMethodService.GetBySettlementTypeAsync(settlementType);
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("available-24x7")]
    public async Task<IActionResult> Get24x7PaymentMethods()
    {
        var data = await _paymentMethodService.Get24x7PaymentMethodsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    // ==================== AVAILABILITY AND STATUS ====================

    [HttpGet("active")]
    public async Task<IActionResult> GetActivePaymentMethods()
    {
        var data = await _paymentMethodService.GetActivePaymentMethodsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactivePaymentMethods()
    {
        var data = await _paymentMethodService.GetInactivePaymentMethodsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("available-for-amount")]
    public async Task<IActionResult> GetAvailableMethodsForAmount([FromQuery] decimal amount)
    {
        if (amount <= 0)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Amount must be greater than zero.", 400));

        var data = await _paymentMethodService.GetAvailableMethodsForAmountAsync(amount);
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    // ==================== LIMIT AND SETTLEMENT CONFIGURATION ====================

    [HttpGet("minimum-amount-range")]
    public async Task<IActionResult> GetByMinimumAmountRange(
        [FromQuery] decimal minimumAmount,
        [FromQuery] decimal maximumAmount)
    {
        if (minimumAmount < 0 || maximumAmount < minimumAmount)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid amount range. Minimum must be non-negative and cannot exceed maximum.", 400));

        var data = await _paymentMethodService.GetByMinimumAmountRangeAsync(
            minimumAmount, maximumAmount);

        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("daily-limit-above")]
    public async Task<IActionResult> GetWithDailyLimitAbove([FromQuery] decimal minimumDailyLimit)
    {
        if (minimumDailyLimit < 0)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Minimum daily limit cannot be negative.", 400));

        var data = await _paymentMethodService.GetWithDailyLimitAboveAsync(minimumDailyLimit);
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("without-maximum-amount")]
    public async Task<IActionResult> GetWithoutMaximumAmount()
    {
        var data = await _paymentMethodService.GetWithoutMaximumAmountAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    // ==================== RELATED PAYMENT OPERATIONS ====================

    [HttpGet("{paymentMethodId:int}/payment-requests")]
    public async Task<IActionResult> GetWithPaymentRequests(int paymentMethodId)
    {
        var data = await _paymentMethodService.GetWithPaymentRequestsAsync(paymentMethodId);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(data));
    }

    [HttpGet("{paymentMethodId:int}/fees")]
    public async Task<IActionResult> GetWithFees(int paymentMethodId)
    {
        var data = await _paymentMethodService.GetWithFeesAsync(paymentMethodId);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(data));
    }

    [HttpGet("{paymentMethodId:int}/reconciliation-batches")]
    public async Task<IActionResult> GetWithReconciliationBatches(int paymentMethodId)
    {
        var data = await _paymentMethodService.GetWithReconciliationBatchesAsync(paymentMethodId);

        return data is null
            ? NotFound(ApiResponse<PaymentMethodDto>.FailureResponse("Payment method not found.", 404))
            : Ok(ApiResponse<PaymentMethodDto>.SuccessResponse(data));
    }

    [HttpGet("with-payment-requests")]
    public async Task<IActionResult> GetMethodsWithPaymentRequests()
    {
        var data = await _paymentMethodService.GetMethodsWithPaymentRequestsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("with-fees")]
    public async Task<IActionResult> GetMethodsWithFees()
    {
        var data = await _paymentMethodService.GetMethodsWithFeesAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("with-reconciliation-batches")]
    public async Task<IActionResult> GetMethodsWithReconciliationBatches()
    {
        var data = await _paymentMethodService.GetMethodsWithReconciliationBatchesAsync();
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    // ==================== ADMINISTRATION AND SEARCH ====================

    [HttpGet("created-within-range")]
    public async Task<IActionResult> GetCreatedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _paymentMethodService.GetCreatedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchPaymentMethods([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        var data = await _paymentMethodService.SearchPaymentMethodsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<PaymentMethodDto>>.SuccessResponse(data));
    }

    [HttpGet("{paymentMethodId:int}/in-use")]
    public async Task<IActionResult> IsPaymentMethodInUse(int paymentMethodId)
    {
        var result = await _paymentMethodService.IsPaymentMethodInUseAsync(paymentMethodId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
