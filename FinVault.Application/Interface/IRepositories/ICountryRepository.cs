
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.CountryDtos;

namespace FinVault.Application.Interface.IRepositories;

public interface ICountryRepository : IGenericRepository<Country>
{
    // =====================================================
    // 1. COUNTRY IDENTIFICATION
    // =====================================================

    // Finds a country using its 2-letter ISO code.
    // Example: IN, US, GB.
    // Used in customer addresses, KYC and country identification.
    Task<Country?> GetByIsoAlpha2CodeAsync(string isoAlpha2Code);


    // Finds a country using its 3-letter ISO code.
    // Example: IND, USA, GBR.
    // Useful when external systems provide 3-letter country codes.
    Task<Country?> GetByIsoAlpha3CodeAsync(string isoAlpha3Code);


    // Finds a country using its numeric ISO code.
    // Example: 356 (India), 840 (United States).
    // Useful for standardized country identification and integrations.
    Task<Country?> GetByNumericCodeAsync(string numericCode);


    // Finds a country using its full name.
    // Example: India, United States.
    // Used for country validation and selection.
    Task<Country?> GetByNameAsync(string name);


    // Finds countries using their international telephone calling code.
    // Example: +91 for India.
    // Useful during customer mobile number registration.
    Task<IEnumerable<Country>> GetCountriesByPhoneCodeAsync(string phoneCode);


    // =====================================================
    // 2. COUNTRY STATUS AND SEARCH
    // =====================================================

    // Returns only active countries.
    // Used in customer, KYC, address and branch dropdowns.
    // Prevents inactive countries from being selected for new records.
    Task<IEnumerable<Country>> GetActiveCountriesAsync();


    // Searches countries by name or ISO codes.
    // Supports active-status filtering and pagination.
    // Useful for country administration screens.
    Task<IEnumerable<Country>> SearchCountriesAsync(
        string? searchTerm,
        bool? isActive,
        int pageNumber,
        int pageSize);


    // Returns recently created country records.
    // Uses CreatedAt to support administrative monitoring.
    Task<IEnumerable<Country>> GetRecentlyAddedCountriesAsync(int count);


    // =====================================================
    // 3. COUNTRY RELATIONSHIPS
    // =====================================================

    // Returns countries that have associated bank branches.
    // Useful for branch administration and operational reporting.
    Task<IEnumerable<Country>> GetCountriesWithBranchesAsync();


    // Returns countries that have associated currencies.
    // Useful for country and currency management.
    Task<IEnumerable<Country>> GetCountriesWithCurrenciesAsync();


    // Returns countries that have associated customers.
    // Useful for customer administration and geographical analysis.
    Task<IEnumerable<Country>> GetCountriesWithCustomersAsync();


    // Retrieves one country with its related branches and currencies.
    // Useful for viewing complete country details in administration.
    Task<Country?> GetCountryDetailsAsync(int countryId);


    // =====================================================
    // 4. COUNTRY VALIDATION AND USAGE
    // =====================================================

    // Checks whether a country is referenced by customers,
    // addresses, branches or currencies.
    // Useful before deactivation or deletion.
    Task<bool> IsCountryInUseAsync(int countryId);


    // Retrieves country-related usage statistics.
    // Useful for administrative dashboards and reporting.
    // Requires a dedicated CountryUsageSummaryDto.
    Task<CountryUsageSummaryDto?> GetCountryUsageSummaryAsync(int countryId);
}
