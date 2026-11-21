
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class ComplianceRecordRepository : GenericRepository<ComplianceRecord>, IComplianceRecordRepository
    {
        public ComplianceRecordRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. COMPLIANCE RECORD IDENTIFICATION
        // =====================================================

        // Finds a compliance record using its unique reference number.
        // Useful for locating a regulatory case during investigation or audit.
        public async Task<ComplianceRecord?> GetByReferenceNumberAsync(string referenceNumber)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ReferenceNumber == referenceNumber);
        }

        // Retrieves a compliance record with its related customer, account, transaction and fraud alert.
        // Useful for reviewing the complete context of a compliance case.
        public async Task<ComplianceRecord?> GetComplianceRecordDetailsAsync(int complianceRecordId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Account)
                .Include(x => x.Transaction)
                .Include(x => x.FraudAlert)
                .Include(x => x.FiledByEmployee)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.ComplianceRecordId == complianceRecordId);
        }


        // =====================================================
        // 2. CUSTOMER AND BANKING RECORD RELATIONSHIPS
        // =====================================================

        // Retrieves compliance records associated with a customer.
        // Useful for reviewing a customer's regulatory and AML history.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves compliance records associated with an account.
        // Useful for investigating account-level compliance concerns.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByAccountIdAsync(int accountId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves compliance records associated with a transaction.
        // Useful for investigating transactions reported for regulatory review.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByTransactionIdAsync(int transactionId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.TransactionId == transactionId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves compliance records associated with a fraud alert.
        // Useful for tracing regulatory follow-up linked to fraud detection.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByFraudAlertIdAsync(int fraudAlertId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.FraudAlertId == fraudAlertId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 3. RECORD TYPE AND RISK CLASSIFICATION
        // =====================================================

        // Retrieves compliance records belonging to a particular record type.
        // Useful for separating STR, CTR, AML screening, PEP and other regulatory workflows.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByTypeAsync(string recordType)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.RecordType == recordType)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves compliance records matching a particular risk level.
        // Useful for prioritizing high-risk and critical compliance cases.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByRiskLevelAsync(string riskLevel)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.RiskLevel == riskLevel)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves compliance records matching a particular status.
        // Useful for monitoring open, under-review, filed, cleared and closed cases.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByStatusAsync(string status)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 4. REVIEW AND FILING MANAGEMENT
        // =====================================================

        // Retrieves compliance records assigned to a particular filing employee.
        // Useful for tracking an employee's regulatory filing responsibilities.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsByFiledByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.FiledByEmployeeId == employeeId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves records whose filing due date has passed and that are not yet filed or closed.
        // Useful for identifying overdue compliance obligations.
        public async Task<IEnumerable<ComplianceRecord>> GetOverdueRecordsAsync(DateTime currentDate)
        {
            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.DueDate.HasValue &&
                    x.DueDate.Value < currentDate &&
                    (x.Status == "Open" || x.Status == "UnderReview"))
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves records with filing due dates within a specified range.
        // Useful for planning upcoming regulatory submissions.
        public async Task<IEnumerable<ComplianceRecord>> GetRecordsDueWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.DueDate >= startDate && x.DueDate <= endDate)
                .OrderBy(x => x.DueDate)
                .ToListAsync();
        }

        // Retrieves records filed within a specified date range.
        // Useful for regulatory filing history and compliance reporting.
        public async Task<IEnumerable<ComplianceRecord>> GetFiledRecordsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.FiledAt.HasValue &&
                    x.FiledAt.Value >= startDate &&
                    x.FiledAt.Value <= endDate)
                .OrderByDescending(x => x.FiledAt)
                .ToListAsync();
        }


        // =====================================================
        // 5. COMPLIANCE SEARCH AND VALIDATION
        // =====================================================

        // Searches compliance records using reference, description and filing reference.
        // Optional filters and pagination support compliance case investigations.
        public async Task<IEnumerable<ComplianceRecord>> SearchComplianceRecordsAsync(string? searchTerm, string? recordType, string? status, string? riskLevel, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
                throw new ArgumentException("End date cannot be earlier than start date.");

            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(x =>
                    x.ReferenceNumber.Contains(searchTerm) ||
                    x.Description.Contains(searchTerm) ||
                    (x.FilingReference != null && x.FilingReference.Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(recordType))
                query = query.Where(x => x.RecordType == recordType.Trim());

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status.Trim());

            if (!string.IsNullOrWhiteSpace(riskLevel))
                query = query.Where(x => x.RiskLevel == riskLevel.Trim());

            if (startDate.HasValue)
                query = query.Where(x => x.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.CreatedAt <= endDate.Value);

            return await query.OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Checks whether a compliance record exists for a particular reference number.
        // Useful for preventing duplicate regulatory case references.
        public async Task<bool> ExistsByReferenceNumberAsync(string referenceNumber)
        {
            return await _dbSet.AnyAsync(x => x.ReferenceNumber == referenceNumber);
        }

        // Counts compliance records associated with a particular customer.
        // Useful for customer-level compliance reporting.
        public async Task<int> CountRecordsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.CountAsync(x => x.CustomerId == customerId);
        }
    }
}
