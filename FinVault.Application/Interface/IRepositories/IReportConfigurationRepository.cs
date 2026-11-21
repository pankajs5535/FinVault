
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IReportConfigurationRepository : IGenericRepository<ReportConfiguration>
    {
        // ============================================================
        // 1. REPORT IDENTIFICATION
        // ============================================================

        // Retrieves a report configuration using its unique report code.
        // Useful for identifying a specific report without relying on its database ID.
        Task<ReportConfiguration?> GetByReportCodeAsync(string reportCode);

        // Checks whether a report code already exists.
        // Useful for preventing duplicate report configurations.
        Task<bool> ExistsByReportCodeAsync(string reportCode);

        // ============================================================
        // 2. REPORT DETAILS
        // ============================================================

        // Retrieves report configuration with its owner and audit users.
        // Useful for displaying report details with ownership and change history.
        Task<ReportConfiguration?> GetReportConfigurationDetailsAsync(int reportConfigurationId);

        // Retrieves all report configurations with their owners.
        // Useful for administrative report configuration screens.
        Task<IEnumerable<ReportConfiguration>> GetAllWithOwnersAsync();

        // ============================================================
        // 3. REPORT TYPE AND FORMAT
        // ============================================================

        // Retrieves report configurations by report type.
        // Useful for separating statement, regulatory, operational, financial and analytics reports.
        Task<IEnumerable<ReportConfiguration>> GetByReportTypeAsync(string reportType);

        // Retrieves report configurations by output format.
        // Useful for finding reports that support PDF, Excel, CSV or JSON output.
        Task<IEnumerable<ReportConfiguration>> GetByOutputFormatAsync(string outputFormat);

        // Retrieves all regulatory report configurations.
        // Useful for identifying reports intended for regulatory reporting workflows.
        Task<IEnumerable<ReportConfiguration>> GetRegulatoryReportsAsync();

        // ============================================================
        // 4. REPORT STATUS
        // ============================================================

        // Retrieves all active report configurations.
        // Useful for showing reports currently available for execution.
        Task<IEnumerable<ReportConfiguration>> GetActiveReportsAsync();

        // Retrieves all inactive report configurations.
        // Useful for reviewing disabled reports without deleting their configuration.
        Task<IEnumerable<ReportConfiguration>> GetInactiveReportsAsync();

        // ============================================================
        // 5. SCHEDULED REPORTS
        // ============================================================

        // Retrieves all report configurations marked as scheduled.
        // Useful for reviewing reports configured for automatic execution.
        Task<IEnumerable<ReportConfiguration>> GetScheduledReportsAsync();

        // Retrieves scheduled reports that are due for execution.
        // Useful for background workers that identify reports whose next run time has arrived.
        Task<IEnumerable<ReportConfiguration>> GetReportsDueForExecutionAsync(DateTime currentDateTime);

        // Retrieves scheduled reports within a specified next-run date range.
        // Useful for monitoring upcoming report execution schedules.
        Task<IEnumerable<ReportConfiguration>> GetScheduledReportsWithinRangeAsync(DateTime startDateTime, DateTime endDateTime);

        // Retrieves reports that have already been executed.
        // Useful for reviewing report configurations with a recorded last execution time.
        Task<IEnumerable<ReportConfiguration>> GetReportsPreviouslyExecutedAsync();

        // Retrieves reports that have never been executed.
        // Useful for identifying newly configured reports awaiting their first execution.
        Task<IEnumerable<ReportConfiguration>> GetReportsNeverExecutedAsync();

        // ============================================================
        // 6. USER OWNERSHIP
        // ============================================================

        // Retrieves report configurations owned by a specific user.
        // Useful for showing reports managed by an individual employee or administrator.
        Task<IEnumerable<ReportConfiguration>> GetReportsByOwnerUserIdAsync(int ownerUserId);

        // Retrieves report configurations that do not have an assigned owner.
        // Useful for finding reports that require administrative ownership.
        Task<IEnumerable<ReportConfiguration>> GetReportsWithoutOwnerAsync();

        // ============================================================
        // 7. REPORT EXECUTION HISTORY
        // ============================================================

        // Retrieves reports whose last execution occurred within a date range.
        // Useful for reviewing report execution activity over a selected period.
        Task<IEnumerable<ReportConfiguration>> GetReportsLastRunWithinRangeAsync(DateTime startDateTime, DateTime endDateTime);

        // Retrieves reports whose next execution is scheduled before a given date.
        // Useful for identifying upcoming scheduled reporting work.
        Task<IEnumerable<ReportConfiguration>> GetReportsWithNextRunBeforeAsync(DateTime dateTime);

        // ============================================================
        // 8. RETENTION AND SEARCH
        // ============================================================

        // Retrieves report configurations using a minimum retention period.
        // Useful for identifying reports with longer data-retention requirements.
        Task<IEnumerable<ReportConfiguration>> GetReportsByMinimumRetentionDaysAsync(int minimumRetentionDays);

        // Searches report configurations by code, name or description.
        // Useful for quickly locating a report in a large banking system.
        Task<IEnumerable<ReportConfiguration>> SearchReportConfigurationsAsync(string searchTerm);

        // Counts report configurations by report type.
        // Useful for reporting how many configurations exist in each report category.
        Task<int> CountByReportTypeAsync(string reportType);
    }
}
