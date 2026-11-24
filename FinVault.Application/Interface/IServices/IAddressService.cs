
using FinVault.Application.DTOs.AddressDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAddressService
    {
        
        // 1. CRUD OPERATIONS
        

        Task<IEnumerable<AddressDto>> GetAllAsync();

        Task<AddressDto?> GetByIdAsync(int addressId);

        Task<AddressDto?> GetAddressDetailsAsync(int addressId);

        Task<AddressDto> CreateAsync(CreateAddressDto dto);

        Task<AddressDto?> UpdateAsync(UpdateAddressDto dto);

        Task<bool> DeleteAsync(int addressId);

        Task<bool> DeleteByCustomerAsync(int addressId, int customerId);


        
        // 2. CUSTOMER ADDRESS MANAGEMENT
        

        Task<IEnumerable<AddressDto>> GetByCustomerIdAsync(int customerId);

        Task<AddressDto?> GetPrimaryAddressByCustomerIdAsync(int customerId);

        Task<AddressDto?> GetByCustomerAndAddressTypeAsync(int customerId, string addressType);

        Task<bool> SetPrimaryAddressAsync(int addressId, int customerId);

        Task<bool> HasAddressOfTypeAsync(int customerId, string addressType);


        
        // 3. ADDRESS TYPE OPERATIONS
        

        Task<IEnumerable<AddressDto>> GetByAddressTypeAsync(string addressType);

        Task<IEnumerable<AddressDto>> GetResidentialAddressesAsync();

        Task<IEnumerable<AddressDto>> GetPermanentAddressesAsync();

        Task<IEnumerable<AddressDto>> GetCorrespondenceAddressesAsync();

        Task<IEnumerable<AddressDto>> GetOfficeAddressesAsync();


        
        // 4. ADDRESS VERIFICATION AND KYC
        

        Task<IEnumerable<AddressDto>> GetVerifiedAddressesAsync();

        Task<IEnumerable<AddressDto>> GetUnverifiedAddressesAsync();

        Task<IEnumerable<AddressDto>> GetVerifiedAddressesWithinRangeAsync(DateTime startDate, DateTime endDate);

        Task<bool> VerifyAddressAsync(int addressId);

        Task<bool> RejectAddressVerificationAsync(int addressId);

        Task<bool> HasVerifiedPrimaryAddressAsync(int customerId);


        
        // 5. LOCATION AND POSTAL SEARCH
        

        Task<IEnumerable<AddressDto>> GetByCountryIdAsync(int countryId);

        Task<IEnumerable<AddressDto>> GetByLocationAsync(string state, string city);

        Task<IEnumerable<AddressDto>> GetByDistrictAsync(string district);

        Task<IEnumerable<AddressDto>> GetByPinCodeAsync(string pinCode);


        
        // 6. SEARCH AND FILTER
        

        Task<IEnumerable<AddressDto>> SearchAsync(
            string? searchTerm,
            int? customerId,
            int? countryId,
            string? addressType,
            bool? isVerified,
            int pageNumber,
            int pageSize);


        
        // 7. VALIDATION OPERATIONS
        

        Task<bool> IsAddressOwnedByCustomerAsync(int addressId, int customerId);

        Task<bool> IsPinCodeValidAsync(string pinCode);

        Task<bool> HasPrimaryAddressAsync(int customerId);

        Task<bool> CanDeleteAddressAsync(int addressId);
    }
}
