using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.CountryDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Country banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface ICountryService
{
        Task<CountryDto?> GetByIdAsync(int countryId);
        Task<IReadOnlyList<CountryDto>> GetAllAsync();
        Task<CountryDto> CreateAsync(CreateCountryDto dto);
        Task<CountryDto?> UpdateAsync(int countryId, UpdateCountryDto dto);
        Task<bool> DeleteAsync(int countryId);
        Task<CountryUsageSummaryDto?> GetUsageSummaryAsync(int countryId);
}

 