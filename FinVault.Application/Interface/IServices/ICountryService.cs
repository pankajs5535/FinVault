
using FinVault.Application.DTOs.CountryDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICountryService
    {
        // 1. CRUD OPERATIONS
        Task<IReadOnlyList<CountryDto>> GetAllAsync();

        Task<CountryDto?> GetByIdAsync(int countryId);

        Task<CountryDto?> GetByNameAsync(string name);

        Task<CountryDto?> GetByIsoAlpha2CodeAsync(string isoAlpha2Code);

        Task<CountryDto?> GetByIsoAlpha3CodeAsync(string isoAlpha3Code);

        Task<CountryDto?> GetByNumericCodeAsync(string numericCode);

        Task<CountryDto> CreateAsync(CreateCountryDto dto);

        Task<CountryDto?> UpdateAsync(int countryId, UpdateCountryDto dto);

        Task<bool> DeleteAsync(int countryId);

        Task DeleteMultipleAsync(IEnumerable<int> countryIds);


  
        // 2. STATUS OPERATIONS
        Task<IReadOnlyList<CountryDto>> GetActiveCountriesAsync();

        Task<IReadOnlyList<CountryDto>> GetInactiveCountriesAsync();

        Task<bool> ActivateAsync(int countryId);

        Task<bool> DeactivateAsync(int countryId);


  
        // 3. SEARCH AND FILTER OPERATIONS
        Task<IReadOnlyList<CountryDto>> SearchAsync(
            string? searchTerm,
            bool? isActive,
            int pageNumber,
            int pageSize);

        Task<IReadOnlyList<CountryDto>> GetByPhoneCodeAsync(string phoneCode);

        Task<IReadOnlyList<CountryDto>> GetRecentlyAddedAsync(int count);


  
        // 4. RELATIONSHIP OPERATIONS
        Task<IReadOnlyList<CountryDto>> GetCountriesWithBranchesAsync();

        Task<IReadOnlyList<CountryDto>> GetCountriesWithCurrenciesAsync();

        Task<IReadOnlyList<CountryDto>> GetCountriesWithCustomersAsync();

        Task<CountryDto?> GetCountryDetailsAsync(int countryId);


  
        // 5. VALIDATION OPERATIONS
        Task<bool> IsCountryNameExistsAsync(string name);

        Task<bool> IsIsoAlpha2CodeExistsAsync(string isoAlpha2Code);

        Task<bool> IsIsoAlpha3CodeExistsAsync(string isoAlpha3Code);

        Task<bool> IsNumericCodeExistsAsync(string numericCode);

        Task<bool> IsPhoneCodeExistsAsync(string phoneCode);


  
        // 6. USAGE AND REPORTING
        Task<bool> IsCountryInUseAsync(int countryId);

        Task<CountryUsageSummaryDto?> GetUsageSummaryAsync(int countryId);
    }
}
