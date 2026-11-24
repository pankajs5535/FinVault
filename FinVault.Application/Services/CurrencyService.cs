using AutoMapper;
using FinVault.Application.DTOs.CurrencyDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Manages currency reference data while protecting currencies already used by accounts or transactions.</summary>
public sealed class CurrencyService : ICurrencyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CurrencyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Finds a currency by its primary key.
    public async Task<CurrencyDto?> GetByIdAsync(int currencyId)
    {
        var entity = await _unitOfWork.Currency.GetCurrencyDetailsAsync(currencyId);
        return entity is null ? null : _mapper.Map<CurrencyDto>(entity);
    }

    // 2. Returns every configured currency.
    public async Task<IReadOnlyList<CurrencyDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Currency.GetAllAsync();
        return _mapper.Map<IReadOnlyList<CurrencyDto>>(entities);
    }

    // 3. Checks ISO identifiers, validates the country reference, and inserts the currency.
    public async Task<CurrencyDto> CreateAsync(CreateCurrencyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var code = dto.CurrencyCode.Trim().ToUpperInvariant();
        var numeric = dto.NumericCode.Trim();
        var name = dto.Name.Trim();
        if (await _unitOfWork.Currency.GetByCurrencyCodeAsync(code) is not null ||
            await _unitOfWork.Currency.GetByNumericCodeAsync(numeric) is not null ||
            await _unitOfWork.Currency.GetByNameAsync(name) is not null)
            throw new InvalidOperationException("A currency with the same code, numeric code or name already exists.");
        if (dto.CountryId.HasValue && await _unitOfWork.Country.GetByIdAsync(dto.CountryId.Value) is null)
            throw new InvalidOperationException("The selected country does not exist.");
        if (dto.IsBaseCurrency && await _unitOfWork.Currency.GetBaseCurrencyAsync() is not null)
            throw new InvalidOperationException("A base currency is already configured.");

        var entity = _mapper.Map<Currency>(dto);
        entity.CurrencyCode = code;
        entity.NumericCode = numeric;
        entity.Name = name;
        entity.Symbol = dto.Symbol.Trim();
        entity.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Currency.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CurrencyDto>(entity);
    }

    // 4. Updates currency details while preventing duplicate identifiers.
    public async Task<CurrencyDto?> UpdateAsync(int currencyId, UpdateCurrencyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.CurrencyId != currencyId) throw new ArgumentException("Route ID and DTO ID do not match.", nameof(dto));
        var entity = await _unitOfWork.Currency.GetByIdAsync(currencyId);
        if (entity is null) return null;
        var code = dto.CurrencyCode.Trim().ToUpperInvariant();
        var numeric = dto.NumericCode.Trim();
        var name = dto.Name.Trim();
        var codeMatch = await _unitOfWork.Currency.GetByCurrencyCodeAsync(code);
        var numericMatch = await _unitOfWork.Currency.GetByNumericCodeAsync(numeric);
        var nameMatch = await _unitOfWork.Currency.GetByNameAsync(name);
        if ((codeMatch is not null && codeMatch.CurrencyId != currencyId) ||
            (numericMatch is not null && numericMatch.CurrencyId != currencyId) ||
            (nameMatch is not null && nameMatch.CurrencyId != currencyId))
            throw new InvalidOperationException("Another currency already uses the supplied identifier.");
        if (dto.CountryId.HasValue && await _unitOfWork.Country.GetByIdAsync(dto.CountryId.Value) is null)
            throw new InvalidOperationException("The selected country does not exist.");
        if (dto.IsBaseCurrency && !entity.IsBaseCurrency && await _unitOfWork.Currency.GetBaseCurrencyAsync() is not null)
            throw new InvalidOperationException("A base currency is already configured.");

        _mapper.Map(dto, entity);
        entity.CurrencyCode = code;
        entity.NumericCode = numeric;
        entity.Name = name;
        entity.Symbol = dto.Symbol.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Currency.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CurrencyDto>(entity);
    }

    // 5. Blocks deletion when a currency is referenced by an account or transaction.
    public async Task<bool> DeleteAsync(int currencyId)
    {
        var entity = await _unitOfWork.Currency.GetByIdAsync(currencyId);
        if (entity is null || await _unitOfWork.Currency.IsCurrencyInUseAsync(currencyId)) return false;
        _unitOfWork.Currency.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
