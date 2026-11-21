
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IRiskAssessmentRepository : IGenericRepository<RiskAssessment>
    {
        // =========================================================
        // 1. RISK ASSESSMENT IDENTIFICATION
        // =========================================================

        // Retrieves complete risk assessment details with its subject, audit users and fraud alerts.
        // Useful for investigating why a customer, account or transaction received a risk decision.
        Task<RiskAssessment?> GetRiskAssessmentDetailsAsync(int riskAssessmentId);

        // Retrieves assessments using their primary key.
        // Useful for accessing a specific risk evaluation record.
        Task<RiskAssessment?> GetByIdWithSubjectAsync(int riskAssessmentId);

        // =========================================================
        // 2. CUSTOMER ASSESSMENTS
        // =========================================================

        // Retrieves all risk assessments for a customer.
        // Useful for reviewing a customer's historical risk profile.
        Task<IEnumerable<RiskAssessment>> GetByCustomerIdAsync(int customerId);

        // Retrieves the latest risk assessment for a customer.
        // Useful for checking the most recent recorded customer risk evaluation.
        Task<RiskAssessment?> GetLatestByCustomerIdAsync(int customerId);

        // Retrieves customer assessments within a date range.
        // Useful for reviewing changes in customer risk over time.
        Task<IEnumerable<RiskAssessment>> GetCustomerAssessmentsWithinRangeAsync(int customerId, DateTime startDate, DateTime endDate);

        // =========================================================
        // 3. ACCOUNT ASSESSMENTS
        // =========================================================

        // Retrieves all risk assessments for an account.
        // Useful for reviewing account-level risk history and monitoring.
        Task<IEnumerable<RiskAssessment>> GetByAccountIdAsync(int accountId);

        // Retrieves the latest risk assessment for an account.
        // Useful for checking the most recent recorded account risk evaluation.
        Task<RiskAssessment?> GetLatestByAccountIdAsync(int accountId);

        // =========================================================
        // 4. TRANSACTION ASSESSMENTS
        // =========================================================

        // Retrieves all risk assessments for a transaction.
        // Useful for investigating transaction screening and risk decisions.
        Task<IEnumerable<RiskAssessment>> GetByTransactionIdAsync(int transactionId);

        // Retrieves the latest risk assessment for a transaction.
        // Useful for identifying the most recent risk decision associated with a transaction.
        Task<RiskAssessment?> GetLatestByTransactionIdAsync(int transactionId);

        // =========================================================
        // 5. ASSESSMENT TYPE AND RISK LEVEL
        // =========================================================

        // Retrieves assessments matching a particular subject type.
        // Useful for separating customer, account and transaction risk monitoring.
        Task<IEnumerable<RiskAssessment>> GetByAssessmentTypeAsync(string assessmentType);

        // Retrieves assessments matching a particular risk level.
        // Useful for prioritizing high-risk and critical-risk cases for review.
        Task<IEnumerable<RiskAssessment>> GetByRiskLevelAsync(string riskLevel);

        // Retrieves assessments matching a particular decision.
        // Useful for reviewing approved, review-required, challenged and declined cases.
        Task<IEnumerable<RiskAssessment>> GetByDecisionAsync(string decision);

        // Retrieves high-risk and critical-risk assessments.
        // Useful for identifying cases that may require enhanced investigation.
        Task<IEnumerable<RiskAssessment>> GetHighAndCriticalRiskAssessmentsAsync();

        // Retrieves assessments with a score at or above the supplied threshold.
        // Useful for identifying cases that meet a configured risk-review threshold.
        Task<IEnumerable<RiskAssessment>> GetByMinimumRiskScoreAsync(decimal minimumRiskScore);

        // =========================================================
        // 6. MODEL AND VALIDITY
        // =========================================================

        // Retrieves assessments produced by a particular model version.
        // Useful for comparing risk decisions across model releases.
        Task<IEnumerable<RiskAssessment>> GetByModelVersionAsync(string modelVersion);

        // Retrieves assessments that have not expired as of the supplied date.
        // Useful for finding risk evaluations that are still within their recorded validity period.
        Task<IEnumerable<RiskAssessment>> GetNonExpiredAssessmentsAsync(DateTime currentDateTime);

        // Retrieves assessments whose expiry date has passed.
        // Useful for identifying risk evaluations that may need to be refreshed.
        Task<IEnumerable<RiskAssessment>> GetExpiredAssessmentsAsync(DateTime currentDateTime);

        // =========================================================
        // 7. DATE AND AUDIT
        // =========================================================

        // Retrieves assessments completed within a date range.
        // Useful for daily and monthly risk assessment reporting.
        Task<IEnumerable<RiskAssessment>> GetAssessedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves assessments created by a particular user.
        // Useful for auditing who initiated or recorded risk assessments.
        Task<IEnumerable<RiskAssessment>> GetByCreatedByAsync(int userId);

        // Retrieves assessments last updated by a particular user.
        // Useful for tracking responsibility for changes to assessment records.
        Task<IEnumerable<RiskAssessment>> GetByUpdatedByAsync(int userId);

        // =========================================================
        // 8. FRAUD ALERTS AND SEARCH
        // =========================================================

        // Retrieves assessments that have generated fraud alerts.
        // Useful for investigating risk evaluations associated with fraud monitoring.
        Task<IEnumerable<RiskAssessment>> GetAssessmentsWithFraudAlertsAsync();

        // Retrieves one assessment with its associated fraud alerts.
        // Useful for reviewing the fraud alerts generated from a specific risk assessment.
        Task<RiskAssessment?> GetAssessmentWithFraudAlertsAsync(int riskAssessmentId);

        // Searches assessments by model version, assessment type or explanation factors.
        // Useful for locating risk evaluations during model and fraud investigations.
        Task<IEnumerable<RiskAssessment>> SearchRiskAssessmentsAsync(string searchTerm);

        // =========================================================
        // 9. REPORTING
        // =========================================================

        // Counts assessments matching a particular risk level within a date range.
        // Useful for reporting the volume of low, medium, high and critical risk cases.
        Task<int> CountByRiskLevelWithinRangeAsync(string riskLevel, DateTime startDate, DateTime endDate);

        // Calculates the average risk score for a particular assessment type.
        // Useful for monitoring overall customer, account or transaction risk trends.
        Task<decimal> GetAverageRiskScoreByAssessmentTypeAsync(string assessmentType);
    }
}
