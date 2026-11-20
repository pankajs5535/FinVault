using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.NotificationDtos;

/// <summary>Input for deleting a Notification.</summary>
public class DeleteNotificationDto
{
    [Range(1, int.MaxValue)]
    public int NotificationId { get; set; }
}
