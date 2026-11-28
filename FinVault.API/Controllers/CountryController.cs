
using FinVault.Application.DTOs.CountryDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _countryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    [HttpGet("{countryId:int}")]
    public async Task<IActionResult> GetById(int countryId)
    {
        var data = await _countryService.GetByIdAsync(countryId);

        return data is null
            ? NotFound(ApiResponse<CountryDto>.FailureResponse("Country not found.", 404))
            : Ok(ApiResponse<CountryDto>.SuccessResponse(data));
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var data = await _countryService.GetByNameAsync(name);

        return data is null
            ? NotFound(ApiResponse<CountryDto>.FailureResponse("Country not found.", 404))
            : Ok(ApiResponse<CountryDto>.SuccessResponse(data));
    }

    [HttpGet("iso-alpha2/{isoAlpha2Code}")]
    public async Task<IActionResult> GetByIsoAlpha2Code(string isoAlpha2Code)
    {
        var data = await _countryService.GetByIsoAlpha2CodeAsync(isoAlpha2Code);

        return data is null
            ? NotFound(ApiResponse<CountryDto>.FailureResponse("Country not found.", 404))
            : Ok(ApiResponse<CountryDto>.SuccessResponse(data));
    }

    [HttpGet("iso-alpha3/{isoAlpha3Code}")]
    public async Task<IActionResult> GetByIsoAlpha3Code(string isoAlpha3Code)
    {
        var data = await _countryService.GetByIsoAlpha3CodeAsync(isoAlpha3Code);

        return data is null
            ? NotFound(ApiResponse<CountryDto>.FailureResponse("Country not found.", 404))
            : Ok(ApiResponse<CountryDto>.SuccessResponse(data));
    }

    [HttpGet("numeric-code/{numericCode}")]
    public async Task<IActionResult> GetByNumericCode(string numericCode)
    {
        var data = await _countryService.GetByNumericCodeAsync(numericCode);

        return data is null
            ? NotFound(ApiResponse<CountryDto>.FailureResponse("Country not found.", 404))
            : Ok(ApiResponse<CountryDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCountryDto request)
    {
        var data = await _countryService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { countryId = data.CountryId },
            ApiResponse<CountryDto>.SuccessResponse(
                data, "Country created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCountryDto request)
    {
        var data = await _countryService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<CountryDto>.FailureResponse("Country not found.", 404))
            : Ok(ApiResponse<CountryDto>.SuccessResponse(data, "Country updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCountryDto request)
    {
        var result = await _countryService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Country deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Country does not exist or cannot be deleted because it is in use.", 409));
    }

    // ==================== PHONE CODE AND STATUS ====================

    [HttpGet("phone-code/{phoneCode}")]
    public async Task<IActionResult> GetCountriesByPhoneCode(string phoneCode)
    {
        var data = await _countryService.GetCountriesByPhoneCodeAsync(phoneCode);
        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCountries()
    {
        var data = await _countryService.GetActiveCountriesAsync();
        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    [HttpPatch("{countryId:int}/activate")]
    public async Task<IActionResult> Activate(int countryId)
    {
        var result = await _countryService.ActivateAsync(countryId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Country activated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Country does not exist or is already active.", 409));
    }

    [HttpPatch("{countryId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int countryId)
    {
        var result = await _countryService.DeactivateAsync(countryId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Country deactivated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Country does not exist or is already inactive.", 409));
    }

    // ==================== COUNTRY RELATIONSHIPS ====================

    [HttpGet("with-branches")]
    public async Task<IActionResult> GetCountriesWithBranches()
    {
        var data = await _countryService.GetCountriesWithBranchesAsync();
        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    [HttpGet("with-currencies")]
    public async Task<IActionResult> GetCountriesWithCurrencies()
    {
        var data = await _countryService.GetCountriesWithCurrenciesAsync();
        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    [HttpGet("with-customers")]
    public async Task<IActionResult> GetCountriesWithCustomers()
    {
        var data = await _countryService.GetCountriesWithCustomersAsync();
        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    [HttpGet("{countryId:int}/details")]
    public async Task<IActionResult> GetCountryDetails(int countryId)
    {
        var data = await _countryService.GetCountryDetailsAsync(countryId);

        return data is null
            ? NotFound(ApiResponse<CountryDto>.FailureResponse("Country not found.", 404))
            : Ok(ApiResponse<CountryDto>.SuccessResponse(data));
    }

    // ==================== SEARCH AND ADMINISTRATION ====================

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number and page size must be greater than zero.", 400));

        var data = await _countryService.SearchAsync(
            searchTerm, isActive, pageNumber, pageSize);

        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentlyAdded([FromQuery] int count = 10)
    {
        if (count < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be greater than zero.", 400));

        var data = await _countryService.GetRecentlyAddedCountriesAsync(count);
        return Ok(ApiResponse<IEnumerable<CountryDto>>.SuccessResponse(data));
    }

    // ==================== USAGE AND VALIDATION ====================

    [HttpGet("{countryId:int}/usage-summary")]
    public async Task<IActionResult> GetCountryUsageSummary(int countryId)
    {
        var data = await _countryService.GetCountryUsageSummaryAsync(countryId);

        return data is null
            ? NotFound(ApiResponse<CountryUsageSummaryDto>.FailureResponse(
                "Country usage summary not found.", 404))
            : Ok(ApiResponse<CountryUsageSummaryDto>.SuccessResponse(data));
    }

    [HttpGet("{countryId:int}/in-use")]
    public async Task<IActionResult> IsCountryInUse(int countryId)
    {
        var result = await _countryService.IsCountryInUseAsync(countryId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/iso-alpha2/{isoAlpha2Code}")]
    public async Task<IActionResult> IsIsoAlpha2CodeExists(string isoAlpha2Code)
    {
        var result = await _countryService.IsIsoAlpha2CodeExistsAsync(isoAlpha2Code);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/iso-alpha3/{isoAlpha3Code}")]
    public async Task<IActionResult> IsIsoAlpha3CodeExists(string isoAlpha3Code)
    {
        var result = await _countryService.IsIsoAlpha3CodeExistsAsync(isoAlpha3Code);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/numeric-code/{numericCode}")]
    public async Task<IActionResult> IsNumericCodeExists(string numericCode)
    {
        var result = await _countryService.IsNumericCodeExistsAsync(numericCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> IsCountryNameExists(string name)
    {
        var result = await _countryService.IsCountryNameExistsAsync(name);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{countryId:int}/can-delete")]
    public async Task<IActionResult> CanCountryBeDeleted(int countryId)
    {
        var result = await _countryService.CanCountryBeDeletedAsync(countryId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
