using FinVault.Application.DTOs.AddressDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAddressService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<AddressDto>> GetAllAsync();
        Task<AddressDto?> GetByIdAsync(int addressId);
        Task<AddressDto?> GetAddressDetailsAsync(int addressId);
        Task<AddressDto> CreateAsync(CreateAddressDto dto);
        Task<AddressDto?> UpdateAsync(UpdateAddressDto dto);
        Task<bool> DeleteAsync(DeleteAddressDto dto);

        // 2. CUSTOMER ADDRESS MANAGEMENT
        Task<IEnumerable<AddressDto>> GetAddressesByCustomerIdAsync(int customerId);
        Task<AddressDto?> GetPrimaryAddressByCustomerIdAsync(int customerId);
        Task<IEnumerable<AddressDto>> GetByCustomerAndAddressTypeAsync(int customerId, string addressType);
        Task<bool> SetPrimaryAddressAsync(int addressId);
        Task<bool> HasAddressOfTypeAsync(int customerId, string addressType);
        Task<bool> HasVerifiedPrimaryAddressAsync(int customerId);

        // 3. COUNTRY AND LOCATION
        Task<IEnumerable<AddressDto>> GetAddressesByCountryIdAsync(int countryId);
        Task<IEnumerable<AddressDto>> GetAddressesByLocationAsync(string state, string city);
        Task<IEnumerable<AddressDto>> GetAddressesByDistrictAsync(string district);
        Task<IEnumerable<AddressDto>> GetAddressesByPinCodeAsync(string pinCode);

        // 4. ADDRESS TYPE AND VERIFICATION
        Task<IEnumerable<AddressDto>> GetAddressesByTypeAsync(string addressType);
        Task<IEnumerable<AddressDto>> GetVerifiedAddressesAsync();
        Task<IEnumerable<AddressDto>> GetUnverifiedAddressesAsync();
        Task<IEnumerable<AddressDto>> GetAddressesVerifiedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> VerifyAddressAsync(int addressId);
        Task<bool> MarkAddressAsUnverifiedAsync(int addressId);

        // 5. SEARCH
        Task<IEnumerable<AddressDto>> SearchAsync(
            string? searchTerm,
            int? customerId,
            int? countryId,
            string? addressType,
            bool? isVerified,
            int pageNumber,
            int pageSize);

        // 6. VALIDATION
        Task<bool> CanAddressBeDeletedAsync(int addressId);
    }
}

//Banking rules: A customer should have only one primary address. 
//Address verification must record VerifiedAt. Validate customer and country existence, and preserve address history where required for KYC and compliance.