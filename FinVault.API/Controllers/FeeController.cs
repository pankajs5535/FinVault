
using FinVault.Application.DTOs.FeeDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeeController : ControllerBase
{
    private readonly IFeeService _feeService;

    public FeeController(IFeeService feeService)
    {
        _feeService = feeService;
    }

    // 1. FEE IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetAllAsync()));

    [HttpGet("{feeId:int}")]
    public async Task<IActionResult> GetById(int feeId)
    {
        var result = await _feeService.GetByIdAsync(feeId);
        return result == null
            ? NotFound(ApiResponse<FeeDto>.FailureResponse("Fee not found.", 404))
            : Ok(ApiResponse<FeeDto>.SuccessResponse(result));
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await _feeService.GetByCodeAsync(code);
        return result == null
            ? NotFound(ApiResponse<FeeDto>.FailureResponse("Fee not found.", 404))
            : Ok(ApiResponse<FeeDto>.SuccessResponse(result));
    }

    [HttpGet("{feeId:int}/details")]
    public async Task<IActionResult> GetFeeDetails(int feeId)
    {
        var result = await _feeService.GetFeeDetailsAsync(feeId);
        return result == null
            ? NotFound(ApiResponse<FeeDto>.FailureResponse("Fee not found.", 404))
            : Ok(ApiResponse<FeeDto>.SuccessResponse(result));
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetByNameAsync(name)));

    [HttpGet("exists/code")]
    public async Task<IActionResult> ExistsByCode([FromQuery] string code)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _feeService.ExistsByCodeAsync(code)));

    // 2. FEE CRUD

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeeDto dto)
    {
        try
        {
            var result = await _feeService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { feeId = result.FeeId },
                ApiResponse<FeeDto>.SuccessResponse(
                    result, "Fee created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<FeeDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<FeeDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateFeeDto dto)
    {
        try
        {
            var result = await _feeService.UpdateAsync(dto);
            return result == null
                ? NotFound(ApiResponse<FeeDto>.FailureResponse("Fee not found.", 404))
                : Ok(ApiResponse<FeeDto>.SuccessResponse(result, "Fee updated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<FeeDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<FeeDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteFeeDto dto)
    {
        try
        {
            var result = await _feeService.DeleteAsync(dto);
            return result
                ? Ok(ApiResponse<bool>.SuccessResponse(true, "Fee deleted successfully."))
                : Conflict(ApiResponse<bool>.FailureResponse(
                    "Fee could not be deleted because it may be referenced by transactions.", 409));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<bool>.FailureResponse(ex.Message, 409));
        }
    }

    // 3. FEE CLASSIFICATION

    [HttpGet("calculation-method/{calculationMethod}")]
    public async Task<IActionResult> GetByCalculationMethod(string calculationMethod)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
                await _feeService.GetByCalculationMethodAsync(calculationMethod)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<FeeDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("frequency/{frequency}")]
    public async Task<IActionResult> GetByFrequency(string frequency)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
                await _feeService.GetByFrequencyAsync(frequency)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<FeeDto>>.FailureResponse(ex.Message, 400));
        }
    }

    // 4. BANKING ENTITY ASSOCIATIONS

    [HttpGet("transaction-type/{transactionTypeId:int}")]
    public async Task<IActionResult> GetByTransactionTypeId(int transactionTypeId)
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetByTransactionTypeIdAsync(transactionTypeId)));

    [HttpGet("payment-method/{paymentMethodId:int}")]
    public async Task<IActionResult> GetByPaymentMethodId(int paymentMethodId)
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetByPaymentMethodIdAsync(paymentMethodId)));

    [HttpGet("account-type/{accountTypeId:int}")]
    public async Task<IActionResult> GetByAccountTypeId(int accountTypeId)
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetByAccountTypeIdAsync(accountTypeId)));

    // 5. EFFECTIVE DATE MANAGEMENT

    [HttpGet("effective")]
    public async Task<IActionResult> GetEffectiveFees([FromQuery] DateTime effectiveDate)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
                await _feeService.GetEffectiveFeesAsync(effectiveDate)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<FeeDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingFees(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
                await _feeService.GetUpcomingFeesAsync(startDate, endDate)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<FeeDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveFees()
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetActiveFeesAsync()));

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveFees()
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetInactiveFeesAsync()));

    // 6. TAX AND AMOUNT MANAGEMENT

    [HttpGet("gst-applicable")]
    public async Task<IActionResult> GetGstApplicableFees()
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetGstApplicableFeesAsync()));

    [HttpGet("amount-range")]
    public async Task<IActionResult> GetFeesWithinAmountRange(
        [FromQuery] decimal minimumAmount,
        [FromQuery] decimal maximumAmount)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
                await _feeService.GetFeesWithinAmountRangeAsync(minimumAmount, maximumAmount)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<FeeDto>>.FailureResponse(ex.Message, 400));
        }
    }

    // 7. TRANSACTION AND USAGE TRACKING

    [HttpGet("with-charged-transactions")]
    public async Task<IActionResult> GetFeesWithChargedTransactions()
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetFeesWithChargedTransactionsAsync()));

    // 8. AUDIT AND SEARCH

    [HttpGet("created-by/{userId:int}")]
    public async Task<IActionResult> GetFeesCreatedByUser(int userId)
        => Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
            await _feeService.GetFeesCreatedByUserAsync(userId)));

    [HttpGet("search")]
    public async Task<IActionResult> SearchFees([FromQuery] string searchTerm)
    {
        try
        {
            return Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(
                await _feeService.SearchFeesAsync(searchTerm)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<FeeDto>>.FailureResponse(ex.Message, 400));
        }
    }
}
