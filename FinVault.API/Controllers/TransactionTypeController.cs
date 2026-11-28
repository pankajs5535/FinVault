
using FinVault.Application.DTOs.TransactionTypeDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TransactionTypeController : ControllerBase
{
    private readonly ITransactionTypeService _transactionTypeService;

    public TransactionTypeController(ITransactionTypeService transactionTypeService)
    {
        _transactionTypeService = transactionTypeService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _transactionTypeService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("{transactionTypeId:int}")]
    public async Task<IActionResult> GetById(int transactionTypeId)
    {
        var data = await _transactionTypeService.GetByIdAsync(transactionTypeId);

        return data is null
            ? NotFound(ApiResponse<TransactionTypeDto>.FailureResponse("Transaction type not found.", 404))
            : Ok(ApiResponse<TransactionTypeDto>.SuccessResponse(data));
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var data = await _transactionTypeService.GetByCodeAsync(code);

        return data is null
            ? NotFound(ApiResponse<TransactionTypeDto>.FailureResponse("Transaction type not found.", 404))
            : Ok(ApiResponse<TransactionTypeDto>.SuccessResponse(data));
    }

    [HttpGet("name")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(ApiResponse<object>.FailureResponse("Name is required.", 400));

        var data = await _transactionTypeService.GetByNameAsync(name);
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("{transactionTypeId:int}/details")]
    public async Task<IActionResult> GetTransactionTypeDetails(int transactionTypeId)
    {
        var data = await _transactionTypeService.GetTransactionTypeDetailsAsync(transactionTypeId);

        return data is null
            ? NotFound(ApiResponse<TransactionTypeDto>.FailureResponse("Transaction type details not found.", 404))
            : Ok(ApiResponse<TransactionTypeDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionTypeDto request)
    {
        var data = await _transactionTypeService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { transactionTypeId = data.TransactionTypeId },
            ApiResponse<TransactionTypeDto>.SuccessResponse(
                data, "Transaction type created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTransactionTypeDto request)
    {
        var data = await _transactionTypeService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<TransactionTypeDto>.FailureResponse("Transaction type not found.", 404))
            : Ok(ApiResponse<TransactionTypeDto>.SuccessResponse(
                data, "Transaction type updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteTransactionTypeDto request)
    {
        var result = await _transactionTypeService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Transaction type deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Transaction type could not be deleted because it may be in use.", 409));
    }

    // ==================== CATEGORY AND STATUS ====================

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var data = await _transactionTypeService.GetByCategoryAsync(category);
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveTransactionTypes()
    {
        var data = await _transactionTypeService.GetActiveTransactionTypesAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveTransactionTypes()
    {
        var data = await _transactionTypeService.GetInactiveTransactionTypesAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpPatch("{transactionTypeId:int}/activate")]
    public async Task<IActionResult> Activate(int transactionTypeId)
    {
        var result = await _transactionTypeService.ActivateAsync(transactionTypeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Transaction type activated successfully."))
            : NotFound(ApiResponse<object>.FailureResponse("Transaction type not found.", 404));
    }

    [HttpPatch("{transactionTypeId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int transactionTypeId)
    {
        var result = await _transactionTypeService.DeactivateAsync(transactionTypeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Transaction type deactivated successfully."))
            : NotFound(ApiResponse<object>.FailureResponse("Transaction type not found.", 404));
    }

    // ==================== APPROVAL AND REVERSAL RULES ====================

    [HttpGet("approval-required")]
    public async Task<IActionResult> GetApprovalRequiredTransactionTypes()
    {
        var data = await _transactionTypeService.GetApprovalRequiredTransactionTypesAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("reversible")]
    public async Task<IActionResult> GetReversibleTransactionTypes()
    {
        var data = await _transactionTypeService.GetReversibleTransactionTypesAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("non-reversible")]
    public async Task<IActionResult> GetNonReversibleTransactionTypes()
    {
        var data = await _transactionTypeService.GetNonReversibleTransactionTypesAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("with-approval-threshold")]
    public async Task<IActionResult> GetTransactionTypesWithApprovalThreshold()
    {
        var data = await _transactionTypeService.GetTransactionTypesWithApprovalThresholdAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("with-daily-limit")]
    public async Task<IActionResult> GetTransactionTypesWithDailyLimit()
    {
        var data = await _transactionTypeService.GetTransactionTypesWithDailyLimitAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    // ==================== BUSINESS RELATIONSHIPS ====================

    [HttpGet("with-transactions")]
    public async Task<IActionResult> GetTransactionTypesWithTransactions()
    {
        var data = await _transactionTypeService.GetTransactionTypesWithTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("with-fees")]
    public async Task<IActionResult> GetTransactionTypesWithFees()
    {
        var data = await _transactionTypeService.GetTransactionTypesWithFeesAsync();
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND VALIDATION ====================

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentlyCreatedTransactionTypes(
        [FromQuery] int count = 10)
    {
        if (count < 1 || count > 500)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be between 1 and 500.", 400));

        var data = await _transactionTypeService.GetRecentlyCreatedTransactionTypesAsync(count);
        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTransactionTypes(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] bool? isActive,
        [FromQuery] bool? requiresApproval,
        [FromQuery] bool? isReversible,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 500)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 500.", 400));

        var data = await _transactionTypeService.SearchTransactionTypesAsync(
            searchTerm,
            category,
            isActive,
            requiresApproval,
            isReversible,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<TransactionTypeDto>>.SuccessResponse(data));
    }

    [HttpGet("{transactionTypeId:int}/in-use")]
    public async Task<IActionResult> IsTransactionTypeInUse(int transactionTypeId)
    {
        var result = await _transactionTypeService.IsTransactionTypeInUseAsync(transactionTypeId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/code/{code}")]
    public async Task<IActionResult> ExistsByCode(string code)
    {
        var result = await _transactionTypeService.ExistsByCodeAsync(code);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
