using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Single message within an AIConversation. Content must be PII-masked before storage.
Deleted together with its conversation (data-minimisation purge).

RELATIONSHIP
- Belongs to AIConversation through AIConversationId.
- Referenced by AIUsageLog through AIChatMessageId.
*/

public class AIChatMessage
{
    // =========================
    // Primary Key
    // =========================

    public int AIChatMessageId { get; set; }

    // =========================
    // Message
    // =========================

    public int SequenceNumber { get; set; }

    // Allowed values: System | User | Assistant | Tool
    public string SenderRole { get; set; } = "User";

    // Masked message text.
    public string Content { get; set; } = null!;

    // =========================
    // Model Metrics
    // =========================

    public string? ModelName { get; set; }

    public int? InputTokens { get; set; }

    public int? OutputTokens { get; set; }

    public int? LatencyMs { get; set; }

    public string? FinishReason { get; set; }

    // =========================
    // Moderation
    // =========================

    public bool IsFlagged { get; set; }

    public string? FlagReason { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int AIConversationId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual AIConversation AIConversation { get; set; } = null!;

    public virtual ICollection<AIUsageLog> AIUsageLogs { get; set; }
        = new List<AIUsageLog>();
}
