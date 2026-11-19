using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Append-only security and data-change audit trail. Old/new values are stored masked so
sensitive data (PAN, Aadhaar, passwords, card data) never appears. Optional hash
chaining makes tampering detectable.

RELATIONSHIP
- Optionally belongs to User through UserId.
*/

public class AuditLog
{
    // =========================
    // Primary Key
    // =========================

    public int AuditLogId { get; set; }

    // =========================
    // Event
    // =========================

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public int? EntityId { get; set; }

    // Allowed values: Success | Failure
    public string Outcome { get; set; } = "Success";

    public bool IsSecuritySensitive { get; set; }

    // =========================
    // Change Data (masked)
    // =========================

    public string? OldValuesMasked { get; set; }

    public string? NewValuesMasked { get; set; }

    // =========================
    // Request Context
    // =========================

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? CorrelationId { get; set; }

    // =========================
    // Tamper Evidence
    // =========================

    public string? PreviousHash { get; set; }

    public string? EntryHash { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? UserId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual User? User { get; set; }
}
