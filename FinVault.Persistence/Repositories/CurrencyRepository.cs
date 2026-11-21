
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
{
    public CurrencyRepository(FinVaultDbContext context) : base(context)
    {
    }

    // =====================================================
    // 1. CURRENCY IDENTIFICATION
    // =====================================================

    // Finds a currency using its 3-letter ISO currency code.
    // Example: INR, USD, EUR.
    // Useful for identifying currencies in banking transactions.
    public async Task<Currency?> GetByCurrencyCodeAsync(string currencyCode)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.CurrencyCode == currencyCode);
    }

    // Finds a currency using its 3-digit ISO numeric code.
    // Example: 356 (INR), 840 (USD).
    // Useful for standardized currency identification and integrations.
    public async Task<Currency?> GetByNumericCodeAsync(string numericCode)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.NumericCode == numericCode);
    }

    // Finds a currency using its full name.
    // Example: Indian Rupee, US Dollar.
    // Useful for currency validation and administrative searches.
    public async Task<Currency?> GetByNameAsync(string name)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name);
    }

    // Finds currencies using their currency symbol.
    // Example: ₹, $, €.
    // Symbols are not necessarily unique, so multiple results are possible.
    public async Task<IEnumerable<Currency>> GetCurrenciesBySymbolAsync(string symbol)
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.Symbol == symbol)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // =====================================================
    // 2. CURRENCY STATUS AND CONFIGURATION
    // =====================================================

    // Retrieves the configured base currency.
    // INR is the base currency in our domestic banking model.
    // Useful for default currency selection and financial reporting.
    public async Task<Currency?> GetBaseCurrencyAsync()
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.IsBaseCurrency);
    }

    // Retrieves all active currencies.
    // Useful for account creation and transaction currency selection.
    public async Task<IEnumerable<Currency>> GetActiveCurrenciesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Retrieves currencies associated with a particular country.
    // Useful for country-based currency selection.
    public async Task<IEnumerable<Currency>> GetCurrenciesByCountryIdAsync(int countryId)
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.CountryId == countryId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Retrieves recently created currency records.
    // Useful for administrative monitoring.
    public async Task<IEnumerable<Currency>> GetRecentlyAddedCurrenciesAsync(int count)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        return await _dbSet.AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    // =====================================================
    // 3. CURRENCY SEARCH AND FILTERING
    // =====================================================

    // Searches currencies by code, numeric code, name or symbol.
    // Supports active-status and country filtering with pagination.
    // Useful for currency management screens.
    public async Task<IEnumerable<Currency>> SearchCurrenciesAsync(string? searchTerm, bool? isActive, int? countryId, int pageNumber, int pageSize)
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
                c.CurrencyCode.Contains(searchTerm) ||
                c.NumericCode.Contains(searchTerm) ||
                c.Name.Contains(searchTerm) ||
                c.Symbol.Contains(searchTerm));
        }

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        if (countryId.HasValue)
            query = query.Where(c => c.CountryId == countryId.Value);

        return await query.OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // =====================================================
    // 4. CURRENCY RELATIONSHIPS
    // =====================================================

    // Retrieves currencies referenced by bank accounts.
    // Useful for identifying currencies currently used by customers.
    // Uses Any() to avoid loading all associated account records.
    public async Task<IEnumerable<Currency>> GetCurrenciesUsedByAccountsAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.Accounts.Any())
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Retrieves currencies referenced by financial transactions.
    // Useful for transaction reporting and currency usage analysis.
    // Uses Any() to avoid loading all associated transaction records.
    public async Task<IEnumerable<Currency>> GetCurrenciesUsedByTransactionsAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(c => c.Transactions.Any())
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    // Retrieves currency details along with its associated country.
    // Useful for displaying complete currency reference information.
    public async Task<Currency?> GetCurrencyDetailsAsync(int currencyId)
    {
        return await _dbSet.AsNoTracking()
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.CurrencyId == currencyId);
    }

    // =====================================================
    // 5. CURRENCY VALIDATION
    // =====================================================

    // Checks whether a currency is referenced by any account or transaction.
    // Useful before deactivation or deletion.
    // Prevents disrupting existing financial records.
    public async Task<bool> IsCurrencyInUseAsync(int currencyId)
    {
        return await _dbSet.AnyAsync(c =>
            c.CurrencyId == currencyId &&
            (c.Accounts.Any() || c.Transactions.Any()));
    }
}
