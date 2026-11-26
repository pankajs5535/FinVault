
using AutoMapper;
using FinVault.Application.DTOs.CountryDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class CountryService : ICountryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CountryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<CountryDto>> GetAllAsync()
    {
        var countries = await _unitOfWork.Country.GetAllAsync();
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<CountryDto?> GetByIdAsync(int countryId)
    {
        var country = await _unitOfWork.Country.GetByIdAsync(countryId);
        return _mapper.Map<CountryDto?>(country);
    }

    public async Task<CountryDto?> GetByNameAsync(string name)
    {
        var country = await _unitOfWork.Country.GetByNameAsync(name);
        return _mapper.Map<CountryDto?>(country);
    }

    public async Task<CountryDto?> GetByIsoAlpha2CodeAsync(string isoAlpha2Code)
    {
        var country = await _unitOfWork.Country.GetByIsoAlpha2CodeAsync(isoAlpha2Code);
        return _mapper.Map<CountryDto?>(country);
    }

    public async Task<CountryDto?> GetByIsoAlpha3CodeAsync(string isoAlpha3Code)
    {
        var country = await _unitOfWork.Country.GetByIsoAlpha3CodeAsync(isoAlpha3Code);
        return _mapper.Map<CountryDto?>(country);
    }

    public async Task<CountryDto?> GetByNumericCodeAsync(string numericCode)
    {
        var country = await _unitOfWork.Country.GetByNumericCodeAsync(numericCode);
        return _mapper.Map<CountryDto?>(country);
    }

    public async Task<CountryDto> CreateAsync(CreateCountryDto dto)
    {
        if (await IsIsoAlpha2CodeExistsAsync(dto.IsoAlpha2Code) ||
            await IsIsoAlpha3CodeExistsAsync(dto.IsoAlpha3Code) ||
            await IsNumericCodeExistsAsync(dto.NumericCode) ||
            await IsCountryNameExistsAsync(dto.Name))
        {
            throw new InvalidOperationException("A country with the supplied identification details already exists.");
        }

        var country = _mapper.Map<Country>(dto);
        country.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Country.AddAsync(country);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CountryDto>(country);
    }

    public async Task<CountryDto?> UpdateAsync(UpdateCountryDto dto)
    {
        var country = await _unitOfWork.Country.GetByIdAsync(dto.CountryId);

        if (country == null)
            return null;

        var alpha2 = await _unitOfWork.Country.GetByIsoAlpha2CodeAsync(dto.IsoAlpha2Code);
        var alpha3 = await _unitOfWork.Country.GetByIsoAlpha3CodeAsync(dto.IsoAlpha3Code);
        var numeric = await _unitOfWork.Country.GetByNumericCodeAsync(dto.NumericCode);
        var name = await _unitOfWork.Country.GetByNameAsync(dto.Name);

        if ((alpha2 != null && alpha2.CountryId != dto.CountryId) ||
            (alpha3 != null && alpha3.CountryId != dto.CountryId) ||
            (numeric != null && numeric.CountryId != dto.CountryId) ||
            (name != null && name.CountryId != dto.CountryId))
        {
            throw new InvalidOperationException("The supplied country identification details are already in use.");
        }

        _mapper.Map(dto, country);
        country.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Country.Update(country);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CountryDto>(country);
    }

    public async Task<bool> DeleteAsync(DeleteCountryDto dto)
    {
        var country = await _unitOfWork.Country.GetByIdAsync(dto.CountryId);

        if (country == null || !await CanCountryBeDeletedAsync(dto.CountryId))
            return false;

        _unitOfWork.Country.Delete(country);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. PHONE CODE AND STATUS

    public async Task<IEnumerable<CountryDto>> GetCountriesByPhoneCodeAsync(string phoneCode)
    {
        var countries = await _unitOfWork.Country.GetCountriesByPhoneCodeAsync(phoneCode);
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<IEnumerable<CountryDto>> GetActiveCountriesAsync()
    {
        var countries = await _unitOfWork.Country.GetActiveCountriesAsync();
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<bool> ActivateAsync(int countryId)
    {
        var country = await _unitOfWork.Country.GetByIdAsync(countryId);

        if (country == null || country.IsActive)
            return false;

        country.IsActive = true;
        country.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Country.Update(country);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int countryId)
    {
        var country = await _unitOfWork.Country.GetByIdAsync(countryId);

        if (country == null || !country.IsActive)
            return false;

        if (await IsCountryInUseAsync(countryId))
            throw new InvalidOperationException("Country cannot be deactivated because it is currently in use.");

        country.IsActive = false;
        country.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Country.Update(country);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. COUNTRY RELATIONSHIPS

    public async Task<IEnumerable<CountryDto>> GetCountriesWithBranchesAsync()
    {
        var countries = await _unitOfWork.Country.GetCountriesWithBranchesAsync();
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<IEnumerable<CountryDto>> GetCountriesWithCurrenciesAsync()
    {
        var countries = await _unitOfWork.Country.GetCountriesWithCurrenciesAsync();
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<IEnumerable<CountryDto>> GetCountriesWithCustomersAsync()
    {
        var countries = await _unitOfWork.Country.GetCountriesWithCustomersAsync();
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<CountryDto?> GetCountryDetailsAsync(int countryId)
    {
        var country = await _unitOfWork.Country.GetCountryDetailsAsync(countryId);
        return _mapper.Map<CountryDto?>(country);
    }

    // 4. SEARCH AND ADMINISTRATION

    public async Task<IEnumerable<CountryDto>> SearchAsync(
        string? searchTerm,
        bool? isActive,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var countries = await _unitOfWork.Country.SearchCountriesAsync(
            searchTerm,
            isActive,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<IEnumerable<CountryDto>> GetRecentlyAddedCountriesAsync(int count)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        var countries = await _unitOfWork.Country.GetRecentlyAddedCountriesAsync(count);
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    // 5. USAGE AND VALIDATION

    public async Task<CountryUsageSummaryDto?> GetCountryUsageSummaryAsync(int countryId)
    {
        return await _unitOfWork.Country.GetCountryUsageSummaryAsync(countryId);
    }

    public async Task<bool> IsCountryInUseAsync(int countryId)
    {
        return await _unitOfWork.Country.IsCountryInUseAsync(countryId);
    }

    public async Task<bool> IsIsoAlpha2CodeExistsAsync(string isoAlpha2Code)
    {
        return await _unitOfWork.Country.GetByIsoAlpha2CodeAsync(isoAlpha2Code) != null;
    }

    public async Task<bool> IsIsoAlpha3CodeExistsAsync(string isoAlpha3Code)
    {
        return await _unitOfWork.Country.GetByIsoAlpha3CodeAsync(isoAlpha3Code) != null;
    }

    public async Task<bool> IsNumericCodeExistsAsync(string numericCode)
    {
        return await _unitOfWork.Country.GetByNumericCodeAsync(numericCode) != null;
    }

    public async Task<bool> IsCountryNameExistsAsync(string name)
    {
        return await _unitOfWork.Country.GetByNameAsync(name) != null;
    }

    public async Task<bool> CanCountryBeDeletedAsync(int countryId)
    {
        var country = await _unitOfWork.Country.GetByIdAsync(countryId);

        if (country == null)
            return false;

        return !await IsCountryInUseAsync(countryId);
    }
}
