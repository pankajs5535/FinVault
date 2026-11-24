using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.FraudAlertDtos;
using FinVault.Application.DTOs.RiskAssessmentDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the FraudRisk banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IFraudRiskService
{
        Task<IReadOnlyList<FraudAlertDto>> GetAlertsAsync(int? customerId);
        Task<FraudAlertDto> CreateAlertAsync(CreateFraudAlertDto dto);
        Task<RiskAssessmentDto> AssessRiskAsync(int customerId, int? accountId, decimal amount);
        Task<bool> ResolveAlertAsync(int fraudAlertId, string resolution);
}
