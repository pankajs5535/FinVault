
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class FraudAlertRepository : GenericRepository<FraudAlert>, IFraudAlertRepository
    {
        public FraudAlertRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. ALERT IDENTIFICATION
        // =========================================================

        // Retrieves a fraud alert using its unique alert number.
        // Useful for tracking an alert during fraud investigation and customer support.
        public async Task<FraudAlert?> GetByAlertNumberAsync(string alertNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.AlertNumber == alertNumber);
        }

        // Retrieves complete alert details with related banking entities and review information.
        // Useful for investigating the alert with its transaction, customer, account and card context.
        public async Task<FraudAlert?> GetFraudAlertDetailsAsync(int fraudAlertId)
        {
            return await _dbSet
                .Include(x => x.FraudRule)
                .Include(x => x.Transaction)
                .Include(x => x.Customer)
                .Include(x => x.Account)
                .Include(x => x.Card)
                .Include(x => x.RiskAssessment)
                .Include(x => x.ReviewedByEmployee)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.ComplianceRecords)
                .FirstOrDefaultAsync(x => x.FraudAlertId == fraudAlertId);
        }

        // Checks whether an alert number already exists.
        // Useful for preventing duplicate fraud alert references.
        public async Task<bool> ExistsByAlertNumberAsync(string alertNumber)
        {
            return await _dbSet.AnyAsync(x => x.AlertNumber == alertNumber);
        }

        // =========================================================
        // 2. ALERT STATUS AND SEVERITY
        // =========================================================

        // Retrieves alerts matching a particular lifecycle status.
        // Useful for separating alerts awaiting review from confirmed or closed cases.
        public async Task<IEnumerable<FraudAlert>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves alerts matching a particular severity.
        // Useful for prioritizing critical and high-risk fraud investigations.
        public async Task<IEnumerable<FraudAlert>> GetBySeverityAsync(string severity)
        {
            return await _dbSet
                .Where(x => x.Severity == severity)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves alerts with a score equal to or above the supplied threshold.
        // Useful for identifying alerts that require closer investigation.
        public async Task<IEnumerable<FraudAlert>> GetAlertsAboveScoreAsync(decimal minimumScore)
        {
            return await _dbSet
                .Where(x => x.Score.HasValue && x.Score.Value >= minimumScore)
                .OrderByDescending(x => x.Score)
                .ToListAsync();
        }

        // Retrieves alerts that are currently open or under review.
        // Useful for monitoring the active fraud investigation queue.
        public async Task<IEnumerable<FraudAlert>> GetActiveFraudAlertsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Open" || x.Status == "UnderReview")
                .OrderByDescending(x => x.Severity == "Critical")
                .ThenByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. BANKING ENTITY ASSOCIATIONS
        // =========================================================

        // Retrieves fraud alerts associated with a particular customer.
        // Useful for reviewing customer-level fraud activity.
        public async Task<IEnumerable<FraudAlert>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves fraud alerts associated with a particular account.
        // Useful for investigating suspicious account activity.
        public async Task<IEnumerable<FraudAlert>> GetByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves fraud alerts associated with a particular transaction.
        // Useful for investigating suspicious or potentially unauthorized transactions.
        public async Task<IEnumerable<FraudAlert>> GetByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .Where(x => x.TransactionId == transactionId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves fraud alerts associated with a particular card.
        // Useful for reviewing suspicious ATM, POS and online card activity.
        public async Task<IEnumerable<FraudAlert>> GetByCardIdAsync(int cardId)
        {
            return await _dbSet
                .Where(x => x.CardId == cardId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves fraud alerts generated by a particular fraud rule.
        // Useful for evaluating the alerts produced by an individual detection rule.
        public async Task<IEnumerable<FraudAlert>> GetByFraudRuleIdAsync(int fraudRuleId)
        {
            return await _dbSet
                .Where(x => x.FraudRuleId == fraudRuleId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves alerts associated with a particular risk assessment.
        // Useful for reviewing fraud alerts generated from customer or account risk evaluations.
        public async Task<IEnumerable<FraudAlert>> GetByRiskAssessmentIdAsync(int riskAssessmentId)
        {
            return await _dbSet
                .Where(x => x.RiskAssessmentId == riskAssessmentId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. INVESTIGATION ASSIGNMENT
        // =========================================================

        // Retrieves alerts assigned to a particular reviewing employee.
        // Useful for monitoring an analyst's investigation workload.
        public async Task<IEnumerable<FraudAlert>> GetByReviewedByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(x => x.ReviewedByEmployeeId == employeeId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves alerts that have not yet been assigned to a reviewing employee.
        // Useful for distributing unassigned fraud cases among analysts.
        public async Task<IEnumerable<FraudAlert>> GetUnassignedFraudAlertsAsync()
        {
            return await _dbSet
                .Where(x => x.ReviewedByEmployeeId == null &&
                            (x.Status == "Open" || x.Status == "UnderReview"))
                .OrderByDescending(x => x.Severity == "Critical")
                .ThenBy(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. INVESTIGATION OUTCOMES
        // =========================================================

        // Retrieves alerts confirmed as fraud.
        // Useful for fraud-loss analysis and confirmed incident reporting.
        public async Task<IEnumerable<FraudAlert>> GetConfirmedFraudAlertsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "ConfirmedFraud")
                .OrderByDescending(x => x.ReviewedAt)
                .ToListAsync();
        }

        // Retrieves alerts classified as false positives.
        // Useful for evaluating detection accuracy and improving fraud rules.
        public async Task<IEnumerable<FraudAlert>> GetFalsePositiveAlertsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "FalsePositive")
                .OrderByDescending(x => x.ReviewedAt)
                .ToListAsync();
        }

        // Retrieves alerts that have associated compliance records.
        // Useful for tracking fraud cases that require compliance documentation.
        public async Task<IEnumerable<FraudAlert>> GetAlertsWithComplianceRecordsAsync()
        {
            return await _dbSet
                .Include(x => x.ComplianceRecords)
                .Where(x => x.ComplianceRecords.Any())
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 6. ALERT TIMELINE AND SEARCH
        // =========================================================

        // Retrieves alerts raised within the supplied date range.
        // Useful for fraud trend analysis and operational reporting.
        public async Task<IEnumerable<FraudAlert>> GetAlertsRaisedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.RaisedAt >= startDate && x.RaisedAt <= endDate)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves alerts reviewed within the supplied date range.
        // Useful for monitoring investigation and review activity.
        public async Task<IEnumerable<FraudAlert>> GetAlertsReviewedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ReviewedAt.HasValue &&
                            x.ReviewedAt.Value >= startDate &&
                            x.ReviewedAt.Value <= endDate)
                .OrderByDescending(x => x.ReviewedAt)
                .ToListAsync();
        }

        // Searches alerts by alert number, description or review notes.
        // Useful for authorized fraud analysts locating relevant cases.
        public async Task<IEnumerable<FraudAlert>> SearchFraudAlertsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.AlertNumber.Contains(searchTerm) ||
                            x.Description.Contains(searchTerm) ||
                            (x.ReviewNotes != null && x.ReviewNotes.Contains(searchTerm)))
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Counts alerts associated with a particular customer.
        // Useful for customer-level fraud monitoring and reporting.
        public async Task<int> CountAlertsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.CountAsync(x => x.CustomerId == customerId);
        }
    }
}
