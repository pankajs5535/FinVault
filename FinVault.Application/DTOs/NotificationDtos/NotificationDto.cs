using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.NotificationDtos;

/// <summary>Returned to clients for Get All and Get By Id of Notification.</summary>
public class NotificationDto
{
    public int NotificationId { get; set; }

    public string Channel { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string RecipientAddress { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public string Body { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime? ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public int RetryCount { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? FailureReason { get; set; }

    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityId { get; set; }

    public int? CustomerId { get; set; }

    public int? UserId { get; set; }

    public int? NotificationTemplateId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
