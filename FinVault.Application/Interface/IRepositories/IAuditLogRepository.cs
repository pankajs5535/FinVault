
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        // =====================================================
        // 1. AUDIT LOG IDENTIFICATION
        // =====================================================

        // Retrieves an audit log with its associated user.
        // Useful for investigating who performed a recorded action.
        Task<AuditLog?> GetAuditLogDetailsAsync(int auditLogId);

        // Retrieves an audit log using its tamper-evident entry hash.
        // Useful for locating a specific entry during integrity verification.
        Task<AuditLog?> GetByEntryHashAsync(string entryHash);


        // =====================================================
        // 2. ENTITY CHANGE HISTORY
        // =====================================================

        // Retrieves audit history for a particular entity record.
        // Useful for reviewing changes made to a customer, account or other banking record.
        Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityName, int entityId);

        // Retrieves audit logs for a particular entity type.
        // Useful for reviewing activity across all records of an entity.
        Task<IEnumerable<AuditLog>> GetLogsByEntityNameAsync(string entityName);

        // Retrieves audit logs for a particular entity and action.
        // Useful for investigating specific operations performed on banking records.
        Task<IEnumerable<AuditLog>> GetLogsByEntityAndActionAsync(string entityName, string action);


        // =====================================================
        // 3. USER ACTIVITY HISTORY
        // =====================================================

        // Retrieves audit logs recorded for a particular user.
        // Useful for reviewing a user's activity history.
        Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(int userId);

        // Retrieves audit logs associated with a particular correlation ID.
        // Useful for tracing all recorded events belonging to one request or workflow.
        Task<IEnumerable<AuditLog>> GetLogsByCorrelationIdAsync(string correlationId);


        // =====================================================
        // 4. OUTCOME AND SECURITY MONITORING
        // =====================================================

        // Retrieves audit logs with a particular outcome.
        // Useful for separating successful and failed operations.
        Task<IEnumerable<AuditLog>> GetLogsByOutcomeAsync(string outcome);

        // Retrieves audit logs marked as security-sensitive.
        // Useful for monitoring sensitive banking and security-related activity.
        Task<IEnumerable<AuditLog>> GetSecuritySensitiveLogsAsync();

        // Retrieves failed security-sensitive operations.
        // Useful for investigating unsuccessful security-critical actions.
        Task<IEnumerable<AuditLog>> GetFailedSecuritySensitiveLogsAsync();


        // =====================================================
        // 5. DATE RANGE AND INTEGRITY INVESTIGATION
        // =====================================================

        // Retrieves audit logs created within a specified date range.
        // Useful for compliance reviews and audit-period reporting.
        Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves audit logs that contain hash-chain information.
        // Useful for reviewing entries participating in tamper-evidence verification.
        Task<IEnumerable<AuditLog>> GetLogsWithHashChainAsync(int pageNumber, int pageSize);

        // Retrieves the latest audit log entry.
        // Useful for obtaining the current end of the audit hash chain.
        Task<AuditLog?> GetLatestAuditLogAsync();


        // =====================================================
        // 6. PAGINATED AUDIT SEARCH
        // =====================================================

        // Searches audit logs using entity, action, outcome, user and security filters.
        // Pagination prevents loading large audit histories at once.
        Task<IEnumerable<AuditLog>> SearchAuditLogsAsync(string? entityName, string? action, string? outcome, int? userId, bool? isSecuritySensitive, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);

        // Counts audit logs recorded for a particular entity record.
        // Useful for measuring the recorded change history of a banking record.
        Task<int> CountEntityHistoryAsync(string entityName, int entityId);
    }
}
