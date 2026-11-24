using FinVault.Application.DTOs.CurrencyDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICurrencyService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<CurrencyDto>> GetAllAsync();
        Task<CurrencyDto?> GetByIdAsync(int currencyId);
        Task<CurrencyDto?> GetByCurrencyCodeAsync(string currencyCode);
        Task<CurrencyDto?> GetByNumericCodeAsync(string numericCode);
        Task<CurrencyDto?> GetByNameAsync(string name);
        Task<CurrencyDto> CreateAsync(CreateCurrencyDto dto);
        Task<CurrencyDto?> UpdateAsync(UpdateCurrencyDto dto);
        Task<bool> DeleteAsync(DeleteCurrencyDto dto);

        // 2. CURRENCY SYMBOL AND STATUS
        Task<IEnumerable<CurrencyDto>> GetCurrenciesBySymbolAsync(string symbol);
        Task<CurrencyDto?> GetBaseCurrencyAsync();
        Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync();

        // 3. COUNTRY RELATIONSHIP
        Task<IEnumerable<CurrencyDto>> GetCurrenciesByCountryIdAsync(int countryId);
        Task<CurrencyDto?> GetCurrencyDetailsAsync(int currencyId);

        // 4. ACCOUNT AND TRANSACTION RELATIONSHIPS
        Task<IEnumerable<CurrencyDto>> GetCurrenciesUsedByAccountsAsync();
        Task<IEnumerable<CurrencyDto>> GetCurrenciesUsedByTransactionsAsync();

        // 5. SEARCH AND ADMINISTRATION
        Task<IEnumerable<CurrencyDto>> SearchAsync(
            string? searchTerm,
            bool? isActive,
            int? countryId,
            int pageNumber,
            int pageSize);

        Task<IEnumerable<CurrencyDto>> GetRecentlyAddedCurrenciesAsync(int count);

        // 6. VALIDATION AND USAGE
        Task<bool> IsCurrencyCodeExistsAsync(string currencyCode);
        Task<bool> IsNumericCodeExistsAsync(string numericCode);
        Task<bool> IsCurrencyNameExistsAsync(string name);
        Task<bool> IsCurrencyInUseAsync(int currencyId);
        Task<bool> CanCurrencyBeDeletedAsync(int currencyId);
    }
}