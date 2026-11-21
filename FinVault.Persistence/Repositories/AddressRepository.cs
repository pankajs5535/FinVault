
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class AddressRepository : GenericRepository<Address>, IAddressRepository
{
    public AddressRepository(FinVaultDbContext context) : base(context)
    {
    }

    // =====================================================
    // 1. ADDRESS IDENTIFICATION
    // =====================================================

    // Finds an address using its unique address ID.
    // Useful for retrieving a specific customer address.
    public async Task<Address?> GetAddressDetailsAsync(int addressId)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Country)
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser)
            .FirstOrDefaultAsync(x => x.AddressId == addressId);
    }

    // Retrieves an address belonging to a particular customer and address type.
    // Useful for finding residential, permanent or correspondence addresses.
    public async Task<IEnumerable<Address>> GetByCustomerAndAddressTypeAsync(int customerId, string addressType)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.AddressType == addressType)
            .OrderByDescending(x => x.IsPrimary)
            .ToListAsync();
    }


    // =====================================================
    // 2. CUSTOMER ADDRESS RELATIONSHIPS
    // =====================================================

    // Retrieves all addresses associated with a customer.
    // Useful for displaying the customer's complete address information.
    public async Task<IEnumerable<Address>> GetAddressesByCustomerIdAsync(int customerId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.AddressType)
            .ToListAsync();
    }

    // Retrieves the primary address of a customer.
    // Useful for statements, correspondence and customer communication.
    public async Task<Address?> GetPrimaryAddressByCustomerIdAsync(int customerId)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsPrimary);
    }

    // Retrieves all addresses belonging to a particular country.
    // Useful for country-based address administration and reporting.
    public async Task<IEnumerable<Address>> GetAddressesByCountryIdAsync(int countryId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.State)
            .ThenBy(x => x.City)
            .ToListAsync();
    }

    // Retrieves addresses belonging to a particular city and state.
    // Useful for geographic customer searches and regional reporting.
    public async Task<IEnumerable<Address>> GetAddressesByLocationAsync(string state, string city)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.State == state && x.City == city)
            .OrderBy(x => x.PinCode)
            .ToListAsync();
    }


    // =====================================================
    // 3. ADDRESS TYPE AND VERIFICATION
    // =====================================================

    // Retrieves addresses belonging to a particular address type.
    // Useful for separating residential, permanent, correspondence and office addresses.
    public async Task<IEnumerable<Address>> GetAddressesByTypeAsync(string addressType)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AddressType == addressType)
            .OrderBy(x => x.CustomerId)
            .ToListAsync();
    }

    // Retrieves all verified addresses.
    // Useful for identifying customer addresses that have passed verification.
    public async Task<IEnumerable<Address>> GetVerifiedAddressesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.IsVerified)
            .OrderByDescending(x => x.VerifiedAt)
            .ToListAsync();
    }

    // Retrieves all addresses that have not been verified.
    // Useful for KYC follow-up and address verification workflows.
    public async Task<IEnumerable<Address>> GetUnverifiedAddressesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => !x.IsVerified)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    // Retrieves addresses verified within a date range.
    // Useful for monitoring address verification activity and audit reporting.
    public async Task<IEnumerable<Address>> GetAddressesVerifiedWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");

        return await _dbSet.AsNoTracking()
            .Where(x => x.IsVerified && x.VerifiedAt >= startDate && x.VerifiedAt <= endDate)
            .OrderByDescending(x => x.VerifiedAt)
            .ToListAsync();
    }


    // =====================================================
    // 4. PIN CODE AND LOCATION SEARCH
    // =====================================================

    // Retrieves addresses using their six-digit PIN code.
    // Useful for finding customers within a postal delivery area.
    public async Task<IEnumerable<Address>> GetAddressesByPinCodeAsync(string pinCode)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.PinCode == pinCode)
            .OrderBy(x => x.City)
            .ToListAsync();
    }

    // Retrieves addresses using a district name.
    // Useful for district-level customer and branch-related reporting.
    public async Task<IEnumerable<Address>> GetAddressesByDistrictAsync(string district)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.District == district)
            .OrderBy(x => x.City)
            .ToListAsync();
    }


    // =====================================================
    // 5. ADDRESS SEARCH AND VALIDATION
    // =====================================================

    // Searches addresses by address lines, landmark, city, district, state or PIN code.
    // Supports optional customer, country, address type and verification filters.
    // Pagination prevents loading large address lists at once.
    public async Task<IEnumerable<Address>> SearchAddressesAsync(string? searchTerm, int? customerId, int? countryId, string? addressType, bool? isVerified, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            query = query.Where(x =>
                x.AddressLine1.Contains(searchTerm) ||
                (x.AddressLine2 != null && x.AddressLine2.Contains(searchTerm)) ||
                (x.Landmark != null && x.Landmark.Contains(searchTerm)) ||
                x.City.Contains(searchTerm) ||
                (x.District != null && x.District.Contains(searchTerm)) ||
                x.State.Contains(searchTerm) ||
                x.PinCode.Contains(searchTerm));
        }

        if (customerId.HasValue)
            query = query.Where(x => x.CustomerId == customerId.Value);

        if (countryId.HasValue)
            query = query.Where(x => x.CountryId == countryId.Value);

        if (!string.IsNullOrWhiteSpace(addressType))
            query = query.Where(x => x.AddressType == addressType.Trim());

        if (isVerified.HasValue)
            query = query.Where(x => x.IsVerified == isVerified.Value);

        return await query.OrderBy(x => x.CustomerId)
            .ThenByDescending(x => x.IsPrimary)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Checks whether a customer already has an address of a particular type.
    // Useful for preventing duplicate address records during customer registration.
    public async Task<bool> HasAddressOfTypeAsync(int customerId, string addressType)
    {
        return await _dbSet.AnyAsync(x => x.CustomerId == customerId && x.AddressType == addressType);
    }

    // Checks whether a customer has a verified primary address.
    // Useful for customer onboarding and KYC validation.
    public async Task<bool> HasVerifiedPrimaryAddressAsync(int customerId)
    {
        return await _dbSet.AnyAsync(x =>
            x.CustomerId == customerId && x.IsPrimary && x.IsVerified);
    }
}
