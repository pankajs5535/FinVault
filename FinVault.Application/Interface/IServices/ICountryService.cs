using FinVault.Application.DTOs.CountryDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICountryService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<CountryDto>> GetAllAsync();
        Task<CountryDto?> GetByIdAsync(int countryId);
        Task<CountryDto?> GetByNameAsync(string name);
        Task<CountryDto?> GetByIsoAlpha2CodeAsync(string isoAlpha2Code);
        Task<CountryDto?> GetByIsoAlpha3CodeAsync(string isoAlpha3Code);
        Task<CountryDto?> GetByNumericCodeAsync(string numericCode);
        Task<CountryDto> CreateAsync(CreateCountryDto dto);
        Task<CountryDto?> UpdateAsync(UpdateCountryDto dto);
        Task<bool> DeleteAsync(DeleteCountryDto dto);

        // 2. PHONE CODE AND STATUS
        Task<IEnumerable<CountryDto>> GetCountriesByPhoneCodeAsync(string phoneCode);
        Task<IEnumerable<CountryDto>> GetActiveCountriesAsync();
        Task<bool> ActivateAsync(int countryId);
        Task<bool> DeactivateAsync(int countryId);

        // 3. COUNTRY RELATIONSHIPS
        Task<IEnumerable<CountryDto>> GetCountriesWithBranchesAsync();
        Task<IEnumerable<CountryDto>> GetCountriesWithCurrenciesAsync();
        Task<IEnumerable<CountryDto>> GetCountriesWithCustomersAsync();
        Task<CountryDto?> GetCountryDetailsAsync(int countryId);

        // 4. SEARCH AND ADMINISTRATION
        Task<IEnumerable<CountryDto>> SearchAsync(
            string? searchTerm,
            bool? isActive,
            int pageNumber,
            int pageSize);

        Task<IEnumerable<CountryDto>> GetRecentlyAddedCountriesAsync(int count);

        // 5. USAGE AND VALIDATION
        Task<CountryUsageSummaryDto?> GetCountryUsageSummaryAsync(int countryId);
        Task<bool> IsCountryInUseAsync(int countryId);
        Task<bool> IsIsoAlpha2CodeExistsAsync(string isoAlpha2Code);
        Task<bool> IsIsoAlpha3CodeExistsAsync(string isoAlpha3Code);
        Task<bool> IsNumericCodeExistsAsync(string numericCode);
        Task<bool> IsCountryNameExistsAsync(string name);
        Task<bool> CanCountryBeDeletedAsync(int countryId);
    }
}