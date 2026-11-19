using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Message sent (or queued) to a customer or user: transaction alerts, OTP-free security
notices, due-date reminders and regulatory communications.

RELATIONSHIP
- Optionally belongs to Customer through CustomerId.
- Optionally belongs to User through UserId.
- Optionally belongs to NotificationTemplate through NotificationTemplateId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class Notification
{
    // =========================
    // Primary Key
    // =========================

    public int NotificationId { get; set; }

    // =========================
    // Message
    // =========================

    // Allowed values: Sms | Email | Push | InApp | WhatsApp
    public string Channel { get; set; } = "Sms";

    // Allowed values: Low | Normal | High | Urgent
    public string Priority { get; set; } = "Normal";

    // Email address or mobile number.
    public string RecipientAddress { get; set; } = null!;

    public string? Subject { get; set; }

    // Rendered message. Must not contain OTPs, full account numbers or other secrets.
    public string Body { get; set; } = null!;

    // =========================
    // Delivery
    // =========================

    // Allowed values: Pending | Queued | Sent | Delivered | Failed | Read
    public string Status { get; set; } = "Pending";

    public DateTime? ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public int RetryCount { get; set; } = 0;

    public string? ProviderMessageId { get; set; }

    public string? FailureReason { get; set; }

    // =========================
    // Related Record (soft reference)
    // =========================

    // Name of the related entity, e.g. Transaction. Not a database foreign key.
    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityId { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int? CustomerId { get; set; }

    public int? UserId { get; set; }

    public int? NotificationTemplateId { get; set; }

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

    public virtual Customer? Customer { get; set; }

    public virtual User? User { get; set; }

    public virtual NotificationTemplate? NotificationTemplate { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
