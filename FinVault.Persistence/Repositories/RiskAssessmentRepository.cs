
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class RiskAssessmentRepository : GenericRepository<RiskAssessment>, IRiskAssessmentRepository
    {
        public RiskAssessmentRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. RISK ASSESSMENT IDENTIFICATION
        // =========================================================

        // Retrieves complete risk assessment details with its subject, audit users and fraud alerts.
        // Useful for investigating why a customer, account or transaction received a risk decision.
        public async Task<RiskAssessment?> GetRiskAssessmentDetailsAsync(int riskAssessmentId)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.Account)
                .Include(x => x.Transaction)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.FraudAlerts)
                .FirstOrDefaultAsync(x => x.RiskAssessmentId == riskAssessmentId);
        }

        // Retrieves assessments using their primary key.
        // Useful for accessing a specific risk evaluation record.
        public async Task<RiskAssessment?> GetByIdWithSubjectAsync(int riskAssessmentId)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.Account)
                .Include(x => x.Transaction)
                .FirstOrDefaultAsync(x => x.RiskAssessmentId == riskAssessmentId);
        }

        // =========================================================
        // 2. CUSTOMER ASSESSMENTS
        // =========================================================

        // Retrieves all risk assessments for a customer.
        // Useful for reviewing a customer's historical risk profile.
        public async Task<IEnumerable<RiskAssessment>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves the latest risk assessment for a customer.
        // Useful for checking the most recent recorded customer risk evaluation.
        public async Task<RiskAssessment?> GetLatestByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.AssessedAt)
                .ThenByDescending(x => x.RiskAssessmentId)
                .FirstOrDefaultAsync();
        }

        // Retrieves customer assessments within a date range.
        // Useful for reviewing changes in customer risk over time.
        public async Task<IEnumerable<RiskAssessment>> GetCustomerAssessmentsWithinRangeAsync(int customerId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId &&
                            x.AssessedAt >= startDate &&
                            x.AssessedAt <= endDate)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. ACCOUNT ASSESSMENTS
        // =========================================================

        // Retrieves all risk assessments for an account.
        // Useful for reviewing account-level risk history and monitoring.
        public async Task<IEnumerable<RiskAssessment>> GetByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves the latest risk assessment for an account.
        // Useful for checking the most recent recorded account risk evaluation.
        public async Task<RiskAssessment?> GetLatestByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.AssessedAt)
                .ThenByDescending(x => x.RiskAssessmentId)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // 4. TRANSACTION ASSESSMENTS
        // =========================================================

        // Retrieves all risk assessments for a transaction.
        // Useful for investigating transaction screening and risk decisions.
        public async Task<IEnumerable<RiskAssessment>> GetByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .Where(x => x.TransactionId == transactionId)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves the latest risk assessment for a transaction.
        // Useful for identifying the most recent risk decision associated with a transaction.
        public async Task<RiskAssessment?> GetLatestByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .Where(x => x.TransactionId == transactionId)
                .OrderByDescending(x => x.AssessedAt)
                .ThenByDescending(x => x.RiskAssessmentId)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // 5. ASSESSMENT TYPE AND RISK LEVEL
        // =========================================================

        // Retrieves assessments matching a particular subject type.
        // Useful for separating customer, account and transaction risk monitoring.
        public async Task<IEnumerable<RiskAssessment>> GetByAssessmentTypeAsync(string assessmentType)
        {
            return await _dbSet
                .Where(x => x.AssessmentType == assessmentType)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves assessments matching a particular risk level.
        // Useful for prioritizing high-risk and critical-risk cases for review.
        public async Task<IEnumerable<RiskAssessment>> GetByRiskLevelAsync(string riskLevel)
        {
            return await _dbSet
                .Where(x => x.RiskLevel == riskLevel)
                .OrderByDescending(x => x.RiskScore)
                .ThenByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves assessments matching a particular decision.
        // Useful for reviewing approved, review-required, challenged and declined cases.
        public async Task<IEnumerable<RiskAssessment>> GetByDecisionAsync(string decision)
        {
            return await _dbSet
                .Where(x => x.Decision == decision)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves high-risk and critical-risk assessments.
        // Useful for identifying cases that may require enhanced investigation.
        public async Task<IEnumerable<RiskAssessment>> GetHighAndCriticalRiskAssessmentsAsync()
        {
            return await _dbSet
                .Where(x => x.RiskLevel == "High" || x.RiskLevel == "Critical")
                .OrderByDescending(x => x.RiskScore)
                .ThenByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves assessments with a score at or above the supplied threshold.
        // Useful for identifying cases that meet a configured risk-review threshold.
        public async Task<IEnumerable<RiskAssessment>> GetByMinimumRiskScoreAsync(decimal minimumRiskScore)
        {
            return await _dbSet
                .Where(x => x.RiskScore >= minimumRiskScore)
                .OrderByDescending(x => x.RiskScore)
                .ToListAsync();
        }

        // =========================================================
        // 6. MODEL AND VALIDITY
        // =========================================================

        // Retrieves assessments produced by a particular model version.
        // Useful for comparing risk decisions across model releases.
        public async Task<IEnumerable<RiskAssessment>> GetByModelVersionAsync(string modelVersion)
        {
            return await _dbSet
                .Where(x => x.ModelVersion == modelVersion)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves assessments that have not expired as of the supplied date.
        // Useful for finding risk evaluations that are still within their recorded validity period.
        public async Task<IEnumerable<RiskAssessment>> GetNonExpiredAssessmentsAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt.Value > currentDateTime)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves assessments whose expiry date has passed.
        // Useful for identifying risk evaluations that may need to be refreshed.
        public async Task<IEnumerable<RiskAssessment>> GetExpiredAssessmentsAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.ExpiresAt.HasValue && x.ExpiresAt.Value <= currentDateTime)
                .OrderBy(x => x.ExpiresAt)
                .ToListAsync();
        }

        // =========================================================
        // 7. DATE AND AUDIT
        // =========================================================

        // Retrieves assessments completed within a date range.
        // Useful for daily and monthly risk assessment reporting.
        public async Task<IEnumerable<RiskAssessment>> GetAssessedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.AssessedAt >= startDate && x.AssessedAt <= endDate)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves assessments created by a particular user.
        // Useful for auditing who initiated or recorded risk assessments.
        public async Task<IEnumerable<RiskAssessment>> GetByCreatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves assessments last updated by a particular user.
        // Useful for tracking responsibility for changes to assessment records.
        public async Task<IEnumerable<RiskAssessment>> GetByUpdatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UpdatedBy == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 8. FRAUD ALERTS AND SEARCH
        // =========================================================

        // Retrieves assessments that have generated fraud alerts.
        // Useful for investigating risk evaluations associated with fraud monitoring.
        public async Task<IEnumerable<RiskAssessment>> GetAssessmentsWithFraudAlertsAsync()
        {
            return await _dbSet
                .Where(x => x.FraudAlerts.Any())
                .Include(x => x.FraudAlerts)
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // Retrieves one assessment with its associated fraud alerts.
        // Useful for reviewing the fraud alerts generated from a specific risk assessment.
        public async Task<RiskAssessment?> GetAssessmentWithFraudAlertsAsync(int riskAssessmentId)
        {
            return await _dbSet
                .Include(x => x.FraudAlerts)
                .FirstOrDefaultAsync(x => x.RiskAssessmentId == riskAssessmentId);
        }

        // Searches assessments by model version, assessment type or explanation factors.
        // Useful for locating risk evaluations during model and fraud investigations.
        public async Task<IEnumerable<RiskAssessment>> SearchRiskAssessmentsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.ModelVersion.Contains(searchTerm) ||
                            x.AssessmentType.Contains(searchTerm) ||
                            (x.FactorsJson != null && x.FactorsJson.Contains(searchTerm)))
                .OrderByDescending(x => x.AssessedAt)
                .ToListAsync();
        }

        // =========================================================
        // 9. REPORTING
        // =========================================================

        // Counts assessments matching a particular risk level within a date range.
        // Useful for reporting the volume of low, medium, high and critical risk cases.
        public async Task<int> CountByRiskLevelWithinRangeAsync(string riskLevel, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.CountAsync(x =>
                x.RiskLevel == riskLevel &&
                x.AssessedAt >= startDate &&
                x.AssessedAt <= endDate);
        }

        // Calculates the average risk score for a particular assessment type.
        // Useful for monitoring overall customer, account or transaction risk trends.
        public async Task<decimal> GetAverageRiskScoreByAssessmentTypeAsync(string assessmentType)
        {
            return await _dbSet
                .Where(x => x.AssessmentType == assessmentType)
                .Select(x => (decimal?)x.RiskScore)
                .AverageAsync() ?? 0m;
        }
    }
}
