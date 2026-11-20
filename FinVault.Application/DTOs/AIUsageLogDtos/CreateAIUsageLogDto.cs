using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIUsageLogDtos;

/// <summary>Input for creating an AIUsageLog.</summary>
public class CreateAIUsageLogDto
{
    [Required]
    [StringLength(100)]
    public string FeatureName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ModelName { get; set; } = string.Empty;

    [StringLength(64)]
    public string? RequestId { get; set; }

    public bool IsSuccess { get; set; } = true;

    [StringLength(50)]
    public string? ErrorCode { get; set; }

    [Range(0, int.MaxValue)]
    public int LatencyMs { get; set; }

    [Range(0, int.MaxValue)]
    public int InputTokens { get; set; }

    [Range(0, int.MaxValue)]
    public int OutputTokens { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalTokens { get; set; }

    [Range(0, double.MaxValue)]
    public decimal EstimatedCostInr { get; set; }

    [Range(1, int.MaxValue)]
    public int? UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AIConversationId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AIChatMessageId { get; set; }
}
