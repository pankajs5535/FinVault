
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories;

public interface ICurrencyRepository : IGenericRepository<Currency>
{
    // =====================================================
    // 1. CURRENCY IDENTIFICATION
    // =====================================================

    // Finds a currency using its 3-letter ISO currency code.
    // Example: INR, USD, EUR.
    // Useful for identifying currencies in banking transactions.
    Task<Currency?> GetByCurrencyCodeAsync(string currencyCode);

    // Finds a currency using its 3-digit ISO numeric code.
    // Example: 356 (INR), 840 (USD).
    // Useful for standardized currency identification and integrations.
    Task<Currency?> GetByNumericCodeAsync(string numericCode);

    // Finds a currency using its full name.
    // Example: Indian Rupee, US Dollar.
    // Useful for currency validation and administrative searches.
    Task<Currency?> GetByNameAsync(string name);

    // Finds currencies using their currency symbol.
    // Example: ₹, $, €.
    // Returns multiple results because symbols are not necessarily unique.
    Task<IEnumerable<Currency>> GetCurrenciesBySymbolAsync(string symbol);


    // =====================================================
    // 2. CURRENCY STATUS AND CONFIGURATION
    // =====================================================

    // Retrieves the configured base currency.
    // INR is the base currency in our domestic banking model.
    // Useful for default currency selection and financial reporting.
    Task<Currency?> GetBaseCurrencyAsync();

    // Retrieves all active currencies.
    // Useful for account creation and transaction currency selection.
    Task<IEnumerable<Currency>> GetActiveCurrenciesAsync();

    // Retrieves currencies associated with a particular country.
    // Useful for country-based currency selection.
    Task<IEnumerable<Currency>> GetCurrenciesByCountryIdAsync(int countryId);

    // Retrieves recently created currency records.
    // Useful for administrative monitoring.
    Task<IEnumerable<Currency>> GetRecentlyAddedCurrenciesAsync(int count);


    // =====================================================
    // 3. CURRENCY SEARCH AND FILTERING
    // =====================================================

    // Searches currencies by code, numeric code, name or symbol.
    // Supports active-status and country filtering with pagination.
    // Useful for currency management screens.
    Task<IEnumerable<Currency>> SearchCurrenciesAsync(string? searchTerm, bool? isActive, int? countryId, int pageNumber, int pageSize);


    // =====================================================
    // 4. CURRENCY RELATIONSHIPS
    // =====================================================

    // Retrieves currencies referenced by bank accounts.
    // Useful for identifying currencies currently used by customers.
    Task<IEnumerable<Currency>> GetCurrenciesUsedByAccountsAsync();

    // Retrieves currencies referenced by financial transactions.
    // Useful for transaction reporting and currency usage analysis.
    Task<IEnumerable<Currency>> GetCurrenciesUsedByTransactionsAsync();

    // Retrieves currency details along with its associated country.
    // Useful for displaying complete currency reference information.
    Task<Currency?> GetCurrencyDetailsAsync(int currencyId);


    // =====================================================
    // 5. CURRENCY VALIDATION
    // =====================================================

    // Checks whether a currency is referenced by any account or transaction.
    // Useful before deactivation or deletion.
    // Prevents disrupting existing financial records.
    Task<bool> IsCurrencyInUseAsync(int currencyId);
}
