
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IFraudRuleRepository : IGenericRepository<FraudRule>
    {
        // =========================================================
        // 1. RULE IDENTIFICATION
        // =========================================================

        // Retrieves a fraud rule using its unique rule code.
        // Useful for identifying a specific detection rule in the fraud engine.
        Task<FraudRule?> GetByRuleCodeAsync(string ruleCode);

        // Retrieves a rule with its creator, updater and generated fraud alerts.
        // Useful for reviewing the complete configuration and operational history of a rule.
        Task<FraudRule?> GetFraudRuleDetailsAsync(int fraudRuleId);

        // Checks whether a rule code already exists.
        // Useful for preventing duplicate fraud rule identifiers.
        Task<bool> ExistsByRuleCodeAsync(string ruleCode);

        // =========================================================
        // 2. RULE CLASSIFICATION
        // =========================================================

        // Retrieves rules matching a particular rule type.
        // Useful for managing velocity, amount, geographic, device and behavioural detection rules.
        Task<IEnumerable<FraudRule>> GetByRuleTypeAsync(string ruleType);

        // Retrieves rules matching a particular severity.
        // Useful for organizing fraud detection rules by risk importance.
        Task<IEnumerable<FraudRule>> GetBySeverityAsync(string severity);

        // Retrieves rules matching a particular action.
        // Useful for reviewing rules configured to alert, block, request step-up authentication or review.
        Task<IEnumerable<FraudRule>> GetByActionOnMatchAsync(string actionOnMatch);

        // =========================================================
        // 3. RULE ACTIVATION AND EFFECTIVE DATES
        // =========================================================

        // Retrieves all active fraud rules.
        // Useful for reviewing detection rules currently enabled in the system.
        Task<IEnumerable<FraudRule>> GetActiveRulesAsync();

        // Retrieves all inactive fraud rules.
        // Useful for reviewing disabled or retired detection rules.
        Task<IEnumerable<FraudRule>> GetInactiveRulesAsync();

        // Retrieves active rules effective on the supplied date.
        // Useful for selecting the rule configurations applicable during transaction evaluation.
        Task<IEnumerable<FraudRule>> GetEffectiveRulesAsync(DateTime effectiveDate);

        // Retrieves rules scheduled to become effective within a date range.
        // Useful for planning future fraud detection policy changes.
        Task<IEnumerable<FraudRule>> GetUpcomingRulesAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 4. PRIORITY AND THRESHOLD MANAGEMENT
        // =========================================================

        // Retrieves active rules ordered by their configured priority.
        // Useful for executing fraud checks in a predictable evaluation order.
        Task<IEnumerable<FraudRule>> GetActiveRulesByPriorityAsync();

        // Retrieves rules whose priority falls within the supplied range.
        // Useful for reviewing and managing fraud rule execution order.
        Task<IEnumerable<FraudRule>> GetRulesByPriorityRangeAsync(int minimumPriority, int maximumPriority);

        // Retrieves rules with a threshold amount equal to or above the supplied amount.
        // Useful for reviewing transaction-value-based fraud detection configurations.
        Task<IEnumerable<FraudRule>> GetRulesByMinimumThresholdAsync(decimal minimumAmount);

        // Retrieves rules configured with the supplied time window.
        // Useful for reviewing velocity detection rules that evaluate transaction frequency.
        Task<IEnumerable<FraudRule>> GetRulesByTimeWindowAsync(int timeWindowMinutes);

        // =========================================================
        // 5. VERSION AND AUDIT MANAGEMENT
        // =========================================================

        // Retrieves rules with a particular configuration version.
        // Useful for tracking changes to fraud detection policies.
        Task<IEnumerable<FraudRule>> GetRulesByVersionAsync(int version);

        // Retrieves rules created by a particular user.
        // Useful for auditing who introduced fraud detection configurations.
        Task<IEnumerable<FraudRule>> GetRulesCreatedByUserAsync(int userId);

        // Retrieves rules updated by a particular user.
        // Useful for tracking administrative changes to fraud detection configurations.
        Task<IEnumerable<FraudRule>> GetRulesUpdatedByUserAsync(int userId);

        // =========================================================
        // 6. ALERT HISTORY AND SEARCH
        // =========================================================

        // Retrieves rules that have generated at least one fraud alert.
        // Useful for identifying detection rules that have produced operational results.
        Task<IEnumerable<FraudRule>> GetRulesWithFraudAlertsAsync();

        // Retrieves rules that have not generated any fraud alerts.
        // Useful for reviewing unused rules and evaluating detection coverage.
        Task<IEnumerable<FraudRule>> GetRulesWithoutFraudAlertsAsync();

        // Searches fraud rules by rule code, name or description.
        // Useful for locating configurations in fraud administration screens.
        Task<IEnumerable<FraudRule>> SearchFraudRulesAsync(string searchTerm);

        // Counts fraud alerts generated by a particular rule.
        // Useful for measuring rule activity and supporting detection-performance reporting.
        Task<int> CountFraudAlertsByRuleIdAsync(int fraudRuleId);
    }
}
