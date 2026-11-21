
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        // =========================================================
        // 1. NOTIFICATION IDENTIFICATION
        // =========================================================

        // Retrieves complete notification details with customer, user and template.
        // Useful for investigating a notification and its delivery configuration.
        Task<Notification?> GetNotificationDetailsAsync(int notificationId);

        // Retrieves a notification using its external provider message ID.
        // Useful for matching delivery callbacks to the original notification.
        Task<Notification?> GetByProviderMessageIdAsync(string providerMessageId);

        // =========================================================
        // 2. CUSTOMER AND USER NOTIFICATIONS
        // =========================================================

        // Retrieves notifications belonging to a customer.
        // Useful for displaying customer-specific transaction alerts and service messages.
        Task<IEnumerable<Notification>> GetByCustomerIdAsync(int customerId);

        // Retrieves notifications belonging to a user.
        // Useful for displaying user-specific security and account activity notices.
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);

        // Retrieves notifications for a customer with their templates.
        // Useful for reviewing the message and template used for customer communications.
        Task<IEnumerable<Notification>> GetCustomerNotificationsWithTemplatesAsync(int customerId);

        // =========================================================
        // 3. CHANNEL AND PRIORITY
        // =========================================================

        // Retrieves notifications sent through a particular channel.
        // Useful for monitoring SMS, email, push, in-app and WhatsApp delivery activity.
        Task<IEnumerable<Notification>> GetByChannelAsync(string channel);

        // Retrieves notifications matching a particular priority.
        // Useful for prioritizing urgent security and financial alerts.
        Task<IEnumerable<Notification>> GetByPriorityAsync(string priority);

        // Retrieves urgent notifications that have not reached a delivered or read state.
        // Useful for identifying high-priority communications requiring delivery attention.
        Task<IEnumerable<Notification>> GetUndeliveredUrgentNotificationsAsync();

        // =========================================================
        // 4. DELIVERY STATUS
        // =========================================================

        // Retrieves notifications matching a particular delivery status.
        // Useful for monitoring notification delivery lifecycle activity.
        Task<IEnumerable<Notification>> GetByStatusAsync(string status);

        // Retrieves notifications awaiting queue processing.
        // Useful for identifying messages ready to be picked up by a delivery worker.
        Task<IEnumerable<Notification>> GetPendingNotificationsAsync();

        // Retrieves notifications already queued for delivery.
        // Useful for monitoring messages waiting for a provider response.
        Task<IEnumerable<Notification>> GetQueuedNotificationsAsync();

        // Retrieves notifications marked as sent.
        // Useful for monitoring messages submitted to an external provider.
        Task<IEnumerable<Notification>> GetSentNotificationsAsync();

        // Retrieves notifications confirmed as delivered.
        // Useful for reviewing successfully delivered customer communications.
        Task<IEnumerable<Notification>> GetDeliveredNotificationsAsync();

        // Retrieves notifications that failed delivery.
        // Useful for investigating provider errors and failed communications.
        Task<IEnumerable<Notification>> GetFailedNotificationsAsync();

        // Retrieves notifications that have been read.
        // Useful for reviewing read confirmations for supported notification channels.
        Task<IEnumerable<Notification>> GetReadNotificationsAsync();

        // =========================================================
        // 5. SCHEDULING AND RETRIES
        // =========================================================

        // Retrieves notifications scheduled within the supplied date range.
        // Useful for monitoring future-dated customer communications.
        Task<IEnumerable<Notification>> GetScheduledNotificationsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves pending or queued notifications whose scheduled time has arrived.
        // Useful for identifying messages ready for scheduled delivery.
        Task<IEnumerable<Notification>> GetNotificationsDueForDeliveryAsync(DateTime currentDateTime);

        // Retrieves failed notifications whose retry count is below the supplied maximum.
        // Useful for identifying candidates for controlled delivery retries.
        Task<IEnumerable<Notification>> GetRetryableFailedNotificationsAsync(int maximumRetryCount);

        // Retrieves notifications whose retry count exceeds the supplied threshold.
        // Useful for identifying repeated delivery failures requiring investigation.
        Task<IEnumerable<Notification>> GetNotificationsWithHighRetryCountAsync(int minimumRetryCount);

        // =========================================================
        // 6. RELATED BANKING RECORDS AND DATE REPORTING
        // =========================================================

        // Retrieves notifications linked to a particular entity and record ID.
        // Useful for locating alerts associated with transactions, accounts or other banking records.
        Task<IEnumerable<Notification>> GetByRelatedEntityAsync(string entityType, int entityId);

        // Retrieves notifications created within the supplied date range.
        // Useful for daily and monthly notification activity reporting.
        Task<IEnumerable<Notification>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves notifications sent within the supplied date range.
        // Useful for reporting provider submission activity.
        Task<IEnumerable<Notification>> GetSentWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 7. SEARCH AND COUNTS
        // =========================================================

        // Searches notifications by recipient, subject, message, provider ID or failure reason.
        // Useful for locating communication records during customer support and investigation.
        Task<IEnumerable<Notification>> SearchNotificationsAsync(string searchTerm);

        // Counts notifications for a customer matching the supplied status.
        // Useful for monitoring customer notification delivery activity.
        Task<int> CountByCustomerAndStatusAsync(int customerId, string status);

        // Counts notifications for a user matching the supplied status.
        // Useful for monitoring user notification delivery activity.
        Task<int> CountByUserAndStatusAsync(int userId, string status);

        // Checks whether a notification is associated with a customer or user.
        // Useful for validating notification ownership before customer or user access.
        Task<bool> BelongsToCustomerOrUserAsync(int notificationId, int? customerId, int? userId);
    }
}
