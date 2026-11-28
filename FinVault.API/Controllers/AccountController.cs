
using FinVault.Application.DTOs.AccountDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // ==================== ACCOUNT CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _accountService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("{accountId:int}")]
    public async Task<IActionResult> GetById(int accountId)
    {
        var data = await _accountService.GetByIdAsync(accountId);

        return data is null
            ? NotFound(ApiResponse<AccountDto>.FailureResponse("Account not found.", 404))
            : Ok(ApiResponse<AccountDto>.SuccessResponse(data));
    }

    [HttpGet("number/{accountNumber}")]
    public async Task<IActionResult> GetByAccountNumber(string accountNumber)
    {
        var data = await _accountService.GetByAccountNumberAsync(accountNumber);

        return data is null
            ? NotFound(ApiResponse<AccountDto>.FailureResponse("Account not found.", 404))
            : Ok(ApiResponse<AccountDto>.SuccessResponse(data));
    }

    [HttpGet("{accountId:int}/details")]
    public async Task<IActionResult> GetAccountDetails(int accountId)
    {
        var data = await _accountService.GetAccountDetailsAsync(accountId);

        return data is null
            ? NotFound(ApiResponse<AccountDto>.FailureResponse("Account details not found.", 404))
            : Ok(ApiResponse<AccountDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountDto request)
    {
        var data = await _accountService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { accountId = data.AccountId },
            ApiResponse<AccountDto>.SuccessResponse(
                data, "Account created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAccountDto request)
    {
        var data = await _accountService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<AccountDto>.FailureResponse(
                "Account not found or update could not be completed.", 404))
            : Ok(ApiResponse<AccountDto>.SuccessResponse(
                data, "Account updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAccountDto request)
    {
        var result = await _accountService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account could not be deleted. It may be in use.", 409));
    }

    // ==================== ACCOUNT STATUS MANAGEMENT ====================

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetAccountsByStatus(string status)
    {
        var data = await _accountService.GetAccountsByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAccounts()
    {
        var data = await _accountService.GetActiveAccountsAsync();
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpPatch("{accountId:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int accountId,
        [FromQuery] string status,
        [FromQuery] string? statusReason)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Status is required.", 400));

        var result = await _accountService.UpdateStatusAsync(
            accountId, status, statusReason);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account status updated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account status update was rejected.", 409));
    }

    [HttpPatch("{accountId:int}/activate")]
    public async Task<IActionResult> Activate(int accountId)
    {
        var result = await _accountService.ActivateAsync(accountId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account activated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account could not be activated.", 409));
    }

    [HttpPatch("{accountId:int}/freeze")]
    public async Task<IActionResult> Freeze(
        int accountId,
        [FromQuery] string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A freeze reason is required.", 400));

        var result = await _accountService.FreezeAsync(accountId, reason);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account frozen successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account could not be frozen.", 409));
    }

    [HttpPatch("{accountId:int}/unfreeze")]
    public async Task<IActionResult> Unfreeze(int accountId)
    {
        var result = await _accountService.UnfreezeAsync(accountId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account unfrozen successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account could not be unfrozen.", 409));
    }

    [HttpPatch("{accountId:int}/dormant")]
    public async Task<IActionResult> MarkAsDormant(int accountId)
    {
        var result = await _accountService.MarkAsDormantAsync(accountId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account marked as dormant."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account could not be marked as dormant.", 409));
    }

    [HttpPatch("{accountId:int}/close")]
    public async Task<IActionResult> Close(
        int accountId,
        [FromQuery] string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A closure reason is required.", 400));

        var result = await _accountService.CloseAsync(accountId, reason);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account closed successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account could not be closed. Check its balance, holds and pending transactions.", 409));
    }

    // ==================== CUSTOMER AND ACCOUNT HOLDER RELATIONSHIPS ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetAccountsByCustomerId(int customerId)
    {
        var data = await _accountService.GetAccountsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("joint")]
    public async Task<IActionResult> GetJointAccounts()
    {
        var data = await _accountService.GetJointAccountsAsync();
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    // ==================== BRANCH, ACCOUNT TYPE AND CURRENCY ====================

    [HttpGet("branch/{branchId:int}")]
    public async Task<IActionResult> GetAccountsByBranchId(int branchId)
    {
        var data = await _accountService.GetAccountsByBranchIdAsync(branchId);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("account-type/{accountTypeId:int}")]
    public async Task<IActionResult> GetAccountsByAccountTypeId(int accountTypeId)
    {
        var data = await _accountService.GetAccountsByAccountTypeIdAsync(accountTypeId);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("currency/{currencyId:int}")]
    public async Task<IActionResult> GetAccountsByCurrencyId(int currencyId)
    {
        var data = await _accountService.GetAccountsByCurrencyIdAsync(currencyId);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    // ==================== BALANCE AND HOLD MONITORING ====================

    [HttpGet("low-available-balance")]
    public async Task<IActionResult> GetAccountsWithLowAvailableBalance(
        [FromQuery] decimal threshold)
    {
        if (threshold < 0)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Threshold cannot be negative.", 400));

        var data = await _accountService.GetAccountsWithLowAvailableBalanceAsync(threshold);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("with-holds")]
    public async Task<IActionResult> GetAccountsWithHolds()
    {
        var data = await _accountService.GetAccountsWithHoldsAsync();
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    // ==================== DEPOSIT MATURITY AND LINKED ACCOUNTS ====================

    [HttpGet("linked-deposits/{payoutAccountId:int}")]
    public async Task<IActionResult> GetLinkedDepositAccounts(int payoutAccountId)
    {
        var data = await _accountService.GetLinkedDepositAccountsAsync(payoutAccountId);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("maturity-range")]
    public async Task<IActionResult> GetAccountsByMaturityDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _accountService.GetAccountsByMaturityDateRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("matured-deposits")]
    public async Task<IActionResult> GetMaturedDepositAccounts(
        [FromQuery] DateTime asOfDate)
    {
        var data = await _accountService.GetMaturedDepositAccountsAsync(asOfDate);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    // ==================== ACCOUNT ACTIVITY ====================

    [HttpGet("recently-active")]
    public async Task<IActionResult> GetRecentlyActiveAccounts(
        [FromQuery] int count = 10)
    {
        if (count < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be greater than zero.", 400));

        var data = await _accountService.GetRecentlyActiveAccountsAsync(count);
        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND VALIDATION ====================

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int? branchId,
        [FromQuery] int? accountTypeId,
        [FromQuery] int? currencyId,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 500)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 500.", 400));

        var data = await _accountService.SearchAsync(
            searchTerm,
            branchId,
            accountTypeId,
            currencyId,
            status,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(data));
    }

    [HttpGet("exists/number/{accountNumber}")]
    public async Task<IActionResult> IsAccountNumberExists(string accountNumber)
    {
        var result = await _accountService.IsAccountNumberExistsAsync(accountNumber);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{accountId:int}/in-use")]
    public async Task<IActionResult> IsAccountInUse(int accountId)
    {
        var result = await _accountService.IsAccountInUseAsync(accountId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{accountId:int}/can-delete")]
    public async Task<IActionResult> CanAccountBeDeleted(int accountId)
    {
        var result = await _accountService.CanAccountBeDeletedAsync(accountId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
