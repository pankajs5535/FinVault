using FinVault.Application.DTOs.RiskAssessmentDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IRiskAssessmentService
    {
        // 1. RISK ASSESSMENT IDENTIFICATION
        Task<IEnumerable<RiskAssessmentDto>> GetAllAsync();
        Task<RiskAssessmentDto?> GetByIdAsync(int riskAssessmentId);
        Task<RiskAssessmentDto?> GetRiskAssessmentDetailsAsync(int riskAssessmentId);
        Task<RiskAssessmentDto?> GetByIdWithSubjectAsync(int riskAssessmentId);
        Task<RiskAssessmentDto> CreateAsync(CreateRiskAssessmentDto dto);
        Task<RiskAssessmentDto?> UpdateAsync(UpdateRiskAssessmentDto dto);
        Task<bool> DeleteAsync(DeleteRiskAssessmentDto dto);

        // 2. CUSTOMER ASSESSMENTS
        Task<IEnumerable<RiskAssessmentDto>> GetByCustomerIdAsync(int customerId);
        Task<RiskAssessmentDto?> GetLatestByCustomerIdAsync(int customerId);
        Task<IEnumerable<RiskAssessmentDto>> GetCustomerAssessmentsWithinRangeAsync(
            int customerId,
            DateTime startDate,
            DateTime endDate);

        // 3. ACCOUNT ASSESSMENTS
        Task<IEnumerable<RiskAssessmentDto>> GetByAccountIdAsync(int accountId);
        Task<RiskAssessmentDto?> GetLatestByAccountIdAsync(int accountId);

        // 4. TRANSACTION ASSESSMENTS
        Task<IEnumerable<RiskAssessmentDto>> GetByTransactionIdAsync(int transactionId);
        Task<RiskAssessmentDto?> GetLatestByTransactionIdAsync(int transactionId);

        // 5. ASSESSMENT TYPE AND RISK LEVEL
        Task<IEnumerable<RiskAssessmentDto>> GetByAssessmentTypeAsync(string assessmentType);
        Task<IEnumerable<RiskAssessmentDto>> GetByRiskLevelAsync(string riskLevel);
        Task<IEnumerable<RiskAssessmentDto>> GetByDecisionAsync(string decision);
        Task<IEnumerable<RiskAssessmentDto>> GetHighAndCriticalRiskAssessmentsAsync();
        Task<IEnumerable<RiskAssessmentDto>> GetByMinimumRiskScoreAsync(decimal minimumRiskScore);

        // 6. MODEL AND VALIDITY
        Task<IEnumerable<RiskAssessmentDto>> GetByModelVersionAsync(string modelVersion);
        Task<IEnumerable<RiskAssessmentDto>> GetNonExpiredAssessmentsAsync(DateTime currentDateTime);
        Task<IEnumerable<RiskAssessmentDto>> GetExpiredAssessmentsAsync(DateTime currentDateTime);

        // 7. DATE AND AUDIT
        Task<IEnumerable<RiskAssessmentDto>> GetAssessedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<RiskAssessmentDto>> GetByCreatedByAsync(int userId);
        Task<IEnumerable<RiskAssessmentDto>> GetByUpdatedByAsync(int userId);

        // 8. FRAUD ALERTS AND SEARCH
        Task<IEnumerable<RiskAssessmentDto>> GetAssessmentsWithFraudAlertsAsync();
        Task<RiskAssessmentDto?> GetAssessmentWithFraudAlertsAsync(int riskAssessmentId);
        Task<IEnumerable<RiskAssessmentDto>> SearchRiskAssessmentsAsync(string searchTerm);

        // 9. REPORTING
        Task<int> CountByRiskLevelWithinRangeAsync(
            string riskLevel,
            DateTime startDate,
            DateTime endDate);
        Task<decimal> GetAverageRiskScoreByAssessmentTypeAsync(string assessmentType);
    }
}


//Banking rules:
//-Risk scores must remain within the configured 0–100 range.
//- Assessment type must identify a Customer, Account, or Transaction.
//- Risk level and decision must use the allowed values defined by the DTOs.
//- Risk assessment factors must not store raw PAN, Aadhaar, or card data.
//- Preserve assessment history and model version for auditability.
//- Expired assessments should not be treated as current risk evaluations.