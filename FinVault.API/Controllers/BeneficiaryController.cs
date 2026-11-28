
using FinVault.Application.DTOs.BeneficiaryDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class BeneficiaryController : ControllerBase
{
    private readonly IBeneficiaryService _beneficiaryService;

    public BeneficiaryController(IBeneficiaryService beneficiaryService)
    {
        _beneficiaryService = beneficiaryService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _beneficiaryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("{beneficiaryId:int}")]
    public async Task<IActionResult> GetById(int beneficiaryId)
    {
        var data = await _beneficiaryService.GetByIdAsync(beneficiaryId);

        return data is null
            ? NotFound(ApiResponse<BeneficiaryDto>.FailureResponse("Beneficiary not found.", 404))
            : Ok(ApiResponse<BeneficiaryDto>.SuccessResponse(data));
    }

    [HttpGet("{beneficiaryId:int}/customer/{customerId:int}")]
    public async Task<IActionResult> GetByIdAndCustomerId(int beneficiaryId, int customerId)
    {
        var data = await _beneficiaryService.GetByIdAndCustomerIdAsync(
            beneficiaryId, customerId);

        return data is null
            ? NotFound(ApiResponse<BeneficiaryDto>.FailureResponse(
                "Beneficiary not found for the specified customer.", 404))
            : Ok(ApiResponse<BeneficiaryDto>.SuccessResponse(data));
    }

    [HttpGet("{beneficiaryId:int}/details")]
    public async Task<IActionResult> GetBeneficiaryDetails(int beneficiaryId)
    {
        var data = await _beneficiaryService.GetBeneficiaryDetailsAsync(beneficiaryId);

        return data is null
            ? NotFound(ApiResponse<BeneficiaryDto>.FailureResponse(
                "Beneficiary details not found.", 404))
            : Ok(ApiResponse<BeneficiaryDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBeneficiaryDto request)
    {
        var data = await _beneficiaryService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { beneficiaryId = data.BeneficiaryId },
            ApiResponse<BeneficiaryDto>.SuccessResponse(
                data, "Beneficiary created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateBeneficiaryDto request)
    {
        var data = await _beneficiaryService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<BeneficiaryDto>.FailureResponse(
                "Beneficiary not found for the specified customer.", 404))
            : Ok(ApiResponse<BeneficiaryDto>.SuccessResponse(
                data, "Beneficiary updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteBeneficiaryDto request)
    {
        var result = await _beneficiaryService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Beneficiary removed successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Beneficiary could not be deleted.", 409));
    }

    // ==================== CUSTOMER BENEFICIARY MANAGEMENT ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetBeneficiariesByCustomerId(int customerId)
    {
        var data = await _beneficiaryService.GetBeneficiariesByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/active")]
    public async Task<IActionResult> GetActiveBeneficiariesByCustomerId(int customerId)
    {
        var data = await _beneficiaryService.GetActiveBeneficiariesByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/pending")]
    public async Task<IActionResult> GetPendingBeneficiariesByCustomerId(int customerId)
    {
        var data = await _beneficiaryService.GetPendingBeneficiariesByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/blocked")]
    public async Task<IActionResult> GetBlockedBeneficiariesByCustomerId(int customerId)
    {
        var data = await _beneficiaryService.GetBlockedBeneficiariesByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/removed")]
    public async Task<IActionResult> GetRemovedBeneficiariesByCustomerId(int customerId)
    {
        var data = await _beneficiaryService.GetRemovedBeneficiariesByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    // ==================== TYPE AND PAYMENT IDENTIFICATION ====================

    [HttpGet("customer/{customerId:int}/type/{beneficiaryType}")]
    public async Task<IActionResult> GetBeneficiariesByType(
        int customerId,
        string beneficiaryType)
    {
        var data = await _beneficiaryService.GetBeneficiariesByTypeAsync(
            customerId, beneficiaryType);

        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/bank")]
    public async Task<IActionResult> GetByAccountAndIfsc(
        int customerId,
        [FromQuery] string accountNumber,
        [FromQuery] string ifscCode)
    {
        var data = await _beneficiaryService.GetByAccountAndIfscAsync(
            customerId, accountNumber, ifscCode);

        return data is null
            ? NotFound(ApiResponse<BeneficiaryDto>.FailureResponse(
                "Bank beneficiary not found.", 404))
            : Ok(ApiResponse<BeneficiaryDto>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/upi")]
    public async Task<IActionResult> GetByUpiVpa(
        int customerId,
        [FromQuery] string upiVpa)
    {
        var data = await _beneficiaryService.GetByUpiVpaAsync(customerId, upiVpa);

        return data is null
            ? NotFound(ApiResponse<BeneficiaryDto>.FailureResponse(
                "UPI beneficiary not found.", 404))
            : Ok(ApiResponse<BeneficiaryDto>.SuccessResponse(data));
    }

    // ==================== FAVORITES AND COOLING PERIOD ====================

    [HttpGet("customer/{customerId:int}/favorites")]
    public async Task<IActionResult> GetFavoriteBeneficiaries(int customerId)
    {
        var data = await _beneficiaryService.GetFavoriteBeneficiariesAsync(customerId);
        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("ready-for-activation")]
    public async Task<IActionResult> GetBeneficiariesReadyForActivation(
        [FromQuery] DateTime currentDateTime)
    {
        if (currentDateTime == default)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid current date and time is required.", 400));

        var data = await _beneficiaryService.GetBeneficiariesReadyForActivationAsync(
            currentDateTime);

        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("cooling-period")]
    public async Task<IActionResult> GetBeneficiariesInCoolingPeriod(
        [FromQuery] DateTime currentDateTime)
    {
        if (currentDateTime == default)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid current date and time is required.", 400));

        var data = await _beneficiaryService.GetBeneficiariesInCoolingPeriodAsync(
            currentDateTime);

        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpPatch("{beneficiaryId:int}/favorite")]
    public async Task<IActionResult> SetFavorite(
        int beneficiaryId,
        [FromQuery] bool isFavorite)
    {
        var result = await _beneficiaryService.SetFavoriteAsync(
            beneficiaryId, isFavorite);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Beneficiary favorite status updated successfully."))
            : NotFound(ApiResponse<object>.FailureResponse(
                "Beneficiary not found.", 404));
    }

    // ==================== INTERNAL ACCOUNT AND TRANSACTION LIMITS ====================

    [HttpGet("internal-account/{internalAccountId:int}")]
    public async Task<IActionResult> GetBeneficiariesByInternalAccountId(int internalAccountId)
    {
        var data = await _beneficiaryService.GetBeneficiariesByInternalAccountIdAsync(
            internalAccountId);

        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/within-transaction-limit")]
    public async Task<IActionResult> GetActiveBeneficiariesWithinTransactionLimit(
        int customerId,
        [FromQuery] decimal amount)
    {
        if (amount <= 0)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Amount must be greater than zero.", 400));

        var data = await _beneficiaryService.GetActiveBeneficiariesWithinTransactionLimitAsync(
            customerId, amount);

        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpPatch("{beneficiaryId:int}/transaction-limit")]
    public async Task<IActionResult> UpdatePerTransactionLimit(
        int beneficiaryId,
        [FromQuery] decimal? limit)
    {
        if (limit.HasValue && limit.Value < 0)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Transaction limit cannot be negative.", 400));

        var result = await _beneficiaryService.UpdatePerTransactionLimitAsync(
            beneficiaryId, limit);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Per-transaction limit updated successfully."))
            : NotFound(ApiResponse<object>.FailureResponse(
                "Beneficiary not found.", 404));
    }

    // ==================== STATUS AND ACTIVATION MANAGEMENT ====================

    [HttpPatch("{beneficiaryId:int}/activate")]
    public async Task<IActionResult> Activate(int beneficiaryId)
    {
        var result = await _beneficiaryService.ActivateAsync(beneficiaryId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Beneficiary activated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Beneficiary could not be activated. It may still be in its cooling period.", 409));
    }

    [HttpPatch("{beneficiaryId:int}/block")]
    public async Task<IActionResult> Block(int beneficiaryId)
    {
        var result = await _beneficiaryService.BlockAsync(beneficiaryId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Beneficiary blocked successfully."))
            : NotFound(ApiResponse<object>.FailureResponse(
                "Beneficiary not found.", 404));
    }

    [HttpPatch("{beneficiaryId:int}/remove")]
    public async Task<IActionResult> Remove(int beneficiaryId)
    {
        var result = await _beneficiaryService.RemoveAsync(beneficiaryId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Beneficiary removed successfully."))
            : NotFound(ApiResponse<object>.FailureResponse(
                "Beneficiary not found.", 404));
    }

    [HttpPatch("{beneficiaryId:int}/cooling-period")]
    public async Task<IActionResult> UpdateCoolingPeriod(
        int beneficiaryId,
        [FromQuery] DateTime coolingPeriodEndsAt)
    {
        if (coolingPeriodEndsAt == default)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid cooling period end date is required.", 400));

        var result = await _beneficiaryService.UpdateCoolingPeriodAsync(
            beneficiaryId, coolingPeriodEndsAt);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Beneficiary cooling period updated successfully."))
            : NotFound(ApiResponse<object>.FailureResponse(
                "Beneficiary not found.", 404));
    }

    // ==================== SEARCH AND VALIDATION ====================

    [HttpGet("customer/{customerId:int}/search")]
    public async Task<IActionResult> Search(
        int customerId,
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        if (pageNumber < 1 || pageSize < 1 || pageSize > 200)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 200.", 400));

        var data = await _beneficiaryService.SearchAsync(
            customerId, searchTerm, pageNumber, pageSize);

        return Ok(ApiResponse<IEnumerable<BeneficiaryDto>>.SuccessResponse(data));
    }

    [HttpGet("{beneficiaryId:int}/customer/{customerId:int}/exists")]
    public async Task<IActionResult> ExistsForCustomer(int beneficiaryId, int customerId)
    {
        var result = await _beneficiaryService.ExistsForCustomerAsync(
            beneficiaryId, customerId);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("customer/{customerId:int}/has-active-bank")]
    public async Task<IActionResult> HasActiveBankBeneficiary(
        int customerId,
        [FromQuery] string accountNumber,
        [FromQuery] string ifscCode)
    {
        var result = await _beneficiaryService.HasActiveBankBeneficiaryAsync(
            customerId, accountNumber, ifscCode);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("customer/{customerId:int}/has-active-upi")]
    public async Task<IActionResult> HasActiveUpiBeneficiary(
        int customerId,
        [FromQuery] string upiVpa)
    {
        var result = await _beneficiaryService.HasActiveUpiBeneficiaryAsync(
            customerId, upiVpa);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{beneficiaryId:int}/can-delete")]
    public async Task<IActionResult> CanBeneficiaryBeDeleted(int beneficiaryId)
    {
        var result = await _beneficiaryService.CanBeneficiaryBeDeletedAsync(beneficiaryId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
