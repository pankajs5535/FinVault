using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
AI assistant conversation session for a user, optionally on behalf of a customer, with
status and escalation to a human employee.

RELATIONSHIP
- Belongs to User through UserId.
- Optionally belongs to Customer through CustomerId.
- Optionally belongs to Employee through EscalatedToEmployeeId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by AIChatMessage through AIConversationId.
- Referenced by AIUsageLog through AIConversationId.
*/

public class AIConversation
{
    // =========================
    // Primary Key
    // =========================

    public int AIConversationId { get; set; }

    // =========================
    // Conversation
    // =========================

    public string ConversationReference { get; set; } = null!;

    public string? Title { get; set; }

    // Allowed values: Web | Mobile | Voice
    public string Channel { get; set; } = "Web";

    public string Language { get; set; } = "en";

    public string? Summary { get; set; }

    // =========================
    // Lifecycle
    // =========================

    // Allowed values: Active | Closed | Archived | Escalated
    public string Status { get; set; } = "Active";

    public DateTime StartedAt { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public int MessageCount { get; set; } = 0;

    // =========================
    // Foreign Keys
    // =========================

    public int UserId { get; set; }

    public int? CustomerId { get; set; }

    public int? EscalatedToEmployeeId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual User User { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual Employee? EscalatedToEmployee { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<AIChatMessage> AIChatMessages { get; set; }
        = new List<AIChatMessage>();

    public virtual ICollection<AIUsageLog> AIUsageLogs { get; set; }
        = new List<AIUsageLog>();
}
