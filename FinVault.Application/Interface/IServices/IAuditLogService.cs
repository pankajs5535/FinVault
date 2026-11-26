using FinVault.Application.DTOs.AuditLogDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAuditLogService
    {
        // 1. AUDIT LOG IDENTIFICATION
        Task<IEnumerable<AuditLogDto>> GetAllAsync();
        Task<AuditLogDto?> GetByIdAsync(int auditLogId);
        Task<AuditLogDto?> GetAuditLogDetailsAsync(int auditLogId);
        Task<AuditLogDto?> GetByEntryHashAsync(string entryHash);
        Task<AuditLogDto> CreateAsync(CreateAuditLogDto dto);

        // 2. ENTITY CHANGE HISTORY
        Task<IEnumerable<AuditLogDto>> GetEntityHistoryAsync(string entityName, int entityId);
        Task<IEnumerable<AuditLogDto>> GetLogsByEntityNameAsync(string entityName);
        Task<IEnumerable<AuditLogDto>> GetLogsByEntityAndActionAsync(
            string entityName,
            string action);
        Task<int> CountEntityHistoryAsync(string entityName, int entityId);

        // 3. USER ACTIVITY HISTORY
        Task<IEnumerable<AuditLogDto>> GetLogsByUserIdAsync(int userId);
        Task<IEnumerable<AuditLogDto>> GetLogsByCorrelationIdAsync(string correlationId);

        // 4. OUTCOME AND SECURITY MONITORING
        Task<IEnumerable<AuditLogDto>> GetLogsByOutcomeAsync(string outcome);
        Task<IEnumerable<AuditLogDto>> GetSecuritySensitiveLogsAsync();
        Task<IEnumerable<AuditLogDto>> GetFailedSecuritySensitiveLogsAsync();

        // 5. DATE RANGE AND INTEGRITY INVESTIGATION
        Task<IEnumerable<AuditLogDto>> GetLogsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<AuditLogDto>> GetLogsWithHashChainAsync(
            int pageNumber,
            int pageSize);
        Task<AuditLogDto?> GetLatestAuditLogAsync();

        // 6. PAGINATED AUDIT SEARCH
        Task<IEnumerable<AuditLogDto>> SearchAuditLogsAsync(
            string? entityName,
            string? action,
            string? outcome,
            int? userId,
            bool? isSecuritySensitive,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);
    }
}


//Banking rules:
//-Audit logs are append - only; do not expose update or delete operations.
//- Mask PAN, Aadhaar, passwords, and card data before recording old or new values.
//- Only allow Success or Failure as outcomes.
//- Preserve hash-chain information (PreviousHash and EntryHash).
//- Restrict audit-log access to authorized users.
//- Record timestamps consistently and preserve correlation IDs for request tracing.