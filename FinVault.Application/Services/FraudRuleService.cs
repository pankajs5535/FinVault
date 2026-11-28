
using AutoMapper;
using FinVault.Application.DTOs.FraudRuleDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;
using System.Text.Json;

namespace FinVault.Application.Services;

public class FraudRuleService : IFraudRuleService
{
    private static readonly string[] ValidRuleTypes =
    {
        "Velocity", "Amount", "Geo", "Device",
        "Behavioural", "Blacklist", "Pattern"
    };

    private static readonly string[] ValidSeverities =
    {
        "Low", "Medium", "High", "Critical"
    };

    private static readonly string[] ValidActions =
    {
        "Alert", "Block", "StepUpAuth", "Review"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FraudRuleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<FraudRuleDto>> GetAllAsync()
    {
        var rules = await _unitOfWork.FraudRule.GetAllAsync();
        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<FraudRuleDto?> GetByIdAsync(int fraudRuleId)
    {
        ValidateId(fraudRuleId, nameof(fraudRuleId));

        var rule = await _unitOfWork.FraudRule.GetByIdAsync(fraudRuleId);
        return _mapper.Map<FraudRuleDto?>(rule);
    }

    public async Task<FraudRuleDto?> GetByRuleCodeAsync(string ruleCode)
    {
        ValidateRequiredString(ruleCode, nameof(ruleCode), 30);

        var rule = await _unitOfWork.FraudRule
            .GetByRuleCodeAsync(ruleCode.Trim());

        return _mapper.Map<FraudRuleDto?>(rule);
    }

    public async Task<FraudRuleDto?> GetFraudRuleDetailsAsync(int fraudRuleId)
    {
        ValidateId(fraudRuleId, nameof(fraudRuleId));

        var rule = await _unitOfWork.FraudRule
            .GetFraudRuleDetailsAsync(fraudRuleId);

        return _mapper.Map<FraudRuleDto?>(rule);
    }

    public async Task<bool> ExistsByRuleCodeAsync(string ruleCode)
    {
        ValidateRequiredString(ruleCode, nameof(ruleCode), 30);

        return await _unitOfWork.FraudRule
            .ExistsByRuleCodeAsync(ruleCode.Trim());
    }

    public async Task<FraudRuleDto> CreateAsync(CreateFraudRuleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRule(
            dto.RuleCode,
            dto.Name,
            dto.RuleType,
            dto.Severity,
            dto.RuleDefinitionJson,
            dto.ActionOnMatch,
            dto.ThresholdAmount,
            dto.TimeWindowMinutes,
            dto.Priority,
            dto.Version,
            dto.EffectiveFrom,
            dto.EffectiveTo);

        var ruleCode = dto.RuleCode.Trim();

        if (await _unitOfWork.FraudRule.ExistsByRuleCodeAsync(ruleCode))
            throw new InvalidOperationException(
                "A fraud rule with this rule code already exists.");

        if (dto.IsActive)
            ValidateRuleDefinitionJson(dto.RuleDefinitionJson);

        var rule = _mapper.Map<FraudRule>(dto);

        rule.RuleCode = ruleCode;
        rule.Name = dto.Name.Trim();
        rule.RuleType = dto.RuleType.Trim();
        rule.Severity = dto.Severity.Trim();
        rule.ActionOnMatch = dto.ActionOnMatch.Trim();
        rule.RuleDefinitionJson = dto.RuleDefinitionJson;
        rule.Description = dto.Description?.Trim();
        rule.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.FraudRule.AddAsync(rule);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FraudRuleDto>(rule);
    }

    public async Task<FraudRuleDto?> UpdateAsync(UpdateFraudRuleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.FraudRuleId, nameof(dto.FraudRuleId));

        ValidateRule(
            dto.RuleCode,
            dto.Name,
            dto.RuleType,
            dto.Severity,
            dto.RuleDefinitionJson,
            dto.ActionOnMatch,
            dto.ThresholdAmount,
            dto.TimeWindowMinutes,
            dto.Priority,
            dto.Version,
            dto.EffectiveFrom,
            dto.EffectiveTo);

        var rule = await _unitOfWork.FraudRule
            .GetByIdAsync(dto.FraudRuleId);

        if (rule == null)
            return null;

        var ruleCode = dto.RuleCode.Trim();

        if (await _unitOfWork.FraudRule.ExistsByRuleCodeAsync(ruleCode) &&
            !string.Equals(rule.RuleCode, ruleCode, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "A fraud rule with this rule code already exists.");
        }

        if (dto.IsActive)
            ValidateRuleDefinitionJson(dto.RuleDefinitionJson);

        bool configurationChanged =
            rule.RuleType != dto.RuleType ||
            rule.RuleDefinitionJson != dto.RuleDefinitionJson ||
            rule.ThresholdAmount != dto.ThresholdAmount ||
            rule.TimeWindowMinutes != dto.TimeWindowMinutes ||
            rule.ActionOnMatch != dto.ActionOnMatch ||
            rule.Severity != dto.Severity ||
            rule.Priority != dto.Priority;

        if (dto.Version < rule.Version)
            throw new InvalidOperationException(
                "Rule version cannot be decreased.");

        if (configurationChanged && dto.Version <= rule.Version)
            throw new InvalidOperationException(
                "A configuration change requires a higher rule version.");

        _mapper.Map(dto, rule);

        rule.RuleCode = ruleCode;
        rule.Name = dto.Name.Trim();
        rule.RuleType = dto.RuleType.Trim();
        rule.Severity = dto.Severity.Trim();
        rule.ActionOnMatch = dto.ActionOnMatch.Trim();
        rule.RuleDefinitionJson = dto.RuleDefinitionJson;
        rule.Description = dto.Description?.Trim();
        rule.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.FraudRule.Update(rule);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FraudRuleDto>(rule);
    }

    public async Task<bool> DeleteAsync(DeleteFraudRuleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.FraudRuleId, nameof(dto.FraudRuleId));

        var rule = await _unitOfWork.FraudRule
            .GetByIdAsync(dto.FraudRuleId);

        if (rule == null)
            return false;

        if (rule.FraudAlerts.Any())
            throw new InvalidOperationException(
                "Fraud rules associated with fraud alerts cannot be deleted. Deactivate the rule instead.");

        _unitOfWork.FraudRule.Delete(rule);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. RULE CLASSIFICATION

    public async Task<IEnumerable<FraudRuleDto>> GetByRuleTypeAsync(string ruleType)
    {
        ValidateRuleType(ruleType);

        var rules = await _unitOfWork.FraudRule
            .GetByRuleTypeAsync(ruleType.Trim());

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetBySeverityAsync(string severity)
    {
        ValidateSeverity(severity);

        var rules = await _unitOfWork.FraudRule
            .GetBySeverityAsync(severity.Trim());

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetByActionOnMatchAsync(string actionOnMatch)
    {
        ValidateAction(actionOnMatch);

        var rules = await _unitOfWork.FraudRule
            .GetByActionOnMatchAsync(actionOnMatch.Trim());

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    // 3. ACTIVATION AND EFFECTIVE DATES

    public async Task<IEnumerable<FraudRuleDto>> GetActiveRulesAsync()
    {
        var rules = await _unitOfWork.FraudRule.GetActiveRulesAsync();
        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetInactiveRulesAsync()
    {
        var rules = await _unitOfWork.FraudRule.GetInactiveRulesAsync();
        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetEffectiveRulesAsync(DateTime effectiveDate)
    {
        ValidateDate(effectiveDate, nameof(effectiveDate));

        var rules = await _unitOfWork.FraudRule
            .GetEffectiveRulesAsync(effectiveDate);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetUpcomingRulesAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var rules = await _unitOfWork.FraudRule
            .GetUpcomingRulesAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    // 4. PRIORITY AND THRESHOLD MANAGEMENT

    public async Task<IEnumerable<FraudRuleDto>> GetActiveRulesByPriorityAsync()
    {
        var rules = await _unitOfWork.FraudRule
            .GetActiveRulesByPriorityAsync();

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetRulesByPriorityRangeAsync(
        int minimumPriority, int maximumPriority)
    {
        if (minimumPriority < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumPriority));

        if (maximumPriority < minimumPriority)
            throw new ArgumentException(
                "Maximum priority cannot be less than minimum priority.");

        var rules = await _unitOfWork.FraudRule
            .GetRulesByPriorityRangeAsync(minimumPriority, maximumPriority);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetRulesByMinimumThresholdAsync(
        decimal minimumAmount)
    {
        if (minimumAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumAmount));

        var rules = await _unitOfWork.FraudRule
            .GetRulesByMinimumThresholdAsync(minimumAmount);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetRulesByTimeWindowAsync(
        int timeWindowMinutes)
    {
        if (timeWindowMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeWindowMinutes));

        var rules = await _unitOfWork.FraudRule
            .GetRulesByTimeWindowAsync(timeWindowMinutes);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    // 5. VERSION AND AUDIT MANAGEMENT

    public async Task<IEnumerable<FraudRuleDto>> GetRulesByVersionAsync(int version)
    {
        if (version <= 0)
            throw new ArgumentOutOfRangeException(nameof(version));

        var rules = await _unitOfWork.FraudRule
            .GetRulesByVersionAsync(version);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetRulesCreatedByUserAsync(int userId)
    {
        ValidateId(userId, nameof(userId));

        var rules = await _unitOfWork.FraudRule
            .GetRulesCreatedByUserAsync(userId);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetRulesUpdatedByUserAsync(int userId)
    {
        ValidateId(userId, nameof(userId));

        var rules = await _unitOfWork.FraudRule
            .GetRulesUpdatedByUserAsync(userId);

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    // 6. ALERT HISTORY AND SEARCH

    public async Task<IEnumerable<FraudRuleDto>> GetRulesWithFraudAlertsAsync()
    {
        var rules = await _unitOfWork.FraudRule
            .GetRulesWithFraudAlertsAsync();

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> GetRulesWithoutFraudAlertsAsync()
    {
        var rules = await _unitOfWork.FraudRule
            .GetRulesWithoutFraudAlertsAsync();

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<IEnumerable<FraudRuleDto>> SearchFraudRulesAsync(string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm), 200);

        var rules = await _unitOfWork.FraudRule
            .SearchFraudRulesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<FraudRuleDto>>(rules);
    }

    public async Task<int> CountFraudAlertsByRuleIdAsync(int fraudRuleId)
    {
        ValidateId(fraudRuleId, nameof(fraudRuleId));

        return await _unitOfWork.FraudRule
            .CountFraudAlertsByRuleIdAsync(fraudRuleId);
    }

    // VALIDATION HELPERS

    private static void ValidateRule(
        string ruleCode,
        string name,
        string ruleType,
        string severity,
        string ruleDefinitionJson,
        string actionOnMatch,
        decimal? thresholdAmount,
        int? timeWindowMinutes,
        int priority,
        int version,
        DateTime? effectiveFrom,
        DateTime? effectiveTo)
    {
        ValidateRequiredString(ruleCode, nameof(ruleCode), 30);
        ValidateRequiredString(name, nameof(name), 100);
        ValidateRuleType(ruleType);
        ValidateSeverity(severity);
        ValidateAction(actionOnMatch);
        ValidateRequiredString(ruleDefinitionJson, nameof(ruleDefinitionJson), 10000);

        if (thresholdAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(thresholdAmount));

        if (timeWindowMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeWindowMinutes));

        if (priority < 0)
            throw new ArgumentOutOfRangeException(nameof(priority));

        if (version <= 0)
            throw new ArgumentOutOfRangeException(nameof(version));

        if (effectiveFrom.HasValue && effectiveTo.HasValue &&
            effectiveTo.Value < effectiveFrom.Value)
        {
            throw new ArgumentException(
                "Effective end date cannot be earlier than effective start date.");
        }
    }

    private static void ValidateRuleDefinitionJson(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw new ArgumentException(
                    "Rule definition JSON must contain a JSON object.",
                    nameof(json));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException(
                "Rule definition contains invalid JSON.", nameof(json), ex);
        }
    }

    private static void ValidateRuleType(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!ValidRuleTypes.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid fraud rule type.", nameof(value));
    }

    private static void ValidateSeverity(string value)
    {
        ValidateRequiredString(value, nameof(value), 10);

        if (!ValidSeverities.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid fraud rule severity.", nameof(value));
    }

    private static void ValidateAction(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!ValidActions.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid fraud rule action.", nameof(value));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName, "ID must be greater than zero.");
    }

    private static void ValidateRequiredString(
        string? value, string parameterName, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", parameterName);

        if (value.Trim().Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException("A valid date is required.", parameterName);
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }
}
