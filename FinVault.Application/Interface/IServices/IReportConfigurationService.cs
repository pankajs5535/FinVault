using FinVault.Application.DTOs.ReportConfigurationDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IReportConfigurationService
    {
        // 1. REPORT IDENTIFICATION
        Task<IEnumerable<ReportConfigurationDto>> GetAllAsync();
        Task<ReportConfigurationDto?> GetByIdAsync(int reportConfigurationId);
        Task<ReportConfigurationDto?> GetByReportCodeAsync(string reportCode);
        Task<bool> ExistsByReportCodeAsync(string reportCode);

        // 2. REPORT CRUD AND DETAILS
        Task<ReportConfigurationDto?> GetReportConfigurationDetailsAsync(int reportConfigurationId);
        Task<IEnumerable<ReportConfigurationDto>> GetAllWithOwnersAsync();
        Task<ReportConfigurationDto> CreateAsync(CreateReportConfigurationDto dto);
        Task<ReportConfigurationDto?> UpdateAsync(UpdateReportConfigurationDto dto);
        Task<bool> DeleteAsync(DeleteReportConfigurationDto dto);

        // 3. REPORT TYPE AND FORMAT
        Task<IEnumerable<ReportConfigurationDto>> GetByReportTypeAsync(string reportType);
        Task<IEnumerable<ReportConfigurationDto>> GetByOutputFormatAsync(string outputFormat);
        Task<IEnumerable<ReportConfigurationDto>> GetRegulatoryReportsAsync();

        // 4. REPORT STATUS
        Task<IEnumerable<ReportConfigurationDto>> GetActiveReportsAsync();
        Task<IEnumerable<ReportConfigurationDto>> GetInactiveReportsAsync();

        // 5. SCHEDULED REPORTS
        Task<IEnumerable<ReportConfigurationDto>> GetScheduledReportsAsync();
        Task<IEnumerable<ReportConfigurationDto>> GetReportsDueForExecutionAsync(DateTime currentDateTime);
        Task<IEnumerable<ReportConfigurationDto>> GetScheduledReportsWithinRangeAsync(
            DateTime startDateTime,
            DateTime endDateTime);
        Task<IEnumerable<ReportConfigurationDto>> GetReportsPreviouslyExecutedAsync();
        Task<IEnumerable<ReportConfigurationDto>> GetReportsNeverExecutedAsync();

        // 6. USER OWNERSHIP
        Task<IEnumerable<ReportConfigurationDto>> GetReportsByOwnerUserIdAsync(int ownerUserId);
        Task<IEnumerable<ReportConfigurationDto>> GetReportsWithoutOwnerAsync();

        // 7. REPORT EXECUTION HISTORY
        Task<IEnumerable<ReportConfigurationDto>> GetReportsLastRunWithinRangeAsync(
            DateTime startDateTime,
            DateTime endDateTime);
        Task<IEnumerable<ReportConfigurationDto>> GetReportsWithNextRunBeforeAsync(DateTime dateTime);

        // 8. RETENTION AND SEARCH
        Task<IEnumerable<ReportConfigurationDto>> GetReportsByMinimumRetentionDaysAsync(int minimumRetentionDays);
        Task<IEnumerable<ReportConfigurationDto>> SearchReportConfigurationsAsync(string searchTerm);

        // 9. REPORTING
        Task<int> CountByReportTypeAsync(string reportType);
    }
}


//Banking rules:
//-Report codes must be unique.
//- Allowed report types: Statement, Regulatory, Operational, Financial, Analytics.
//- Allowed output formats: Pdf, Excel, Csv, Json.
//- Execute only approved query definitions or stored procedures; never execute free - form user SQL.
//- Only authorized users can create, modify, schedule, or execute reports.
//- Validate recipients and report parameters before execution.
//- Enforce retention requirements, especially for regulatory reports