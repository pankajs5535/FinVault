using FinVault.Application.DTOs.FraudAlertDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IFraudAlertService
    {
        // 1. ALERT IDENTIFICATION
        Task<IEnumerable<FraudAlertDto>> GetAllAsync();
        Task<FraudAlertDto?> GetByIdAsync(int fraudAlertId);
        Task<FraudAlertDto?> GetByAlertNumberAsync(string alertNumber);
        Task<FraudAlertDto?> GetFraudAlertDetailsAsync(int fraudAlertId);
        Task<bool> ExistsByAlertNumberAsync(string alertNumber);
        Task<FraudAlertDto> CreateAsync(CreateFraudAlertDto dto);
        Task<FraudAlertDto?> UpdateAsync(UpdateFraudAlertDto dto);
        Task<bool> DeleteAsync(DeleteFraudAlertDto dto);

        // 2. ALERT STATUS AND SEVERITY
        Task<IEnumerable<FraudAlertDto>> GetByStatusAsync(string status);
        Task<IEnumerable<FraudAlertDto>> GetBySeverityAsync(string severity);
        Task<IEnumerable<FraudAlertDto>> GetAlertsAboveScoreAsync(decimal minimumScore);
        Task<IEnumerable<FraudAlertDto>> GetActiveFraudAlertsAsync();

        // 3. BANKING ENTITY ASSOCIATIONS
        Task<IEnumerable<FraudAlertDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<FraudAlertDto>> GetByAccountIdAsync(int accountId);
        Task<IEnumerable<FraudAlertDto>> GetByTransactionIdAsync(int transactionId);
        Task<IEnumerable<FraudAlertDto>> GetByCardIdAsync(int cardId);
        Task<IEnumerable<FraudAlertDto>> GetByFraudRuleIdAsync(int fraudRuleId);
        Task<IEnumerable<FraudAlertDto>> GetByRiskAssessmentIdAsync(int riskAssessmentId);

        // 4. INVESTIGATION ASSIGNMENT
        Task<IEnumerable<FraudAlertDto>> GetByReviewedByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<FraudAlertDto>> GetUnassignedFraudAlertsAsync();

        // 5. INVESTIGATION OUTCOMES
        Task<IEnumerable<FraudAlertDto>> GetConfirmedFraudAlertsAsync();
        Task<IEnumerable<FraudAlertDto>> GetFalsePositiveAlertsAsync();
        Task<IEnumerable<FraudAlertDto>> GetAlertsWithComplianceRecordsAsync();

        // 6. ALERT TIMELINE AND SEARCH
        Task<IEnumerable<FraudAlertDto>> GetAlertsRaisedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<FraudAlertDto>> GetAlertsReviewedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<FraudAlertDto>> SearchFraudAlertsAsync(string searchTerm);

        // 7. REPORTING
        Task<int> CountAlertsByCustomerIdAsync(int customerId);
    }
}

//Banking rules:
//-Alert numbers must be unique.
//- Preserve the original alert and its linked fraud rule.
//- Restrict investigation and review actions to authorized fraud analysts.
//- Record review notes and action taken when an alert is reviewed.
//- Preserve confirmed-fraud and false-positive outcomes for audit and fraud analysis.