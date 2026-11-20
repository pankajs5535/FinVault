using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FraudRuleDtos;

/// <summary>Input for creating a FraudRule.</summary>
public class CreateFraudRuleDto
{
    [Required]
    [StringLength(30)]
    public string RuleCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Velocity|Amount|Geo|Device|Behavioural|Blacklist|Pattern)$", ErrorMessage = "RuleType must be one of: Velocity, Amount, Geo, Device, Behavioural, Blacklist, Pattern.")]
    public string RuleType { get; set; } = "Velocity";

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Low|Medium|High|Critical)$", ErrorMessage = "Severity must be one of: Low, Medium, High, Critical.")]
    public string Severity { get; set; } = "Medium";

    [Required]
    public string RuleDefinitionJson { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal? ThresholdAmount { get; set; }

    [Range(0, int.MaxValue)]
    public int? TimeWindowMinutes { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Alert|Block|StepUpAuth|Review)$", ErrorMessage = "ActionOnMatch must be one of: Alert, Block, StepUpAuth, Review.")]
    public string ActionOnMatch { get; set; } = "Alert";

    [Range(0, int.MaxValue)]
    public int Priority { get; set; } = 100;

    [Range(0, int.MaxValue)]
    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
