
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IComplianceRecordRepository : IGenericRepository<ComplianceRecord>
    {
        // =====================================================
        // 1. COMPLIANCE RECORD IDENTIFICATION
        // =====================================================

        // Finds a compliance record using its unique reference number.
        // Useful for locating a regulatory case during investigation or audit.
        Task<ComplianceRecord?> GetByReferenceNumberAsync(string referenceNumber);

        // Retrieves a compliance record with its related customer, account, transaction and fraud alert.
        // Useful for reviewing the complete context of a compliance case.
        Task<ComplianceRecord?> GetComplianceRecordDetailsAsync(int complianceRecordId);


        // =====================================================
        // 2. CUSTOMER AND BANKING RECORD RELATIONSHIPS
        // =====================================================

        // Retrieves compliance records associated with a customer.
        // Useful for reviewing a customer's regulatory and AML history.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByCustomerIdAsync(int customerId);

        // Retrieves compliance records associated with an account.
        // Useful for investigating account-level compliance concerns.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByAccountIdAsync(int accountId);

        // Retrieves compliance records associated with a transaction.
        // Useful for investigating transactions reported for regulatory review.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByTransactionIdAsync(int transactionId);

        // Retrieves compliance records associated with a fraud alert.
        // Useful for tracing regulatory follow-up linked to fraud detection.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByFraudAlertIdAsync(int fraudAlertId);


        // =====================================================
        // 3. RECORD TYPE AND RISK CLASSIFICATION
        // =====================================================

        // Retrieves compliance records belonging to a particular record type.
        // Useful for separating STR, CTR, AML screening, PEP and other regulatory workflows.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByTypeAsync(string recordType);

        // Retrieves compliance records matching a particular risk level.
        // Useful for prioritizing high-risk and critical compliance cases.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByRiskLevelAsync(string riskLevel);

        // Retrieves compliance records matching a particular status.
        // Useful for monitoring open, under-review, filed, cleared and closed cases.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByStatusAsync(string status);


        // =====================================================
        // 4. REVIEW AND FILING MANAGEMENT
        // =====================================================

        // Retrieves compliance records assigned to a particular filing employee.
        // Useful for tracking an employee's regulatory filing responsibilities.
        Task<IEnumerable<ComplianceRecord>> GetRecordsByFiledByEmployeeIdAsync(int employeeId);

        // Retrieves records whose filing due date has passed and that are not yet filed or closed.
        // Useful for identifying overdue compliance obligations.
        Task<IEnumerable<ComplianceRecord>> GetOverdueRecordsAsync(DateTime currentDate);

        // Retrieves records with filing due dates within a specified range.
        // Useful for planning upcoming regulatory submissions.
        Task<IEnumerable<ComplianceRecord>> GetRecordsDueWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves records filed within a specified date range.
        // Useful for regulatory filing history and compliance reporting.
        Task<IEnumerable<ComplianceRecord>> GetFiledRecordsWithinRangeAsync(DateTime startDate, DateTime endDate);


        // =====================================================
        // 5. COMPLIANCE SEARCH AND VALIDATION
        // =====================================================

        // Searches compliance records using reference, description and filing reference.
        // Optional filters and pagination support compliance case investigations.
        Task<IEnumerable<ComplianceRecord>> SearchComplianceRecordsAsync(string? searchTerm, string? recordType, string? status, string? riskLevel, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);

        // Checks whether a compliance record exists for a particular reference number.
        // Useful for preventing duplicate regulatory case references.
        Task<bool> ExistsByReferenceNumberAsync(string referenceNumber);

        // Counts compliance records associated with a particular customer.
        // Useful for customer-level compliance reporting.
        Task<int> CountRecordsByCustomerIdAsync(int customerId);
    }
}
