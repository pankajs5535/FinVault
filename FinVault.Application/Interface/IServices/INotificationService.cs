using FinVault.Application.DTOs.NotificationDtos;
using FinVault.Application.DTOs.NotificationTemplateDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface INotificationService
    {
        // 1. NOTIFICATION CRUD
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task<NotificationDto?> GetByIdAsync(int notificationId);
        Task<NotificationDto?> GetNotificationDetailsAsync(int notificationId);
        Task<NotificationDto?> GetByProviderMessageIdAsync(string providerMessageId);
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task<NotificationDto?> UpdateAsync(UpdateNotificationDto dto);
        Task<bool> DeleteAsync(DeleteNotificationDto dto);

        // 2. CUSTOMER AND USER NOTIFICATIONS
        Task<IEnumerable<NotificationDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetCustomerNotificationsWithTemplatesAsync(int customerId);
        Task<bool> BelongsToCustomerOrUserAsync(
            int notificationId,
            int? customerId,
            int? userId);

        // 3. CHANNEL AND PRIORITY
        Task<IEnumerable<NotificationDto>> GetByChannelAsync(string channel);
        Task<IEnumerable<NotificationDto>> GetByPriorityAsync(string priority);
        Task<IEnumerable<NotificationDto>> GetUndeliveredUrgentNotificationsAsync();

        // 4. DELIVERY STATUS
        Task<IEnumerable<NotificationDto>> GetByStatusAsync(string status);
        Task<IEnumerable<NotificationDto>> GetPendingNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetQueuedNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetSentNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetDeliveredNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetFailedNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetReadNotificationsAsync();

        // 5. SCHEDULING AND RETRIES
        Task<IEnumerable<NotificationDto>> GetScheduledNotificationsWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<NotificationDto>> GetNotificationsDueForDeliveryAsync(DateTime currentDateTime);
        Task<IEnumerable<NotificationDto>> GetRetryableFailedNotificationsAsync(int maximumRetryCount);
        Task<IEnumerable<NotificationDto>> GetNotificationsWithHighRetryCountAsync(int minimumRetryCount);

        // 6. RELATED BANKING RECORDS AND REPORTING
        Task<IEnumerable<NotificationDto>> GetByRelatedEntityAsync(string entityType, int entityId);
        Task<IEnumerable<NotificationDto>> GetCreatedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<NotificationDto>> GetSentWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<NotificationDto>> SearchNotificationsAsync(string searchTerm);
        Task<int> CountByCustomerAndStatusAsync(int customerId, string status);
        Task<int> CountByUserAndStatusAsync(int userId, string status);

        // 7. NOTIFICATION TEMPLATE CRUD
        Task<IEnumerable<NotificationTemplateDto>> GetAllTemplatesAsync();
        Task<NotificationTemplateDto?> GetTemplateByIdAsync(int notificationTemplateId);
        Task<NotificationTemplateDto?> GetTemplateDetailsAsync(int notificationTemplateId);
        Task<NotificationTemplateDto?> GetByCodeChannelAndLanguageAsync(
            string templateCode,
            string channel,
            string language);
        Task<bool> ExistsByCodeChannelAndLanguageAsync(
            string templateCode,
            string channel,
            string language);
        Task<NotificationTemplateDto> CreateTemplateAsync(CreateNotificationTemplateDto dto);
        Task<NotificationTemplateDto?> UpdateTemplateAsync(UpdateNotificationTemplateDto dto);
        Task<bool> DeleteTemplateAsync(DeleteNotificationTemplateDto dto);

        // 8. TEMPLATE CHANNEL, LANGUAGE AND CATEGORY
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByCodeAsync(string templateCode);
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByChannelAsync(string channel);
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByLanguageAsync(string language);
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByChannelAndLanguageAsync(
            string channel,
            string language);
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByCategoryAsync(string category);

        // 9. TEMPLATE STATUS AND VERSIONING
        Task<IEnumerable<NotificationTemplateDto>> GetActiveTemplatesAsync();
        Task<IEnumerable<NotificationTemplateDto>> GetInactiveTemplatesAsync();
        Task<IEnumerable<NotificationTemplateDto>> GetActiveTemplatesByChannelAndLanguageAsync(
            string channel,
            string language);
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByVersionAsync(int version);
        Task<NotificationTemplateDto?> GetLatestTemplateVersionAsync(
            string templateCode,
            string channel,
            string language);
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByCreatedByAsync(int userId);
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByUpdatedByAsync(int userId);

        // 10. SMS DLT AND TEMPLATE CONTENT
        Task<IEnumerable<NotificationTemplateDto>> GetSmsTemplatesWithDltIdAsync();
        Task<IEnumerable<NotificationTemplateDto>> GetSmsTemplatesWithoutDltIdAsync();
        Task<IEnumerable<NotificationTemplateDto>> SearchTemplatesByBodyAsync(string searchTerm);

        // 11. TEMPLATE USAGE AND SEARCH
        Task<IEnumerable<NotificationTemplateDto>> GetTemplatesWithNotificationsAsync();
        Task<IEnumerable<NotificationTemplateDto>> GetUnusedTemplatesAsync();
        Task<NotificationTemplateDto?> GetTemplateWithNotificationsAsync(int notificationTemplateId);
        Task<int> CountNotificationsByTemplateAsync(int notificationTemplateId);
        Task<IEnumerable<NotificationTemplateDto>> SearchTemplatesAsync(string searchTerm);
    }
}


//Banking rules:
//-Use only active templates for new notifications.
//- Enforce valid channel, priority, category, and delivery-status values.
//- Do not send duplicate notifications when processing provider callbacks or retries.
//- Preserve provider message IDs and delivery timestamps.
//- Apply controlled retry limits to failed notifications.
//- SMS templates intended for DLT-regulated delivery must have the required DLT registration details.
//- Protect recipient addresses and notification content from unauthorized access.