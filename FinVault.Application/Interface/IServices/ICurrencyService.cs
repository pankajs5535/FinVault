
using FinVault.Application.DTOs.CurrencyDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICurrencyService
    {
      
        // 1. CRUD OPERATIONS
        Task<IReadOnlyList<CurrencyDto>> GetAllAsync();
        Task<CurrencyDto?> GetByIdAsync(int currencyId);
        Task<CurrencyDto?> GetByCurrencyCodeAsync(string currencyCode);
        Task<CurrencyDto?> GetByNumericCodeAsync(string numericCode);
        Task<CurrencyDto?> GetByNameAsync(string name);
        Task<IReadOnlyList<CurrencyDto>> GetBySymbolAsync(string symbol);

        Task<CurrencyDto?> GetBaseCurrencyAsync();

        Task<CurrencyDto> CreateAsync(CreateCurrencyDto dto);

        Task<CurrencyDto?> UpdateAsync(int currencyId, UpdateCurrencyDto dto);

        Task<bool> DeleteAsync(int currencyId);

        Task DeleteMultipleAsync(IEnumerable<int> currencyIds);


      
        // 2. STATUS AND CONFIGURATION OPERATIONS
      

        Task<IReadOnlyList<CurrencyDto>> GetActiveCurrenciesAsync();

        Task<IReadOnlyList<CurrencyDto>> GetInactiveCurrenciesAsync();

        Task<bool> ActivateAsync(int currencyId);

        Task<bool> DeactivateAsync(int currencyId);

        Task<bool> SetBaseCurrencyAsync(int currencyId);


      
        // 3. COUNTRY-BASED OPERATIONS
      

        Task<IReadOnlyList<CurrencyDto>> GetByCountryIdAsync(int countryId);

        Task<IReadOnlyList<CurrencyDto>> GetCurrenciesWithoutCountryAsync();


      
        // 4. SEARCH AND FILTER OPERATIONS
      

        Task<IReadOnlyList<CurrencyDto>> SearchAsync(
            string? searchTerm,
            bool? isActive,
            int? countryId,
            int pageNumber,
            int pageSize);

        Task<IReadOnlyList<CurrencyDto>> GetRecentlyAddedAsync(int count);


      
        // 5. RELATIONSHIP AND USAGE OPERATIONS
      

        Task<IReadOnlyList<CurrencyDto>> GetCurrenciesUsedByAccountsAsync();

        Task<IReadOnlyList<CurrencyDto>> GetCurrenciesUsedByTransactionsAsync();

        Task<CurrencyDto?> GetCurrencyDetailsAsync(int currencyId);

        Task<bool> IsCurrencyInUseAsync(int currencyId);


      
        // 6. VALIDATION OPERATIONS
      

        Task<bool> IsCurrencyCodeExistsAsync(string currencyCode);

        Task<bool> IsNumericCodeExistsAsync(string numericCode);

        Task<bool> IsCurrencyNameExistsAsync(string name);

        Task<bool> IsCurrencySymbolExistsAsync(string symbol);
    }
}
