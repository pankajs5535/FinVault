using FinVault.Application.DTOs.FraudRuleDtos;
using System.Data;

namespace FinVault.Application.Interfaces.Services
{
    public interface IFraudRuleService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<FraudRuleDto>> GetAllAsync();
        Task<FraudRuleDto?> GetByIdAsync(int fraudRuleId);
        Task<FraudRuleDto?> GetByRuleCodeAsync(string ruleCode);
        Task<FraudRuleDto?> GetFraudRuleDetailsAsync(int fraudRuleId);
        Task<bool> ExistsByRuleCodeAsync(string ruleCode);
        Task<FraudRuleDto> CreateAsync(CreateFraudRuleDto dto);
        Task<FraudRuleDto?> UpdateAsync(UpdateFraudRuleDto dto);
        Task<bool> DeleteAsync(DeleteFraudRuleDto dto);

        // 2. RULE CLASSIFICATION
        Task<IEnumerable<FraudRuleDto>> GetByRuleTypeAsync(string ruleType);
        Task<IEnumerable<FraudRuleDto>> GetBySeverityAsync(string severity);
        Task<IEnumerable<FraudRuleDto>> GetByActionOnMatchAsync(string actionOnMatch);

        // 3. ACTIVATION AND EFFECTIVE DATES
        Task<IEnumerable<FraudRuleDto>> GetActiveRulesAsync();
        Task<IEnumerable<FraudRuleDto>> GetInactiveRulesAsync();
        Task<IEnumerable<FraudRuleDto>> GetEffectiveRulesAsync(DateTime effectiveDate);
        Task<IEnumerable<FraudRuleDto>> GetUpcomingRulesAsync(DateTime startDate, DateTime endDate);

        // 4. PRIORITY AND THRESHOLD MANAGEMENT
        Task<IEnumerable<FraudRuleDto>> GetActiveRulesByPriorityAsync();
        Task<IEnumerable<FraudRuleDto>> GetRulesByPriorityRangeAsync(int minimumPriority, int maximumPriority);
        Task<IEnumerable<FraudRuleDto>> GetRulesByMinimumThresholdAsync(decimal minimumAmount);
        Task<IEnumerable<FraudRuleDto>> GetRulesByTimeWindowAsync(int timeWindowMinutes);

        // 5. VERSION AND AUDIT MANAGEMENT
        Task<IEnumerable<FraudRuleDto>> GetRulesByVersionAsync(int version);
        Task<IEnumerable<FraudRuleDto>> GetRulesCreatedByUserAsync(int userId);
        Task<IEnumerable<FraudRuleDto>> GetRulesUpdatedByUserAsync(int userId);

        // 6. ALERT HISTORY AND SEARCH
        Task<IEnumerable<FraudRuleDto>> GetRulesWithFraudAlertsAsync();
        Task<IEnumerable<FraudRuleDto>> GetRulesWithoutFraudAlertsAsync();
        Task<IEnumerable<FraudRuleDto>> SearchFraudRulesAsync(string searchTerm);
        Task<int> CountFraudAlertsByRuleIdAsync(int fraudRuleId);
    }
}


//Banking rules:
//-Rule codes must be unique.
//- Validate RuleDefinitionJson before activating a rule.
//- Only active and effective rules should be evaluated by the fraud engine.
//- Preserve rule versions and audit who changed each configuration.
//- Restrict rule creation and modification to authorized fraud administrators.