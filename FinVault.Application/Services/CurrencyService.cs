using AutoMapper;
using FinVault.Application.DTOs.CurrencyDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CurrencyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<CurrencyDto>> GetAllAsync()
    {
        var currencies = await _unitOfWork.Currency.GetAllAsync();
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    public async Task<CurrencyDto?> GetByIdAsync(int currencyId)
    {
        var currency = await _unitOfWork.Currency.GetByIdAsync(currencyId);
        return _mapper.Map<CurrencyDto?>(currency);
    }

    public async Task<CurrencyDto?> GetByCurrencyCodeAsync(string currencyCode)
    {
        var currency = await _unitOfWork.Currency.GetByCurrencyCodeAsync(currencyCode);
        return _mapper.Map<CurrencyDto?>(currency);
    }

    public async Task<CurrencyDto?> GetByNumericCodeAsync(string numericCode)
    {
        var currency = await _unitOfWork.Currency.GetByNumericCodeAsync(numericCode);
        return _mapper.Map<CurrencyDto?>(currency);
    }

    public async Task<CurrencyDto?> GetByNameAsync(string name)
    {
        var currency = await _unitOfWork.Currency.GetByNameAsync(name);
        return _mapper.Map<CurrencyDto?>(currency);
    }

    public async Task<CurrencyDto> CreateAsync(CreateCurrencyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var currencyCode = NormalizeCode(dto.CurrencyCode);
        var numericCode = NormalizeCode(dto.NumericCode);
        var name = NormalizeName(dto.Name);

        if (await IsCurrencyCodeExistsAsync(currencyCode))
            throw new InvalidOperationException($"Currency code '{currencyCode}' already exists.");

        if (await IsNumericCodeExistsAsync(numericCode))
            throw new InvalidOperationException($"Currency numeric code '{numericCode}' already exists.");

        if (await IsCurrencyNameExistsAsync(name))
            throw new InvalidOperationException($"Currency name '{name}' already exists.");

        await ValidateCountryAsync(dto.CountryId);

        if (dto.IsBaseCurrency && !dto.IsActive)
            throw new InvalidOperationException("The base currency must be active.");

        var currency = _mapper.Map<Currency>(dto);
        currency.CurrencyCode = currencyCode;
        currency.NumericCode = numericCode;
        currency.Name = name;
        currency.Symbol = dto.Symbol.Trim();
        currency.CreatedAt = DateTime.UtcNow;

        if (currency.IsBaseCurrency)
            await ClearExistingBaseCurrencyAsync();

        await _unitOfWork.Currency.AddAsync(currency);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<CurrencyDto?> UpdateAsync(UpdateCurrencyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var currency = await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId);
        if (currency == null)
            return null;

        var currencyCode = NormalizeCode(dto.CurrencyCode);
        var numericCode = NormalizeCode(dto.NumericCode);
        var name = NormalizeName(dto.Name);

        var existingByCode = await _unitOfWork.Currency.GetByCurrencyCodeAsync(currencyCode);
        if (existingByCode != null && existingByCode.CurrencyId != dto.CurrencyId)
            throw new InvalidOperationException($"Currency code '{currencyCode}' already exists.");

        var existingByNumericCode = await _unitOfWork.Currency.GetByNumericCodeAsync(numericCode);
        if (existingByNumericCode != null && existingByNumericCode.CurrencyId != dto.CurrencyId)
            throw new InvalidOperationException($"Currency numeric code '{numericCode}' already exists.");

        var existingByName = await _unitOfWork.Currency.GetByNameAsync(name);
        if (existingByName != null && existingByName.CurrencyId != dto.CurrencyId)
            throw new InvalidOperationException($"Currency name '{name}' already exists.");

        await ValidateCountryAsync(dto.CountryId);

        if (dto.IsBaseCurrency && !dto.IsActive)
            throw new InvalidOperationException("The base currency must be active.");

        if (!dto.IsActive && await IsCurrencyInUseAsync(dto.CurrencyId))
            throw new InvalidOperationException("A currency used by existing accounts or transactions cannot be deactivated.");

        _mapper.Map(dto, currency);
        currency.CurrencyCode = currencyCode;
        currency.NumericCode = numericCode;
        currency.Name = name;
        currency.Symbol = dto.Symbol.Trim();
        currency.UpdatedAt = DateTime.UtcNow;

        if (currency.IsBaseCurrency)
            await ClearExistingBaseCurrencyAsync(currency.CurrencyId);

        _unitOfWork.Currency.Update(currency);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<bool> DeleteAsync(DeleteCurrencyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var currency = await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId);
        if (currency == null || !await CanCurrencyBeDeletedAsync(dto.CurrencyId))
            return false;

        _unitOfWork.Currency.Delete(currency);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CURRENCY SYMBOL AND STATUS

    public async Task<IEnumerable<CurrencyDto>> GetCurrenciesBySymbolAsync(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Currency symbol is required.", nameof(symbol));

        var currencies = await _unitOfWork.Currency.GetCurrenciesBySymbolAsync(symbol.Trim());
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    public async Task<CurrencyDto?> GetBaseCurrencyAsync()
    {
        var currency = await _unitOfWork.Currency.GetBaseCurrencyAsync();
        return _mapper.Map<CurrencyDto?>(currency);
    }

    public async Task<IEnumerable<CurrencyDto>> GetActiveCurrenciesAsync()
    {
        var currencies = await _unitOfWork.Currency.GetActiveCurrenciesAsync();
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    // 3. COUNTRY RELATIONSHIP

    public async Task<IEnumerable<CurrencyDto>> GetCurrenciesByCountryIdAsync(int countryId)
    {
        if (countryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(countryId));

        var currencies = await _unitOfWork.Currency.GetCurrenciesByCountryIdAsync(countryId);
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    public async Task<CurrencyDto?> GetCurrencyDetailsAsync(int currencyId)
    {
        if (currencyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(currencyId));

        var currency = await _unitOfWork.Currency.GetCurrencyDetailsAsync(currencyId);
        return _mapper.Map<CurrencyDto?>(currency);
    }

    // 4. ACCOUNT AND TRANSACTION RELATIONSHIPS

    public async Task<IEnumerable<CurrencyDto>> GetCurrenciesUsedByAccountsAsync()
    {
        var currencies = await _unitOfWork.Currency.GetCurrenciesUsedByAccountsAsync();
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    public async Task<IEnumerable<CurrencyDto>> GetCurrenciesUsedByTransactionsAsync()
    {
        var currencies = await _unitOfWork.Currency.GetCurrenciesUsedByTransactionsAsync();
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    // 5. SEARCH AND ADMINISTRATION

    public async Task<IEnumerable<CurrencyDto>> SearchAsync(
        string? searchTerm,
        bool? isActive,
        int? countryId,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

        if (countryId.HasValue && countryId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(countryId), "Country ID must be greater than zero.");

        var currencies = await _unitOfWork.Currency.SearchCurrenciesAsync(
            searchTerm?.Trim(),
            isActive,
            countryId,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    public async Task<IEnumerable<CurrencyDto>> GetRecentlyAddedCurrenciesAsync(int count)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero.");

        var currencies = await _unitOfWork.Currency.GetRecentlyAddedCurrenciesAsync(count);
        return _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
    }

    // 6. VALIDATION AND USAGE

    public async Task<bool> IsCurrencyCodeExistsAsync(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return false;

        return await _unitOfWork.Currency.GetByCurrencyCodeAsync(NormalizeCode(currencyCode)) != null;
    }

    public async Task<bool> IsNumericCodeExistsAsync(string numericCode)
    {
        if (string.IsNullOrWhiteSpace(numericCode))
            return false;

        return await _unitOfWork.Currency.GetByNumericCodeAsync(NormalizeCode(numericCode)) != null;
    }

    public async Task<bool> IsCurrencyNameExistsAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return await _unitOfWork.Currency.GetByNameAsync(NormalizeName(name)) != null;
    }

    public async Task<bool> IsCurrencyInUseAsync(int currencyId)
    {
        if (currencyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(currencyId));

        return await _unitOfWork.Currency.IsCurrencyInUseAsync(currencyId);
    }

    public async Task<bool> CanCurrencyBeDeletedAsync(int currencyId)
    {
        if (currencyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(currencyId));

        var currency = await _unitOfWork.Currency.GetByIdAsync(currencyId);
        if (currency == null)
            return false;

        return !await IsCurrencyInUseAsync(currencyId);
    }

    // PRIVATE BUSINESS-RULE HELPERS

    private async Task ValidateCountryAsync(int? countryId)
    {
        if (!countryId.HasValue)
            return;

        if (countryId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(countryId), "Country ID must be greater than zero.");

        var country = await _unitOfWork.Country.GetByIdAsync(countryId.Value);
        if (country == null)
            throw new InvalidOperationException("The selected country does not exist.");

        if (!country.IsActive)
            throw new InvalidOperationException("An inactive country cannot be assigned to a currency.");
    }

    private async Task ClearExistingBaseCurrencyAsync(int? exceptCurrencyId = null)
    {
        var currentBaseCurrency = await _unitOfWork.Currency.GetBaseCurrencyAsync();

        if (currentBaseCurrency == null ||
            (exceptCurrencyId.HasValue && currentBaseCurrency.CurrencyId == exceptCurrencyId.Value))
            return;

        var trackedBaseCurrency = await _unitOfWork.Currency.GetByIdAsync(currentBaseCurrency.CurrencyId);
        if (trackedBaseCurrency == null)
            return;

        trackedBaseCurrency.IsBaseCurrency = false;
        trackedBaseCurrency.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Currency.Update(trackedBaseCurrency);
    }

    private static string NormalizeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Currency code is required.", nameof(value));

        return value.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Currency name is required.", nameof(value));

        return value.Trim();
    }
}