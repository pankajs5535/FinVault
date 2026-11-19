using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Per-request record of AI usage (tokens, latency, estimated cost, success) for
monitoring, quotas and cost allocation.

RELATIONSHIP
- Optionally belongs to User through UserId.
- Optionally belongs to AIConversation through AIConversationId.
- Optionally belongs to AIChatMessage through AIChatMessageId.
*/

public class AIUsageLog
{
    // =========================
    // Primary Key
    // =========================

    public int AIUsageLogId { get; set; }

    // =========================
    // Request
    // =========================

    public string FeatureName { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public string ModelName { get; set; } = null!;

    public string? RequestId { get; set; }

    public bool IsSuccess { get; set; } = true;

    public string? ErrorCode { get; set; }

    public int LatencyMs { get; set; }

    // =========================
    // Usage
    // =========================

    public int InputTokens { get; set; }

    public int OutputTokens { get; set; }

    public int TotalTokens { get; set; }

    public decimal EstimatedCostInr { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? UserId { get; set; }

    public int? AIConversationId { get; set; }

    public int? AIChatMessageId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual User? User { get; set; }

    public virtual AIConversation? AIConversation { get; set; }

    public virtual AIChatMessage? AIChatMessage { get; set; }
}
