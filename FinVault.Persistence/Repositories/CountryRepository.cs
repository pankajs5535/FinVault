
using FinVault.Application.DTOs.CountryDtos;
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class CountryRepository : GenericRepository<Country>, ICountryRepository
{
    public CountryRepository(FinVaultDbContext context) : base(context)
    {
    }

    // 1. COUNTRY IDENTIFICATION


    // Finds a country using its 2-letter ISO code.
    // Example: IN, US, GB.
    // Used for customer addresses and KYC identification.
    public async Task<Country?> GetByIsoAlpha2CodeAsync(string isoAlpha2Code)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.IsoAlpha2Code == isoAlpha2Code);
    }

    // Finds a country using its 3-letter ISO code.
    // Example: IND, USA, GBR.
    // Useful for external system integrations.
    public async Task<Country?> GetByIsoAlpha3CodeAsync(string isoAlpha3Code)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.IsoAlpha3Code == isoAlpha3Code);
    }

    // Finds a country using its numeric ISO code.
    // Example: 356 (India), 840 (United States).
    // Useful for standardized country identification.
    public async Task<Country?> GetByNumericCodeAsync(string numericCode)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.NumericCode == numericCode);
    }

    // Finds a country using its full name.
    // Useful for country validation and selection.
    public async Task<Country?> GetByNameAsync(string name)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name);
    }

    // Finds countries using their international phone code.
    // Example: +91 for India.
    // Useful during customer mobile number registration.
    public async Task<IEnumerable<Country>> GetCountriesByPhoneCodeAsync(string phoneCode)
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.PhoneCode == phoneCode)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }


    // 2. COUNTRY STATUS AND SEARCH


    // Returns only active countries.
    // Used in customer, KYC, address and branch dropdowns.
    public async Task<IEnumerable<Country>> GetActiveCountriesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Searches countries by name, ISO codes, numeric code or phone code.
    // Supports optional active-status filtering and pagination.
    public async Task<IEnumerable<Country>> SearchCountriesAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            query = query.Where(c =>
                c.Name.Contains(searchTerm) ||
                c.IsoAlpha2Code.Contains(searchTerm) ||
                c.IsoAlpha3Code.Contains(searchTerm) ||
                c.NumericCode.Contains(searchTerm) ||
                (c.PhoneCode != null && c.PhoneCode.Contains(searchTerm)));
        }

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        return await query.OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Returns recently created country records.
    // Useful for administrative monitoring.
    public async Task<IEnumerable<Country>> GetRecentlyAddedCountriesAsync(int count)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        return await _dbSet.AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    // 3. COUNTRY RELATIONSHIPS

    // Returns countries that have associated bank branches.
    public async Task<IEnumerable<Country>> GetCountriesWithBranchesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.Branches.Any())
            .Include(c => c.Branches)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Returns countries that have associated currencies.
    public async Task<IEnumerable<Country>> GetCountriesWithCurrenciesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.Currencies.Any())
            .Include(c => c.Currencies)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Returns countries that have associated customers.
    public async Task<IEnumerable<Country>> GetCountriesWithCustomersAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.Customers.Any())
            .Include(c => c.Customers)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Retrieves country details with associated branches and currencies.
    public async Task<Country?> GetCountryDetailsAsync(int countryId)
    {
        return await _dbSet.AsNoTracking()
            .Include(c => c.Branches)
            .Include(c => c.Currencies)
            .FirstOrDefaultAsync(c => c.CountryId == countryId);
    }


    // 4. COUNTRY VALIDATION AND USAGE

    // Checks whether a country is referenced by customers,
    // addresses, branches or currencies.
    public async Task<bool> IsCountryInUseAsync(int countryId)
    {
        return await _dbSet.AnyAsync(c =>
            c.CountryId == countryId &&
            (c.Customers.Any() ||
             c.Addresses.Any() ||
             c.Branches.Any() ||
             c.Currencies.Any()));
    }

    // Returns country usage counts for administrative reporting.
    // Retrieves counts without loading all related records.
    public async Task<CountryUsageSummaryDto?> GetCountryUsageSummaryAsync(int countryId)
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.CountryId == countryId)
            .Select(c => new CountryUsageSummaryDto
            {
                CountryId = c.CountryId,
                CountryName = c.Name,
                TotalCustomers = c.Customers.Count(),
                TotalBranches = c.Branches.Count(),
                TotalCurrencies = c.Currencies.Count()
            })
            .FirstOrDefaultAsync();
    }
}
