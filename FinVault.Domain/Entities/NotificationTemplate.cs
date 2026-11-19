using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Reusable message template per channel and language (SMS, email, push, in-app, WhatsApp).
SMS templates carry the TRAI DLT template registration id required in India.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Notification through NotificationTemplateId.
*/

public class NotificationTemplate
{
    // =========================
    // Primary Key
    // =========================

    public int NotificationTemplateId { get; set; }

    // =========================
    // Template Information
    // =========================

    public string TemplateCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    // Allowed values: Sms | Email | Push | InApp | WhatsApp
    public string Channel { get; set; } = "Sms";

    public string Language { get; set; } = "en";

    // Allowed values: Transactional | Security | Promotional | Regulatory | Reminder
    public string Category { get; set; } = "Transactional";

    // =========================
    // Content
    // =========================

    public string? Subject { get; set; }

    // Template body with {{placeholders}}.
    public string BodyTemplate { get; set; } = null!;

    public string? DltTemplateId { get; set; }

    // =========================
    // Status
    // =========================

    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;

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

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; }
        = new List<Notification>();
}
