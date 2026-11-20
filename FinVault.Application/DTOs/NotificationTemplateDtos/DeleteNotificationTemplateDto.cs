using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.NotificationTemplateDtos;

/// <summary>Input for deleting a NotificationTemplate.</summary>
public class DeleteNotificationTemplateDto
{
    [Range(1, int.MaxValue)]
    public int NotificationTemplateId { get; set; }
}
