using AutoMapper;
using FinVault.Application.DTOs.CountryDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Applies country validation and business rules before persisting country data.</summary>
public sealed class CountryService : ICountryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CountryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Returns one country; null means the requested country does not exist.
    public async Task<CountryDto?> GetByIdAsync(int countryId)
    {
        var country = await _unitOfWork.Country.GetCountryDetailsAsync(countryId);
        return country is null ? null : _mapper.Map<CountryDto>(country);
    }

    // 2. Returns all countries for administrative lists and selection controls.
    public async Task<IReadOnlyList<CountryDto>> GetAllAsync()
    {
        var countries = await _unitOfWork.Country.GetAllAsync();
        return _mapper.Map<IReadOnlyList<CountryDto>>(countries);
    }

    // 3. Normalizes ISO codes and name, checks uniqueness, then creates the record.
    public async Task<CountryDto> CreateAsync(CreateCountryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var alpha2 = dto.IsoAlpha2Code.Trim().ToUpperInvariant();
        var alpha3 = dto.IsoAlpha3Code.Trim().ToUpperInvariant();
        var numeric = dto.NumericCode.Trim();
        var name = dto.Name.Trim();

        if (await _unitOfWork.Country.GetByIsoAlpha2CodeAsync(alpha2) is not null ||
            await _unitOfWork.Country.GetByIsoAlpha3CodeAsync(alpha3) is not null ||
            await _unitOfWork.Country.GetByNumericCodeAsync(numeric) is not null ||
            await _unitOfWork.Country.GetByNameAsync(name) is not null)
            throw new InvalidOperationException("A country with the same name or ISO code already exists.");

        var country = _mapper.Map<Country>(dto);
        country.IsoAlpha2Code = alpha2;
        country.IsoAlpha3Code = alpha3;
        country.NumericCode = numeric;
        country.Name = name;
        country.PhoneCode = dto.PhoneCode?.Trim();
        country.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Country.AddAsync(country);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CountryDto>(country);
    }

    // 4. Updates an existing country after checking that changed identifiers remain unique.
    public async Task<CountryDto?> UpdateAsync(int countryId, UpdateCountryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.CountryId != countryId)
            throw new ArgumentException("The route country ID and DTO country ID do not match.", nameof(dto));

        var country = await _unitOfWork.Country.GetByIdAsync(countryId);
        if (country is null) return null;

        var alpha2 = dto.IsoAlpha2Code.Trim().ToUpperInvariant();
        var alpha3 = dto.IsoAlpha3Code.Trim().ToUpperInvariant();
        var numeric = dto.NumericCode.Trim();
        var name = dto.Name.Trim();

        var duplicateAlpha2 = await _unitOfWork.Country.GetByIsoAlpha2CodeAsync(alpha2);
        var duplicateAlpha3 = await _unitOfWork.Country.GetByIsoAlpha3CodeAsync(alpha3);
        var duplicateNumeric = await _unitOfWork.Country.GetByNumericCodeAsync(numeric);
        var duplicateName = await _unitOfWork.Country.GetByNameAsync(name);
        if ((duplicateAlpha2 is not null && duplicateAlpha2.CountryId != countryId) ||
            (duplicateAlpha3 is not null && duplicateAlpha3.CountryId != countryId) ||
            (duplicateNumeric is not null && duplicateNumeric.CountryId != countryId) ||
            (duplicateName is not null && duplicateName.CountryId != countryId))
            throw new InvalidOperationException("Another country already uses the supplied name or ISO code.");

        _mapper.Map(dto, country);
        country.IsoAlpha2Code = alpha2;
        country.IsoAlpha3Code = alpha3;
        country.NumericCode = numeric;
        country.Name = name;
        country.PhoneCode = dto.PhoneCode?.Trim();
        country.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Country.Update(country);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CountryDto>(country);
    }

    // 5. Prevents deletion while customers, addresses, branches or currencies reference it.
    public async Task<bool> DeleteAsync(int countryId)
    {
        var country = await _unitOfWork.Country.GetByIdAsync(countryId);
        if (country is null || await _unitOfWork.Country.IsCountryInUseAsync(countryId)) return false;
        _unitOfWork.Country.Delete(country);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 6. Delegates usage counts to the repository's purpose-built reporting query.
    public Task<CountryUsageSummaryDto?> GetUsageSummaryAsync(int countryId)
    {
        return _unitOfWork.Country.GetCountryUsageSummaryAsync(countryId);
    }
}
