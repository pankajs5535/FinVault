
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. AUDIT LOG IDENTIFICATION
        // =====================================================

        // Retrieves an audit log with its associated user.
        // Useful for investigating who performed a recorded action.
        public async Task<AuditLog?> GetAuditLogDetailsAsync(int auditLogId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.AuditLogId == auditLogId);
        }

        // Retrieves an audit log using its tamper-evident entry hash.
        // Useful for locating a specific entry during integrity verification.
        public async Task<AuditLog?> GetByEntryHashAsync(string entryHash)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EntryHash == entryHash);
        }


        // =====================================================
        // 2. ENTITY CHANGE HISTORY
        // =====================================================

        // Retrieves audit history for a particular entity record.
        // Useful for reviewing changes made to a customer, account or other banking record.
        public async Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityName, int entityId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.EntityName == entityName && x.EntityId == entityId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.AuditLogId)
                .ToListAsync();
        }

        // Retrieves audit logs for a particular entity type.
        // Useful for reviewing activity across all records of an entity.
        public async Task<IEnumerable<AuditLog>> GetLogsByEntityNameAsync(string entityName)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.EntityName == entityName)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves audit logs for a particular entity and action.
        // Useful for investigating specific operations performed on banking records.
        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAndActionAsync(string entityName, string action)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.EntityName == entityName && x.Action == action)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 3. USER ACTIVITY HISTORY
        // =====================================================

        // Retrieves audit logs recorded for a particular user.
        // Useful for reviewing a user's activity history.
        public async Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves audit logs associated with a particular correlation ID.
        // Useful for tracing all recorded events belonging to one request or workflow.
        public async Task<IEnumerable<AuditLog>> GetLogsByCorrelationIdAsync(string correlationId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CorrelationId == correlationId)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.AuditLogId)
                .ToListAsync();
        }


        // =====================================================
        // 4. OUTCOME AND SECURITY MONITORING
        // =====================================================

        // Retrieves audit logs with a particular outcome.
        // Useful for separating successful and failed operations.
        public async Task<IEnumerable<AuditLog>> GetLogsByOutcomeAsync(string outcome)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Outcome == outcome)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves audit logs marked as security-sensitive.
        // Useful for monitoring sensitive banking and security-related activity.
        public async Task<IEnumerable<AuditLog>> GetSecuritySensitiveLogsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.IsSecuritySensitive)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves failed security-sensitive operations.
        // Useful for investigating unsuccessful security-critical actions.
        public async Task<IEnumerable<AuditLog>> GetFailedSecuritySensitiveLogsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.IsSecuritySensitive && x.Outcome == "Failure")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 5. DATE RANGE AND INTEGRITY INVESTIGATION
        // =====================================================

        // Retrieves audit logs created within a specified date range.
        // Useful for compliance reviews and audit-period reporting.
        public async Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves audit logs that contain hash-chain information.
        // Useful for reviewing entries participating in tamper-evidence verification.
        public async Task<IEnumerable<AuditLog>> GetLogsWithHashChainAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.EntryHash != null)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.AuditLogId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Retrieves the latest audit log entry.
        // Useful for obtaining the current end of the audit hash chain.
        public async Task<AuditLog?> GetLatestAuditLogAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.EntryHash != null)
                .OrderByDescending(x => x.AuditLogId)
                .FirstOrDefaultAsync();
        }


        // =====================================================
        // 6. PAGINATED AUDIT SEARCH
        // =====================================================

        // Searches audit logs using entity, action, outcome, user and security filters.
        // Pagination prevents loading large audit histories at once.
        public async Task<IEnumerable<AuditLog>> SearchAuditLogsAsync(string? entityName, string? action, string? outcome, int? userId, bool? isSecuritySensitive, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
                throw new ArgumentException("End date cannot be earlier than start date.");

            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityName))
                query = query.Where(x => x.EntityName == entityName.Trim());

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(x => x.Action == action.Trim());

            if (!string.IsNullOrWhiteSpace(outcome))
                query = query.Where(x => x.Outcome == outcome.Trim());

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (isSecuritySensitive.HasValue)
                query = query.Where(x => x.IsSecuritySensitive == isSecuritySensitive.Value);

            if (startDate.HasValue)
                query = query.Where(x => x.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.CreatedAt <= endDate.Value);

            return await query.OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.AuditLogId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Counts audit logs recorded for a particular entity record.
        // Useful for measuring the recorded change history of a banking record.
        public async Task<int> CountEntityHistoryAsync(string entityName, int entityId)
        {
            return await _dbSet.CountAsync(x => x.EntityName == entityName && x.EntityId == entityId);
        }
    }
}
