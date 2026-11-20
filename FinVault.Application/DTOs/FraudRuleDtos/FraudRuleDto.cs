using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FraudRuleDtos;

/// <summary>Returned to clients for Get All and Get By Id of FraudRule.</summary>
public class FraudRuleDto
{
    public int FraudRuleId { get; set; }

    public string RuleCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string RuleType { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string RuleDefinitionJson { get; set; } = string.Empty;

    public decimal? ThresholdAmount { get; set; }

    public int? TimeWindowMinutes { get; set; }

    public string ActionOnMatch { get; set; } = string.Empty;

    public int Priority { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
