using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.NotificationDtos;

/// <summary>Input for creating a Notification.</summary>
public class CreateNotificationDto
{
    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Sms|Email|Push|InApp|WhatsApp)$", ErrorMessage = "Channel must be one of: Sms, Email, Push, InApp, WhatsApp.")]
    public string Channel { get; set; } = "Sms";

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Low|Normal|High|Urgent)$", ErrorMessage = "Priority must be one of: Low, Normal, High, Urgent.")]
    public string Priority { get; set; } = "Normal";

    [Required]
    [StringLength(256)]
    public string RecipientAddress { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Subject { get; set; }

    [Required]
    public string Body { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Pending|Queued|Sent|Delivered|Failed|Read)$", ErrorMessage = "Status must be one of: Pending, Queued, Sent, Delivered, Failed, Read.")]
    public string Status { get; set; } = "Pending";

    public DateTime? ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }

    [Range(0, int.MaxValue)]
    public int RetryCount { get; set; } = 0;

    [StringLength(100)]
    public string? ProviderMessageId { get; set; }

    [StringLength(500)]
    public string? FailureReason { get; set; }

    [StringLength(50)]
    public string? RelatedEntityType { get; set; }

    [Range(0, int.MaxValue)]
    public int? RelatedEntityId { get; set; }

    [Range(1, int.MaxValue)]
    public int? CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int? NotificationTemplateId { get; set; }
}
