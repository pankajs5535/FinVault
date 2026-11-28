
using FinVault.Application.DTOs.AccountHolderDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AccountHolderController : ControllerBase
{
    private readonly IAccountHolderService _accountHolderService;

    public AccountHolderController(IAccountHolderService accountHolderService)
    {
        _accountHolderService = accountHolderService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _accountHolderService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("{accountHolderId:int}")]
    public async Task<IActionResult> GetById(int accountHolderId)
    {
        var data = await _accountHolderService.GetByIdAsync(accountHolderId);

        return data is null
            ? NotFound(ApiResponse<AccountHolderDto>.FailureResponse("Account holder not found.", 404))
            : Ok(ApiResponse<AccountHolderDto>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/customer/{customerId:int}")]
    public async Task<IActionResult> GetByAccountAndCustomer(int accountId, int customerId)
    {
        var data = await _accountHolderService.GetByAccountAndCustomerAsync(accountId, customerId);

        return data is null
            ? NotFound(ApiResponse<AccountHolderDto>.FailureResponse("Account holder relationship not found.", 404))
            : Ok(ApiResponse<AccountHolderDto>.SuccessResponse(data));
    }

    [HttpGet("{accountHolderId:int}/details")]
    public async Task<IActionResult> GetAccountHolderDetails(int accountHolderId)
    {
        var data = await _accountHolderService.GetAccountHolderDetailsAsync(accountHolderId);

        return data is null
            ? NotFound(ApiResponse<AccountHolderDto>.FailureResponse("Account holder details not found.", 404))
            : Ok(ApiResponse<AccountHolderDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountHolderDto request)
    {
        var data = await _accountHolderService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { accountHolderId = data.AccountHolderId },
            ApiResponse<AccountHolderDto>.SuccessResponse(data, "Account holder created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAccountHolderDto request)
    {
        var data = await _accountHolderService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<AccountHolderDto>.FailureResponse("Account holder not found.", 404))
            : Ok(ApiResponse<AccountHolderDto>.SuccessResponse(data, "Account holder updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAccountHolderDto request)
    {
        var result = await _accountHolderService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account holder deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse("Account holder could not be deleted.", 409));
    }

    // ==================== ACCOUNT AND CUSTOMER RELATIONSHIPS ====================

    [HttpGet("account/{accountId:int}")]
    public async Task<IActionResult> GetAccountHoldersByAccountId(int accountId)
    {
        var data = await _accountHolderService.GetAccountHoldersByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetAccountHoldersByCustomerId(int customerId)
    {
        var data = await _accountHolderService.GetAccountHoldersByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    // ==================== STATUS AND HOLDER CLASSIFICATION ====================

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAccountHolders()
    {
        var data = await _accountHolderService.GetActiveAccountHoldersAsync();
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveAccountHolders()
    {
        var data = await _accountHolderService.GetInactiveAccountHoldersAsync();
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("primary")]
    public async Task<IActionResult> GetPrimaryAccountHolders()
    {
        var data = await _accountHolderService.GetPrimaryAccountHoldersAsync();
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("joint")]
    public async Task<IActionResult> GetJointAccountHolders()
    {
        var data = await _accountHolderService.GetJointAccountHoldersAsync();
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("guardians")]
    public async Task<IActionResult> GetGuardianAccountHolders()
    {
        var data = await _accountHolderService.GetGuardianAccountHoldersAsync();
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("authorized-signatories")]
    public async Task<IActionResult> GetAuthorizedSignatories()
    {
        var data = await _accountHolderService.GetAuthorizedSignatoriesAsync();
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/operators")]
    public async Task<IActionResult> GetAccountOperators(int accountId)
    {
        var data = await _accountHolderService.GetAccountOperatorsAsync(accountId);
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    // ==================== EFFECTIVE PERIOD MANAGEMENT ====================

    [HttpGet("effective-on")]
    public async Task<IActionResult> GetAccountHoldersEffectiveOnDate(
        [FromQuery] DateTime effectiveDate)
    {
        if (effectiveDate == default)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid effective date is required.", 400));

        var data = await _accountHolderService.GetAccountHoldersEffectiveOnDateAsync(effectiveDate);
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("ending-within-range")]
    public async Task<IActionResult> GetAccountHoldersEndingWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate.Date < startDate.Date)
            return BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));

        var data = await _accountHolderService.GetAccountHoldersEndingWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    // ==================== PERMISSIONS AND STATUS MANAGEMENT ====================

    [HttpPatch("{accountHolderId:int}/activate")]
    public async Task<IActionResult> Activate(int accountHolderId)
    {
        var result = await _accountHolderService.ActivateAsync(accountHolderId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account holder activated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse("Account holder could not be activated.", 409));
    }

    [HttpPatch("{accountHolderId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int accountHolderId)
    {
        var result = await _accountHolderService.DeactivateAsync(accountHolderId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account holder deactivated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse("Account holder could not be deactivated.", 409));
    }

    [HttpPatch("{accountHolderId:int}/operating-permission")]
    public async Task<IActionResult> UpdateOperatingPermission(
        int accountHolderId,
        [FromQuery] bool canOperate)
    {
        var result = await _accountHolderService.UpdateOperatingPermissionAsync(accountHolderId, canOperate);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Operating permission updated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse("Operating permission could not be updated.", 409));
    }

    [HttpPatch("{accountHolderId:int}/effective-period")]
    public async Task<IActionResult> UpdateEffectivePeriod(
        int accountHolderId,
        [FromQuery] DateTime effectiveFrom,
        [FromQuery] DateTime? effectiveTo)
    {
        if (effectiveFrom == default ||
            (effectiveTo.HasValue && effectiveTo.Value.Date < effectiveFrom.Date))
        {
            return BadRequest(ApiResponse<object>.FailureResponse("Invalid effective period.", 400));
        }

        var result = await _accountHolderService.UpdateEffectivePeriodAsync(
            accountHolderId, effectiveFrom, effectiveTo);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Effective period updated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse("Effective period could not be updated.", 409));
    }

    // ==================== SEARCH AND VALIDATION ====================

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] int? accountId,
        [FromQuery] int? customerId,
        [FromQuery] string? holderType,
        [FromQuery] bool? isActive,
        [FromQuery] bool? canOperate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 500)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 500.", 400));
        }

        var data = await _accountHolderService.SearchAsync(
            accountId, customerId, holderType, isActive, canOperate, pageNumber, pageSize);

        return Ok(ApiResponse<IEnumerable<AccountHolderDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/customer/{customerId:int}/is-active")]
    public async Task<IActionResult> IsActiveAccountHolder(int accountId, int customerId)
    {
        var result = await _accountHolderService.IsActiveAccountHolderAsync(accountId, customerId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("account/{accountId:int}/customer/{customerId:int}/can-operate")]
    public async Task<IActionResult> CanCustomerOperateAccount(int accountId, int customerId)
    {
        var result = await _accountHolderService.CanCustomerOperateAccountAsync(accountId, customerId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
