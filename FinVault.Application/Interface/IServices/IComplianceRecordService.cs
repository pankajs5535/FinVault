using FinVault.Application.DTOs.ComplianceRecordDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IComplianceRecordService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<ComplianceRecordDto>> GetAllAsync();
        Task<ComplianceRecordDto?> GetByIdAsync(int complianceRecordId);
        Task<ComplianceRecordDto?> GetByReferenceNumberAsync(string referenceNumber);
        Task<ComplianceRecordDto?> GetComplianceRecordDetailsAsync(int complianceRecordId);
        Task<ComplianceRecordDto> CreateAsync(CreateComplianceRecordDto dto);
        Task<ComplianceRecordDto?> UpdateAsync(UpdateComplianceRecordDto dto);
        Task<bool> DeleteAsync(DeleteComplianceRecordDto dto);
        Task<bool> ExistsByReferenceNumberAsync(string referenceNumber);

        // 2. CUSTOMER AND BANKING RECORD RELATIONSHIPS
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByCustomerIdAsync(int customerId);
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByAccountIdAsync(int accountId);
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByTransactionIdAsync(int transactionId);
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByFraudAlertIdAsync(int fraudAlertId);

        // 3. RECORD TYPE AND RISK CLASSIFICATION
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByTypeAsync(string recordType);
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByRiskLevelAsync(string riskLevel);
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByStatusAsync(string status);

        // 4. REVIEW AND FILING MANAGEMENT
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsByFiledByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<ComplianceRecordDto>> GetOverdueRecordsAsync(DateTime currentDate);
        Task<IEnumerable<ComplianceRecordDto>> GetRecordsDueWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<ComplianceRecordDto>> GetFiledRecordsWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 5. COMPLIANCE SEARCH AND REPORTING
        Task<IEnumerable<ComplianceRecordDto>> SearchComplianceRecordsAsync(
            string? searchTerm,
            string? recordType,
            string? status,
            string? riskLevel,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);

        Task<int> CountRecordsByCustomerIdAsync(int customerId);
    }
}


//Banking rules:
//-Enforce unique compliance reference numbers.
//- Restrict access to authorized compliance personnel.
//- Preserve filing references, status history and audit information.
//- Do not silently alter filed regulatory records; use controlled correction procedures.
//- Track overdue filing obligations against their due dates.