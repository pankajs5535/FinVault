using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.NotificationDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Notification banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface INotificationService
{
        Task<IReadOnlyList<NotificationDto>> GetForCustomerAsync(int customerId);
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> SendAsync(int notificationId);
}
