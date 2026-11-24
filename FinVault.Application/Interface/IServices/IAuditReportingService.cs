using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.AuditLogDtos;
using FinVault.Application.DTOs.ReportConfigurationDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the AuditReporting banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IAuditReportingService
{
        Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(DateTime from, DateTime to, int? userId);
        Task<IReadOnlyList<ReportConfigurationDto>> GetReportConfigurationsAsync();
        Task<ReportConfigurationDto> CreateReportConfigurationAsync(CreateReportConfigurationDto dto);
        Task<ReportConfigurationDto?> UpdateReportConfigurationAsync(int reportConfigurationId, UpdateReportConfigurationDto dto);
}
