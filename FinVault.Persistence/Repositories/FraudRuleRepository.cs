
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class FraudRuleRepository : GenericRepository<FraudRule>, IFraudRuleRepository
    {
        public FraudRuleRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. RULE IDENTIFICATION
        // =========================================================

        // Retrieves a fraud rule using its unique rule code.
        // Useful for identifying a specific detection rule in the fraud engine.
        public async Task<FraudRule?> GetByRuleCodeAsync(string ruleCode)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.RuleCode == ruleCode);
        }

        // Retrieves a rule with its creator, updater and generated fraud alerts.
        // Useful for reviewing the complete configuration and operational history of a rule.
        public async Task<FraudRule?> GetFraudRuleDetailsAsync(int fraudRuleId)
        {
            return await _dbSet
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.FraudAlerts)
                .FirstOrDefaultAsync(x => x.FraudRuleId == fraudRuleId);
        }

        // Checks whether a rule code already exists.
        // Useful for preventing duplicate fraud rule identifiers.
        public async Task<bool> ExistsByRuleCodeAsync(string ruleCode)
        {
            return await _dbSet.AnyAsync(x => x.RuleCode == ruleCode);
        }

        // =========================================================
        // 2. RULE CLASSIFICATION
        // =========================================================

        // Retrieves rules matching a particular rule type.
        // Useful for managing velocity, amount, geographic, device and behavioural detection rules.
        public async Task<IEnumerable<FraudRule>> GetByRuleTypeAsync(string ruleType)
        {
            return await _dbSet
                .Where(x => x.RuleType == ruleType)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Retrieves rules matching a particular severity.
        // Useful for organizing fraud detection rules by risk importance.
        public async Task<IEnumerable<FraudRule>> GetBySeverityAsync(string severity)
        {
            return await _dbSet
                .Where(x => x.Severity == severity)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Retrieves rules matching a particular action.
        // Useful for reviewing rules configured to alert, block, request step-up authentication or review.
        public async Task<IEnumerable<FraudRule>> GetByActionOnMatchAsync(string actionOnMatch)
        {
            return await _dbSet
                .Where(x => x.ActionOnMatch == actionOnMatch)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // =========================================================
        // 3. RULE ACTIVATION AND EFFECTIVE DATES
        // =========================================================

        // Retrieves all active fraud rules.
        // Useful for reviewing detection rules currently enabled in the system.
        public async Task<IEnumerable<FraudRule>> GetActiveRulesAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Retrieves all inactive fraud rules.
        // Useful for reviewing disabled or retired detection rules.
        public async Task<IEnumerable<FraudRule>> GetInactiveRulesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Retrieves active rules effective on the supplied date.
        // Useful for selecting the rule configurations applicable during transaction evaluation.
        public async Task<IEnumerable<FraudRule>> GetEffectiveRulesAsync(DateTime effectiveDate)
        {
            return await _dbSet
                .Where(x => x.IsActive &&
                            (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value <= effectiveDate) &&
                            (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= effectiveDate))
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Retrieves rules scheduled to become effective within a date range.
        // Useful for planning future fraud detection policy changes.
        public async Task<IEnumerable<FraudRule>> GetUpcomingRulesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.EffectiveFrom.HasValue &&
                            x.EffectiveFrom.Value >= startDate &&
                            x.EffectiveFrom.Value <= endDate)
                .OrderBy(x => x.EffectiveFrom)
                .ToListAsync();
        }

        // =========================================================
        // 4. PRIORITY AND THRESHOLD MANAGEMENT
        // =========================================================

        // Retrieves active rules ordered by their configured priority.
        // Useful for executing fraud checks in a predictable evaluation order.
        public async Task<IEnumerable<FraudRule>> GetActiveRulesByPriorityAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.FraudRuleId)
                .ToListAsync();
        }

        // Retrieves rules whose priority falls within the supplied range.
        // Useful for reviewing and managing fraud rule execution order.
        public async Task<IEnumerable<FraudRule>> GetRulesByPriorityRangeAsync(int minimumPriority, int maximumPriority)
        {
            return await _dbSet
                .Where(x => x.Priority >= minimumPriority && x.Priority <= maximumPriority)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Retrieves rules with a threshold amount equal to or above the supplied amount.
        // Useful for reviewing transaction-value-based fraud detection configurations.
        public async Task<IEnumerable<FraudRule>> GetRulesByMinimumThresholdAsync(decimal minimumAmount)
        {
            return await _dbSet
                .Where(x => x.ThresholdAmount.HasValue && x.ThresholdAmount.Value >= minimumAmount)
                .OrderBy(x => x.ThresholdAmount)
                .ToListAsync();
        }

        // Retrieves rules configured with the supplied time window.
        // Useful for reviewing velocity detection rules that evaluate transaction frequency.
        public async Task<IEnumerable<FraudRule>> GetRulesByTimeWindowAsync(int timeWindowMinutes)
        {
            return await _dbSet
                .Where(x => x.TimeWindowMinutes == timeWindowMinutes)
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // =========================================================
        // 5. VERSION AND AUDIT MANAGEMENT
        // =========================================================

        // Retrieves rules with a particular configuration version.
        // Useful for tracking changes to fraud detection policies.
        public async Task<IEnumerable<FraudRule>> GetRulesByVersionAsync(int version)
        {
            return await _dbSet
                .Where(x => x.Version == version)
                .OrderBy(x => x.RuleCode)
                .ToListAsync();
        }

        // Retrieves rules created by a particular user.
        // Useful for auditing who introduced fraud detection configurations.
        public async Task<IEnumerable<FraudRule>> GetRulesCreatedByUserAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves rules updated by a particular user.
        // Useful for tracking administrative changes to fraud detection configurations.
        public async Task<IEnumerable<FraudRule>> GetRulesUpdatedByUserAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UpdatedBy == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 6. ALERT HISTORY AND SEARCH
        // =========================================================

        // Retrieves rules that have generated at least one fraud alert.
        // Useful for identifying detection rules that have produced operational results.
        public async Task<IEnumerable<FraudRule>> GetRulesWithFraudAlertsAsync()
        {
            return await _dbSet
                .Include(x => x.FraudAlerts)
                .Where(x => x.FraudAlerts.Any())
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Retrieves rules that have not generated any fraud alerts.
        // Useful for reviewing unused rules and evaluating detection coverage.
        public async Task<IEnumerable<FraudRule>> GetRulesWithoutFraudAlertsAsync()
        {
            return await _dbSet
                .Where(x => !x.FraudAlerts.Any())
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Searches fraud rules by rule code, name or description.
        // Useful for locating configurations in fraud administration screens.
        public async Task<IEnumerable<FraudRule>> SearchFraudRulesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.RuleCode.Contains(searchTerm) ||
                            x.Name.Contains(searchTerm) ||
                            (x.Description != null && x.Description.Contains(searchTerm)))
                .OrderBy(x => x.Priority)
                .ToListAsync();
        }

        // Counts fraud alerts generated by a particular rule.
        // Useful for measuring rule activity and supporting detection-performance reporting.
        public async Task<int> CountFraudAlertsByRuleIdAsync(int fraudRuleId)
        {
            return await _dbSet
                .Where(x => x.FraudRuleId == fraudRuleId)
                .SelectMany(x => x.FraudAlerts)
                .CountAsync();
        }
    }
}
