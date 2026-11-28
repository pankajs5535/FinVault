
using FinVault.Application.DTOs.CurrencyDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _currencyService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    [HttpGet("{currencyId:int}")]
    public async Task<IActionResult> GetById(int currencyId)
    {
        var data = await _currencyService.GetByIdAsync(currencyId);

        return data is null
            ? NotFound(ApiResponse<CurrencyDto>.FailureResponse("Currency not found.", 404))
            : Ok(ApiResponse<CurrencyDto>.SuccessResponse(data));
    }

    [HttpGet("code/{currencyCode}")]
    public async Task<IActionResult> GetByCurrencyCode(string currencyCode)
    {
        var data = await _currencyService.GetByCurrencyCodeAsync(currencyCode);

        return data is null
            ? NotFound(ApiResponse<CurrencyDto>.FailureResponse("Currency not found.", 404))
            : Ok(ApiResponse<CurrencyDto>.SuccessResponse(data));
    }

    [HttpGet("numeric-code/{numericCode}")]
    public async Task<IActionResult> GetByNumericCode(string numericCode)
    {
        var data = await _currencyService.GetByNumericCodeAsync(numericCode);

        return data is null
            ? NotFound(ApiResponse<CurrencyDto>.FailureResponse("Currency not found.", 404))
            : Ok(ApiResponse<CurrencyDto>.SuccessResponse(data));
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var data = await _currencyService.GetByNameAsync(name);

        return data is null
            ? NotFound(ApiResponse<CurrencyDto>.FailureResponse("Currency not found.", 404))
            : Ok(ApiResponse<CurrencyDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCurrencyDto request)
    {
        var data = await _currencyService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { currencyId = data.CurrencyId },
            ApiResponse<CurrencyDto>.SuccessResponse(
                data, "Currency created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCurrencyDto request)
    {
        var data = await _currencyService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<CurrencyDto>.FailureResponse("Currency not found.", 404))
            : Ok(ApiResponse<CurrencyDto>.SuccessResponse(
                data, "Currency updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCurrencyDto request)
    {
        var result = await _currencyService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Currency deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Currency does not exist or cannot be deleted because it is in use.", 409));
    }

    // ==================== CURRENCY SYMBOL AND STATUS ====================

    [HttpGet("symbol/{symbol}")]
    public async Task<IActionResult> GetCurrenciesBySymbol(string symbol)
    {
        var data = await _currencyService.GetCurrenciesBySymbolAsync(symbol);
        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    [HttpGet("base")]
    public async Task<IActionResult> GetBaseCurrency()
    {
        var data = await _currencyService.GetBaseCurrencyAsync();

        return data is null
            ? NotFound(ApiResponse<CurrencyDto>.FailureResponse(
                "Base currency not found.", 404))
            : Ok(ApiResponse<CurrencyDto>.SuccessResponse(data));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCurrencies()
    {
        var data = await _currencyService.GetActiveCurrenciesAsync();
        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    // ==================== COUNTRY RELATIONSHIP ====================

    [HttpGet("country/{countryId:int}")]
    public async Task<IActionResult> GetCurrenciesByCountryId(int countryId)
    {
        var data = await _currencyService.GetCurrenciesByCountryIdAsync(countryId);
        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    [HttpGet("{currencyId:int}/details")]
    public async Task<IActionResult> GetCurrencyDetails(int currencyId)
    {
        var data = await _currencyService.GetCurrencyDetailsAsync(currencyId);

        return data is null
            ? NotFound(ApiResponse<CurrencyDto>.FailureResponse(
                "Currency not found.", 404))
            : Ok(ApiResponse<CurrencyDto>.SuccessResponse(data));
    }

    // ==================== ACCOUNT AND TRANSACTION RELATIONSHIPS ====================

    [HttpGet("used-by-accounts")]
    public async Task<IActionResult> GetCurrenciesUsedByAccounts()
    {
        var data = await _currencyService.GetCurrenciesUsedByAccountsAsync();
        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    [HttpGet("used-by-transactions")]
    public async Task<IActionResult> GetCurrenciesUsedByTransactions()
    {
        var data = await _currencyService.GetCurrenciesUsedByTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND ADMINISTRATION ====================

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int? countryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 ||
            (countryId.HasValue && countryId.Value <= 0))
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number and page size must be greater than zero. Country ID must be positive.", 400));
        }

        var data = await _currencyService.SearchAsync(
            searchTerm, isActive, countryId, pageNumber, pageSize);

        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentlyAddedCurrencies(
        [FromQuery] int count = 10)
    {
        if (count < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be greater than zero.", 400));

        var data = await _currencyService.GetRecentlyAddedCurrenciesAsync(count);
        return Ok(ApiResponse<IEnumerable<CurrencyDto>>.SuccessResponse(data));
    }

    // ==================== VALIDATION AND USAGE ====================

    [HttpGet("exists/code/{currencyCode}")]
    public async Task<IActionResult> IsCurrencyCodeExists(string currencyCode)
    {
        var result = await _currencyService.IsCurrencyCodeExistsAsync(currencyCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/numeric-code/{numericCode}")]
    public async Task<IActionResult> IsNumericCodeExists(string numericCode)
    {
        var result = await _currencyService.IsNumericCodeExistsAsync(numericCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> IsCurrencyNameExists(string name)
    {
        var result = await _currencyService.IsCurrencyNameExistsAsync(name);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{currencyId:int}/in-use")]
    public async Task<IActionResult> IsCurrencyInUse(int currencyId)
    {
        var result = await _currencyService.IsCurrencyInUseAsync(currencyId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{currencyId:int}/can-delete")]
    public async Task<IActionResult> CanCurrencyBeDeleted(int currencyId)
    {
        var result = await _currencyService.CanCurrencyBeDeletedAsync(currencyId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
