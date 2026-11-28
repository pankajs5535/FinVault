
using FinVault.Application.DTOs.AccountTypeDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AccountTypeController : ControllerBase
{
    private readonly IAccountTypeService _accountTypeService;

    public AccountTypeController(IAccountTypeService accountTypeService)
    {
        _accountTypeService = accountTypeService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _accountTypeService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("{accountTypeId:int}")]
    public async Task<IActionResult> GetById(int accountTypeId)
    {
        var data = await _accountTypeService.GetByIdAsync(accountTypeId);

        return data is null
            ? NotFound(ApiResponse<AccountTypeDto>.FailureResponse("Account type not found.", 404))
            : Ok(ApiResponse<AccountTypeDto>.SuccessResponse(data));
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var data = await _accountTypeService.GetByCodeAsync(code);

        return data is null
            ? NotFound(ApiResponse<AccountTypeDto>.FailureResponse("Account type not found.", 404))
            : Ok(ApiResponse<AccountTypeDto>.SuccessResponse(data));
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var data = await _accountTypeService.GetByNameAsync(name);
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("{accountTypeId:int}/details")]
    public async Task<IActionResult> GetAccountTypeDetails(int accountTypeId)
    {
        var data = await _accountTypeService.GetAccountTypeDetailsAsync(accountTypeId);

        return data is null
            ? NotFound(ApiResponse<AccountTypeDto>.FailureResponse("Account type details not found.", 404))
            : Ok(ApiResponse<AccountTypeDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountTypeDto request)
    {
        var data = await _accountTypeService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { accountTypeId = data.AccountTypeId },
            ApiResponse<AccountTypeDto>.SuccessResponse(
                data, "Account type created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAccountTypeDto request)
    {
        var data = await _accountTypeService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<AccountTypeDto>.FailureResponse("Account type not found.", 404))
            : Ok(ApiResponse<AccountTypeDto>.SuccessResponse(
                data, "Account type updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAccountTypeDto request)
    {
        var result = await _accountTypeService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account type deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account type does not exist or is currently in use.", 409));
    }

    // ==================== CATEGORY AND STATUS ====================

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var data = await _accountTypeService.GetByCategoryAsync(category);
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAccountTypes()
    {
        var data = await _accountTypeService.GetActiveAccountTypesAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveAccountTypes()
    {
        var data = await _accountTypeService.GetInactiveAccountTypesAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpPatch("{accountTypeId:int}/activate")]
    public async Task<IActionResult> Activate(int accountTypeId)
    {
        var result = await _accountTypeService.ActivateAsync(accountTypeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account type activated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account type could not be activated.", 409));
    }

    [HttpPatch("{accountTypeId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int accountTypeId)
    {
        var result = await _accountTypeService.DeactivateAsync(accountTypeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Account type deactivated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Account type could not be deactivated because it may be in use.", 409));
    }

    // ==================== ACCOUNT PRODUCT FEATURES ====================

    [HttpGet("interest-bearing")]
    public async Task<IActionResult> GetInterestBearingAccountTypes()
    {
        var data = await _accountTypeService.GetInterestBearingAccountTypesAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("overdraft")]
    public async Task<IActionResult> GetOverdraftAccountTypes()
    {
        var data = await _accountTypeService.GetOverdraftAccountTypesAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("tenure-range")]
    public async Task<IActionResult> GetAccountTypesByTenureRange(
        [FromQuery] int minTenureMonths,
        [FromQuery] int maxTenureMonths)
    {
        if (minTenureMonths < 0 || maxTenureMonths < 0 ||
            minTenureMonths > maxTenureMonths)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid tenure range. Minimum tenure cannot exceed maximum tenure.", 400));
        }

        var data = await _accountTypeService.GetAccountTypesByTenureRangeAsync(
            minTenureMonths, maxTenureMonths);

        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    // ==================== BUSINESS RELATIONSHIPS ====================

    [HttpGet("with-accounts")]
    public async Task<IActionResult> GetAccountTypesWithAccounts()
    {
        var data = await _accountTypeService.GetAccountTypesWithAccountsAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("with-interest-rates")]
    public async Task<IActionResult> GetAccountTypesWithInterestRates()
    {
        var data = await _accountTypeService.GetAccountTypesWithInterestRatesAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("with-fees")]
    public async Task<IActionResult> GetAccountTypesWithFees()
    {
        var data = await _accountTypeService.GetAccountTypesWithFeesAsync();
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND ADMINISTRATION ====================

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentlyCreatedAccountTypes(
        [FromQuery] int count = 10)
    {
        if (count < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be greater than zero.", 400));

        var data = await _accountTypeService.GetRecentlyCreatedAccountTypesAsync(count);
        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] bool? isActive,
        [FromQuery] bool? isInterestBearing,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 500)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 500.", 400));
        }

        var data = await _accountTypeService.SearchAsync(
            searchTerm,
            category,
            isActive,
            isInterestBearing,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<AccountTypeDto>>.SuccessResponse(data));
    }

    // ==================== VALIDATION AND USAGE ====================

    [HttpGet("exists/code/{code}")]
    public async Task<IActionResult> IsCodeExists(string code)
    {
        var result = await _accountTypeService.IsCodeExistsAsync(code);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> IsNameExists(string name)
    {
        var result = await _accountTypeService.IsNameExistsAsync(name);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{accountTypeId:int}/in-use")]
    public async Task<IActionResult> IsAccountTypeInUse(int accountTypeId)
    {
        var result = await _accountTypeService.IsAccountTypeInUseAsync(accountTypeId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{accountTypeId:int}/can-delete")]
    public async Task<IActionResult> CanAccountTypeBeDeleted(int accountTypeId)
    {
        var result = await _accountTypeService.CanAccountTypeBeDeletedAsync(accountTypeId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
