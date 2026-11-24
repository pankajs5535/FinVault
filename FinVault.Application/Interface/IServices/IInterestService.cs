using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.InterestRateDtos;
using FinVault.Application.DTOs.InterestCalculationDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Interest banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IInterestService
{
        Task<IReadOnlyList<InterestRateDto>> GetRatesAsync(int? accountTypeId);
        Task<InterestRateDto> CreateRateAsync(CreateInterestRateDto dto);
        Task<InterestRateDto?> UpdateRateAsync(int interestRateId, UpdateInterestRateDto dto);
        Task<InterestCalculationDto> CalculateAsync(int accountId, DateTime asOfDate);
        Task<bool> PostAccruedInterestAsync(int interestCalculationId);
}
