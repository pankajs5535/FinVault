
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class ReportConfigurationRepository : GenericRepository<ReportConfiguration>, IReportConfigurationRepository
    {
        private readonly FinVaultDbContext _context;
        private readonly DbSet<ReportConfiguration> _reportConfigurations;

        public ReportConfigurationRepository(FinVaultDbContext context) : base(context)
        {
            _context = context;
            _reportConfigurations = context.Set<ReportConfiguration>();
        }

        // ============================================================
        // 1. REPORT IDENTIFICATION
        // ============================================================

        // Retrieves a report configuration using its unique report code.
        // Useful for identifying a specific report without relying on its database ID.
        public async Task<ReportConfiguration?> GetByReportCodeAsync(string reportCode)
        {
            return await _reportConfigurations.AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReportCode == reportCode);
        }

        // Checks whether a report code already exists.
        // Useful for preventing duplicate report configurations.
        public async Task<bool> ExistsByReportCodeAsync(string reportCode)
        {
            return await _reportConfigurations.AnyAsync(r => r.ReportCode == reportCode);
        }

        // ============================================================
        // 2. REPORT DETAILS
        // ============================================================

        // Retrieves report configuration with its owner and audit users.
        // Useful for displaying report details with ownership and change history.
        public async Task<ReportConfiguration?> GetReportConfigurationDetailsAsync(int reportConfigurationId)
        {
            return await _reportConfigurations.AsNoTracking()
                .Include(r => r.OwnerUser)
                .Include(r => r.CreatedByUser)
                .Include(r => r.UpdatedByUser)
                .FirstOrDefaultAsync(r => r.ReportConfigurationId == reportConfigurationId);
        }

        // Retrieves all report configurations with their owners.
        // Useful for administrative report configuration screens.
        public async Task<IEnumerable<ReportConfiguration>> GetAllWithOwnersAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Include(r => r.OwnerUser)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // ============================================================
        // 3. REPORT TYPE AND FORMAT
        // ============================================================

        // Retrieves report configurations by report type.
        // Useful for separating statement, regulatory, operational, financial and analytics reports.
        public async Task<IEnumerable<ReportConfiguration>> GetByReportTypeAsync(string reportType)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.ReportType == reportType)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // Retrieves report configurations by output format.
        // Useful for finding reports that support PDF, Excel, CSV or JSON output.
        public async Task<IEnumerable<ReportConfiguration>> GetByOutputFormatAsync(string outputFormat)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.OutputFormat == outputFormat)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // Retrieves all regulatory report configurations.
        // Useful for identifying reports intended for regulatory reporting workflows.
        public async Task<IEnumerable<ReportConfiguration>> GetRegulatoryReportsAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.IsRegulatory)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // ============================================================
        // 4. REPORT STATUS
        // ============================================================

        // Retrieves all active report configurations.
        // Useful for showing reports currently available for execution.
        public async Task<IEnumerable<ReportConfiguration>> GetActiveReportsAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // Retrieves all inactive report configurations.
        // Useful for reviewing disabled reports without deleting their configuration.
        public async Task<IEnumerable<ReportConfiguration>> GetInactiveReportsAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => !r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // ============================================================
        // 5. SCHEDULED REPORTS
        // ============================================================

        // Retrieves all report configurations marked as scheduled.
        // Useful for reviewing reports configured for automatic execution.
        public async Task<IEnumerable<ReportConfiguration>> GetScheduledReportsAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.IsScheduled)
                .OrderBy(r => r.NextRunAt)
                .ToListAsync();
        }

        // Retrieves scheduled reports that are due for execution.
        // Useful for background workers that identify reports whose next run time has arrived.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsDueForExecutionAsync(DateTime currentDateTime)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.IsActive && r.IsScheduled && r.NextRunAt.HasValue && r.NextRunAt.Value <= currentDateTime)
                .OrderBy(r => r.NextRunAt)
                .ToListAsync();
        }

        // Retrieves scheduled reports within a specified next-run date range.
        // Useful for monitoring upcoming report execution schedules.
        public async Task<IEnumerable<ReportConfiguration>> GetScheduledReportsWithinRangeAsync(DateTime startDateTime, DateTime endDateTime)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.IsScheduled && r.NextRunAt.HasValue && r.NextRunAt.Value >= startDateTime && r.NextRunAt.Value <= endDateTime)
                .OrderBy(r => r.NextRunAt)
                .ToListAsync();
        }

        // Retrieves reports that have already been executed.
        // Useful for reviewing report configurations with a recorded last execution time.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsPreviouslyExecutedAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.LastRunAt.HasValue)
                .OrderByDescending(r => r.LastRunAt)
                .ToListAsync();
        }

        // Retrieves reports that have never been executed.
        // Useful for identifying newly configured reports awaiting their first execution.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsNeverExecutedAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => !r.LastRunAt.HasValue)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // ============================================================
        // 6. USER OWNERSHIP
        // ============================================================

        // Retrieves report configurations owned by a specific user.
        // Useful for showing reports managed by an individual employee or administrator.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsByOwnerUserIdAsync(int ownerUserId)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.OwnerUserId == ownerUserId)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // Retrieves report configurations that do not have an assigned owner.
        // Useful for finding reports that require administrative ownership.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsWithoutOwnerAsync()
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => !r.OwnerUserId.HasValue)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // ============================================================
        // 7. REPORT EXECUTION HISTORY
        // ============================================================

        // Retrieves reports whose last execution occurred within a date range.
        // Useful for reviewing report execution activity over a selected period.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsLastRunWithinRangeAsync(DateTime startDateTime, DateTime endDateTime)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.LastRunAt.HasValue && r.LastRunAt.Value >= startDateTime && r.LastRunAt.Value <= endDateTime)
                .OrderByDescending(r => r.LastRunAt)
                .ToListAsync();
        }

        // Retrieves reports whose next execution is scheduled before a given date.
        // Useful for identifying upcoming scheduled reporting work.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsWithNextRunBeforeAsync(DateTime dateTime)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.IsScheduled && r.NextRunAt.HasValue && r.NextRunAt.Value <= dateTime)
                .OrderBy(r => r.NextRunAt)
                .ToListAsync();
        }

        // ============================================================
        // 8. RETENTION AND SEARCH
        // ============================================================

        // Retrieves report configurations using a minimum retention period.
        // Useful for identifying reports with longer data-retention requirements.
        public async Task<IEnumerable<ReportConfiguration>> GetReportsByMinimumRetentionDaysAsync(int minimumRetentionDays)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.RetentionDays >= minimumRetentionDays)
                .OrderByDescending(r => r.RetentionDays)
                .ToListAsync();
        }

        // Searches report configurations by code, name or description.
        // Useful for quickly locating a report in a large banking system.
        public async Task<IEnumerable<ReportConfiguration>> SearchReportConfigurationsAsync(string searchTerm)
        {
            return await _reportConfigurations.AsNoTracking()
                .Where(r => r.ReportCode.Contains(searchTerm) || r.Name.Contains(searchTerm) || (r.Description != null && r.Description.Contains(searchTerm)))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // Counts report configurations by report type.
        // Useful for reporting how many configurations exist in each report category.
        public async Task<int> CountByReportTypeAsync(string reportType)
        {
            return await _reportConfigurations.CountAsync(r => r.ReportType == reportType);
        }
    }
}
