
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories;

public interface IAddressRepository : IGenericRepository<Address>
{
    // =====================================================
    // 1. ADDRESS IDENTIFICATION
    // =====================================================

    // Finds an address using its unique address ID.
    // Useful for retrieving a specific customer address.
    Task<Address?> GetAddressDetailsAsync(int addressId);

    // Retrieves an address belonging to a particular customer and address type.
    // Useful for finding residential, permanent or correspondence addresses.
    Task<IEnumerable<Address>> GetByCustomerAndAddressTypeAsync(int customerId, string addressType);


    // =====================================================
    // 2. CUSTOMER ADDRESS RELATIONSHIPS
    // =====================================================

    // Retrieves all addresses associated with a customer.
    // Useful for displaying the customer's complete address information.
    Task<IEnumerable<Address>> GetAddressesByCustomerIdAsync(int customerId);

    // Retrieves the primary address of a customer.
    // Useful for statements, correspondence and customer communication.
    Task<Address?> GetPrimaryAddressByCustomerIdAsync(int customerId);

    // Retrieves all addresses belonging to a particular country.
    // Useful for country-based address administration and reporting.
    Task<IEnumerable<Address>> GetAddressesByCountryIdAsync(int countryId);

    // Retrieves addresses belonging to a particular city and state.
    // Useful for geographic customer searches and regional reporting.
    Task<IEnumerable<Address>> GetAddressesByLocationAsync(string state, string city);


    // =====================================================
    // 3. ADDRESS TYPE AND VERIFICATION
    // =====================================================

    // Retrieves addresses belonging to a particular address type.
    // Useful for separating residential, permanent, correspondence and office addresses.
    Task<IEnumerable<Address>> GetAddressesByTypeAsync(string addressType);

    // Retrieves all verified addresses.
    // Useful for identifying customer addresses that have passed verification.
    Task<IEnumerable<Address>> GetVerifiedAddressesAsync();

    // Retrieves all addresses that have not been verified.
    // Useful for KYC follow-up and address verification workflows.
    Task<IEnumerable<Address>> GetUnverifiedAddressesAsync();

    // Retrieves addresses verified within a date range.
    // Useful for monitoring address verification activity and audit reporting.
    Task<IEnumerable<Address>> GetAddressesVerifiedWithinRangeAsync(DateTime startDate, DateTime endDate);


    // =====================================================
    // 4. PIN CODE AND LOCATION SEARCH
    // =====================================================

    // Retrieves addresses using their six-digit PIN code.
    // Useful for finding customers within a postal delivery area.
    Task<IEnumerable<Address>> GetAddressesByPinCodeAsync(string pinCode);

    // Retrieves addresses using a district name.
    // Useful for district-level customer and branch-related reporting.
    Task<IEnumerable<Address>> GetAddressesByDistrictAsync(string district);


    // =====================================================
    // 5. ADDRESS SEARCH AND VALIDATION
    // =====================================================

    // Searches addresses by address lines, landmark, city, district, state or PIN code.
    // Supports optional customer, country, address type and verification filters.
    // Pagination prevents loading large address lists at once.
    Task<IEnumerable<Address>> SearchAddressesAsync(string? searchTerm, int? customerId, int? countryId, string? addressType, bool? isVerified, int pageNumber, int pageSize);

    // Checks whether a customer already has an address of a particular type.
    // Useful for preventing duplicate address records during customer registration.
    Task<bool> HasAddressOfTypeAsync(int customerId, string addressType);

    // Checks whether a customer has a verified primary address.
    // Useful for customer onboarding and KYC validation.
    Task<bool> HasVerifiedPrimaryAddressAsync(int customerId);
}
