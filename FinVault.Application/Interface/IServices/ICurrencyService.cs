using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.CurrencyDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Currency banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface ICurrencyService
{
        Task<CurrencyDto?> GetByIdAsync(int currencyId);
        Task<IReadOnlyList<CurrencyDto>> GetAllAsync();
        Task<CurrencyDto> CreateAsync(CreateCurrencyDto dto);
        Task<CurrencyDto?> UpdateAsync(int currencyId, UpdateCurrencyDto dto);
        Task<bool> DeleteAsync(int currencyId);
}
