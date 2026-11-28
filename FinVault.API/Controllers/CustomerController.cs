
using FinVault.Application.DTOs.CustomerDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // ==================== CUSTOMER CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _customerService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetById(int customerId)
    {
        var data = await _customerService.GetByIdAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpGet("number/{customerNumber}")]
    public async Task<IActionResult> GetByCustomerNumber(string customerNumber)
    {
        var data = await _customerService.GetByCustomerNumberAsync(customerNumber);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpGet("mobile/{mobileNumber}")]
    public async Task<IActionResult> GetByMobileNumber(string mobileNumber)
    {
        var data = await _customerService.GetByMobileNumberAsync(mobileNumber);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpGet("email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var data = await _customerService.GetByEmailAsync(email);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpGet("{customerId:int}/details")]
    public async Task<IActionResult> GetCustomerDetails(int customerId)
    {
        var data = await _customerService.GetCustomerDetailsAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer details not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto request)
    {
        var data = await _customerService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { customerId = data.CustomerId },
            ApiResponse<CustomerDto>.SuccessResponse(
                data, "Customer created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCustomerDto request)
    {
        var data = await _customerService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(
                data, "Customer updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCustomerDto request)
    {
        var result = await _customerService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Customer deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Customer could not be deleted because it does not exist or has associated records.", 409));
    }

    // ==================== CUSTOMER CLASSIFICATION ====================

    [HttpGet("type/{customerType}")]
    public async Task<IActionResult> GetCustomersByType(string customerType)
    {
        var data = await _customerService.GetCustomersByTypeAsync(customerType);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("gender/{gender}")]
    public async Task<IActionResult> GetCustomersByGender(string gender)
    {
        var data = await _customerService.GetCustomersByGenderAsync(gender);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("risk-category/{riskCategory}")]
    public async Task<IActionResult> GetCustomersByRiskCategory(string riskCategory)
    {
        var data = await _customerService.GetCustomersByRiskCategoryAsync(riskCategory);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetCustomersByStatus(string status)
    {
        var data = await _customerService.GetCustomersByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    // ==================== BRANCH AND RELATIONSHIP MANAGEMENT ====================

    [HttpGet("home-branch/{homeBranchId:int}")]
    public async Task<IActionResult> GetCustomersByHomeBranchId(int homeBranchId)
    {
        var data = await _customerService.GetCustomersByHomeBranchIdAsync(homeBranchId);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("relationship-manager/{employeeId:int}")]
    public async Task<IActionResult> GetCustomersByRelationshipManagerId(int employeeId)
    {
        var data = await _customerService.GetCustomersByRelationshipManagerIdAsync(employeeId);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("nationality-country/{countryId:int}")]
    public async Task<IActionResult> GetCustomersByNationalityCountryId(int countryId)
    {
        var data = await _customerService.GetCustomersByNationalityCountryIdAsync(countryId);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("{customerId:int}/branch-and-manager")]
    public async Task<IActionResult> GetCustomerWithBranchAndManager(int customerId)
    {
        var data = await _customerService.GetCustomerWithBranchAndManagerAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    // ==================== ACCOUNT, KYC AND ADDRESS RELATIONSHIPS ====================

    [HttpGet("{customerId:int}/accounts")]
    public async Task<IActionResult> GetCustomerWithAccounts(int customerId)
    {
        var data = await _customerService.GetCustomerWithAccountsAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpGet("{customerId:int}/kyc-history")]
    public async Task<IActionResult> GetCustomerWithKycHistory(int customerId)
    {
        var data = await _customerService.GetCustomerWithKycHistoryAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpGet("{customerId:int}/addresses")]
    public async Task<IActionResult> GetCustomerWithAddresses(int customerId)
    {
        var data = await _customerService.GetCustomerWithAddressesAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<CustomerDto>.FailureResponse("Customer not found.", 404))
            : Ok(ApiResponse<CustomerDto>.SuccessResponse(data));
    }

    [HttpGet("online-banking")]
    public async Task<IActionResult> GetCustomersWithOnlineBanking()
    {
        var data = await _customerService.GetCustomersWithOnlineBankingAsync();
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    // ==================== ONBOARDING AND AUDIT HISTORY ====================

    [HttpGet("onboarded-within-range")]
    public async Task<IActionResult> GetCustomersOnboardedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _customerService.GetCustomersOnboardedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("recently-onboarded")]
    public async Task<IActionResult> GetRecentlyOnboardedCustomers(
        [FromQuery] int count = 10)
    {
        if (count < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be greater than zero.", 400));

        var data = await _customerService.GetRecentlyOnboardedCustomersAsync(count);
        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("updated-within-range")]
    public async Task<IActionResult> GetCustomersUpdatedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _customerService.GetCustomersUpdatedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND VALIDATION ====================

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] string? status,
        [FromQuery] string? riskCategory,
        [FromQuery] int? homeBranchId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 ||
            (homeBranchId.HasValue && homeBranchId.Value <= 0))
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid pagination or home branch ID.", 400));
        }

        var data = await _customerService.SearchAsync(
            searchTerm,
            status,
            riskCategory,
            homeBranchId,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResponse(data));
    }

    [HttpGet("exists/customer-number/{customerNumber}")]
    public async Task<IActionResult> ExistsByCustomerNumber(string customerNumber)
    {
        var result = await _customerService.ExistsByCustomerNumberAsync(customerNumber);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/mobile/{mobileNumber}")]
    public async Task<IActionResult> ExistsByMobileNumber(string mobileNumber)
    {
        var result = await _customerService.ExistsByMobileNumberAsync(mobileNumber);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/email")]
    public async Task<IActionResult> ExistsByEmail([FromQuery] string email)
    {
        var result = await _customerService.ExistsByEmailAsync(email);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{customerId:int}/account-count")]
    public async Task<IActionResult> CountCustomerAccounts(int customerId)
    {
        var count = await _customerService.CountCustomerAccountsAsync(customerId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }
}
