using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIUsageLogDtos;

/// <summary>Returned to clients for Get All and Get By Id of AIUsageLog.</summary>
public class AIUsageLogDto
{
    public int AIUsageLogId { get; set; }

    public string FeatureName { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public string ModelName { get; set; } = string.Empty;

    public string? RequestId { get; set; }

    public bool IsSuccess { get; set; }

    public string? ErrorCode { get; set; }

    public int LatencyMs { get; set; }

    public int InputTokens { get; set; }

    public int OutputTokens { get; set; }

    public int TotalTokens { get; set; }

    public decimal EstimatedCostInr { get; set; }

    public int? UserId { get; set; }

    public int? AIConversationId { get; set; }

    public int? AIChatMessageId { get; set; }

    public DateTime CreatedAt { get; set; }
}
