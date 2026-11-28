
using FinVault.Application.DTOs.AddressDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _addressService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("{addressId:int}")]
    public async Task<IActionResult> GetById(int addressId)
    {
        var data = await _addressService.GetByIdAsync(addressId);

        return data is null
            ? NotFound(ApiResponse<AddressDto>.FailureResponse("Address not found.", 404))
            : Ok(ApiResponse<AddressDto>.SuccessResponse(data));
    }

    [HttpGet("{addressId:int}/details")]
    public async Task<IActionResult> GetAddressDetails(int addressId)
    {
        var data = await _addressService.GetAddressDetailsAsync(addressId);

        return data is null
            ? NotFound(ApiResponse<AddressDto>.FailureResponse("Address details not found.", 404))
            : Ok(ApiResponse<AddressDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressDto request)
    {
        var data = await _addressService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { addressId = data.AddressId },
            ApiResponse<AddressDto>.SuccessResponse(
                data, "Address created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAddressDto request)
    {
        var data = await _addressService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<AddressDto>.FailureResponse("Address not found.", 404))
            : Ok(ApiResponse<AddressDto>.SuccessResponse(
                data, "Address updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAddressDto request)
    {
        var result = await _addressService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Address deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Address does not exist or cannot be deleted.", 409));
    }

    // ==================== CUSTOMER ADDRESS MANAGEMENT ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetAddressesByCustomerId(int customerId)
    {
        var data = await _addressService.GetAddressesByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/primary")]
    public async Task<IActionResult> GetPrimaryAddressByCustomerId(int customerId)
    {
        var data = await _addressService.GetPrimaryAddressByCustomerIdAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<AddressDto>.FailureResponse(
                "Primary address not found.", 404))
            : Ok(ApiResponse<AddressDto>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/type/{addressType}")]
    public async Task<IActionResult> GetByCustomerAndAddressType(
        int customerId,
        string addressType)
    {
        var data = await _addressService.GetByCustomerAndAddressTypeAsync(
            customerId, addressType);

        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpPatch("{addressId:int}/set-primary")]
    public async Task<IActionResult> SetPrimaryAddress(int addressId)
    {
        var result = await _addressService.SetPrimaryAddressAsync(addressId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Primary address updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Unable to set the primary address.", 400));
    }

    [HttpGet("customer/{customerId:int}/has-address-type/{addressType}")]
    public async Task<IActionResult> HasAddressOfType(
        int customerId,
        string addressType)
    {
        var result = await _addressService.HasAddressOfTypeAsync(
            customerId, addressType);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("customer/{customerId:int}/has-verified-primary")]
    public async Task<IActionResult> HasVerifiedPrimaryAddress(int customerId)
    {
        var result = await _addressService.HasVerifiedPrimaryAddressAsync(customerId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== COUNTRY AND LOCATION ====================

    [HttpGet("country/{countryId:int}")]
    public async Task<IActionResult> GetAddressesByCountryId(int countryId)
    {
        var data = await _addressService.GetAddressesByCountryIdAsync(countryId);
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("location")]
    public async Task<IActionResult> GetAddressesByLocation(
        [FromQuery] string state,
        [FromQuery] string city)
    {
        var data = await _addressService.GetAddressesByLocationAsync(state, city);
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("district/{district}")]
    public async Task<IActionResult> GetAddressesByDistrict(string district)
    {
        var data = await _addressService.GetAddressesByDistrictAsync(district);
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("pincode/{pinCode}")]
    public async Task<IActionResult> GetAddressesByPinCode(string pinCode)
    {
        var data = await _addressService.GetAddressesByPinCodeAsync(pinCode);
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    // ==================== ADDRESS TYPE AND VERIFICATION ====================

    [HttpGet("type/{addressType}")]
    public async Task<IActionResult> GetAddressesByType(string addressType)
    {
        var data = await _addressService.GetAddressesByTypeAsync(addressType);
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("verified")]
    public async Task<IActionResult> GetVerifiedAddresses()
    {
        var data = await _addressService.GetVerifiedAddressesAsync();
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("unverified")]
    public async Task<IActionResult> GetUnverifiedAddresses()
    {
        var data = await _addressService.GetUnverifiedAddressesAsync();
        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpGet("verified/date-range")]
    public async Task<IActionResult> GetAddressesVerifiedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _addressService.GetAddressesVerifiedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    [HttpPatch("{addressId:int}/verify")]
    public async Task<IActionResult> VerifyAddress(int addressId)
    {
        var result = await _addressService.VerifyAddressAsync(addressId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Address verified successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Address verification failed.", 400));
    }

    [HttpPatch("{addressId:int}/mark-unverified")]
    public async Task<IActionResult> MarkAddressAsUnverified(int addressId)
    {
        var result = await _addressService.MarkAddressAsUnverifiedAsync(addressId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Address marked as unverified successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Unable to mark address as unverified.", 400));
    }

    // ==================== SEARCH ====================

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int? customerId,
        [FromQuery] int? countryId,
        [FromQuery] string? addressType,
        [FromQuery] bool? isVerified,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 ||
            (customerId.HasValue && customerId.Value <= 0) ||
            (countryId.HasValue && countryId.Value <= 0))
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid pagination, customer ID or country ID.", 400));
        }

        var data = await _addressService.SearchAsync(
            searchTerm,
            customerId,
            countryId,
            addressType,
            isVerified,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<AddressDto>>.SuccessResponse(data));
    }

    // ==================== VALIDATION ====================

    [HttpGet("{addressId:int}/can-delete")]
    public async Task<IActionResult> CanAddressBeDeleted(int addressId)
    {
        var result = await _addressService.CanAddressBeDeletedAsync(addressId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
