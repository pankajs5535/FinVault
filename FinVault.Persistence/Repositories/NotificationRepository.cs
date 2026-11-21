
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. NOTIFICATION IDENTIFICATION
        // =========================================================

        // Retrieves complete notification details with customer, user and template.
        // Useful for investigating a notification and its delivery configuration.
        public async Task<Notification?> GetNotificationDetailsAsync(int notificationId)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.User)
                .Include(x => x.NotificationTemplate)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);
        }

        // Retrieves a notification using its external provider message ID.
        // Useful for matching delivery callbacks to the original notification.
        public async Task<Notification?> GetByProviderMessageIdAsync(string providerMessageId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.ProviderMessageId == providerMessageId);
        }

        // =========================================================
        // 2. CUSTOMER AND USER NOTIFICATIONS
        // =========================================================

        // Retrieves notifications belonging to a customer.
        // Useful for displaying customer-specific transaction alerts and service messages.
        public async Task<IEnumerable<Notification>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications belonging to a user.
        // Useful for displaying user-specific security and account activity notices.
        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications for a customer with their templates.
        // Useful for reviewing the message and template used for customer communications.
        public async Task<IEnumerable<Notification>> GetCustomerNotificationsWithTemplatesAsync(int customerId)
        {
            return await _dbSet
                .Include(x => x.NotificationTemplate)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. CHANNEL AND PRIORITY
        // =========================================================

        // Retrieves notifications sent through a particular channel.
        // Useful for monitoring SMS, email, push, in-app and WhatsApp delivery activity.
        public async Task<IEnumerable<Notification>> GetByChannelAsync(string channel)
        {
            return await _dbSet
                .Where(x => x.Channel == channel)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications matching a particular priority.
        // Useful for prioritizing urgent security and financial alerts.
        public async Task<IEnumerable<Notification>> GetByPriorityAsync(string priority)
        {
            return await _dbSet
                .Where(x => x.Priority == priority)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves urgent notifications that have not reached a delivered or read state.
        // Useful for identifying high-priority communications requiring delivery attention.
        public async Task<IEnumerable<Notification>> GetUndeliveredUrgentNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Priority == "Urgent" &&
                            (x.Status == "Pending" ||
                             x.Status == "Queued" ||
                             x.Status == "Sent" ||
                             x.Status == "Failed"))
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. DELIVERY STATUS
        // =========================================================

        // Retrieves notifications matching a particular delivery status.
        // Useful for monitoring notification delivery lifecycle activity.
        public async Task<IEnumerable<Notification>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications awaiting queue processing.
        // Useful for identifying messages ready to be picked up by a delivery worker.
        public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Pending")
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications already queued for delivery.
        // Useful for monitoring messages waiting for a provider response.
        public async Task<IEnumerable<Notification>> GetQueuedNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Queued")
                .OrderBy(x => x.ScheduledAt ?? x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications marked as sent.
        // Useful for monitoring messages submitted to an external provider.
        public async Task<IEnumerable<Notification>> GetSentNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Sent")
                .OrderByDescending(x => x.SentAt)
                .ToListAsync();
        }

        // Retrieves notifications confirmed as delivered.
        // Useful for reviewing successfully delivered customer communications.
        public async Task<IEnumerable<Notification>> GetDeliveredNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Delivered")
                .OrderByDescending(x => x.DeliveredAt)
                .ToListAsync();
        }

        // Retrieves notifications that failed delivery.
        // Useful for investigating provider errors and failed communications.
        public async Task<IEnumerable<Notification>> GetFailedNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Failed")
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications that have been read.
        // Useful for reviewing read confirmations for supported notification channels.
        public async Task<IEnumerable<Notification>> GetReadNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Read")
                .OrderByDescending(x => x.ReadAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. SCHEDULING AND RETRIES
        // =========================================================

        // Retrieves notifications scheduled within the supplied date range.
        // Useful for monitoring future-dated customer communications.
        public async Task<IEnumerable<Notification>> GetScheduledNotificationsWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ScheduledAt.HasValue &&
                            x.ScheduledAt.Value >= startDate &&
                            x.ScheduledAt.Value <= endDate)
                .OrderBy(x => x.ScheduledAt)
                .ToListAsync();
        }

        // Retrieves pending or queued notifications whose scheduled time has arrived.
        // Useful for identifying messages ready for scheduled delivery.
        public async Task<IEnumerable<Notification>> GetNotificationsDueForDeliveryAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => (x.Status == "Pending" || x.Status == "Queued") &&
                            (!x.ScheduledAt.HasValue || x.ScheduledAt.Value <= currentDateTime))
                .OrderBy(x => x.ScheduledAt ?? x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves failed notifications whose retry count is below the supplied maximum.
        // Useful for identifying candidates for controlled delivery retries.
        public async Task<IEnumerable<Notification>> GetRetryableFailedNotificationsAsync(int maximumRetryCount)
        {
            return await _dbSet
                .Where(x => x.Status == "Failed" && x.RetryCount < maximumRetryCount)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications whose retry count exceeds the supplied threshold.
        // Useful for identifying repeated delivery failures requiring investigation.
        public async Task<IEnumerable<Notification>> GetNotificationsWithHighRetryCountAsync(int minimumRetryCount)
        {
            return await _dbSet
                .Where(x => x.RetryCount >= minimumRetryCount)
                .OrderByDescending(x => x.RetryCount)
                .ToListAsync();
        }

        // =========================================================
        // 6. RELATED BANKING RECORDS AND DATE REPORTING
        // =========================================================

        // Retrieves notifications linked to a particular entity and record ID.
        // Useful for locating alerts associated with transactions, accounts or other banking records.
        public async Task<IEnumerable<Notification>> GetByRelatedEntityAsync(string entityType, int entityId)
        {
            return await _dbSet
                .Where(x => x.RelatedEntityType == entityType && x.RelatedEntityId == entityId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications created within the supplied date range.
        // Useful for daily and monthly notification activity reporting.
        public async Task<IEnumerable<Notification>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves notifications sent within the supplied date range.
        // Useful for reporting provider submission activity.
        public async Task<IEnumerable<Notification>> GetSentWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.SentAt.HasValue &&
                            x.SentAt.Value >= startDate &&
                            x.SentAt.Value <= endDate)
                .OrderByDescending(x => x.SentAt)
                .ToListAsync();
        }

        // =========================================================
        // 7. SEARCH AND COUNTS
        // =========================================================

        // Searches notifications by recipient, subject, message, provider ID or failure reason.
        // Useful for locating communication records during customer support and investigation.
        public async Task<IEnumerable<Notification>> SearchNotificationsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.RecipientAddress.Contains(searchTerm) ||
                            (x.Subject != null && x.Subject.Contains(searchTerm)) ||
                            x.Body.Contains(searchTerm) ||
                            (x.ProviderMessageId != null && x.ProviderMessageId.Contains(searchTerm)) ||
                            (x.FailureReason != null && x.FailureReason.Contains(searchTerm)) ||
                            (x.RelatedEntityType != null && x.RelatedEntityType.Contains(searchTerm)))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Counts notifications for a customer matching the supplied status.
        // Useful for monitoring customer notification delivery activity.
        public async Task<int> CountByCustomerAndStatusAsync(int customerId, string status)
        {
            return await _dbSet.CountAsync(x => x.CustomerId == customerId && x.Status == status);
        }

        // Counts notifications for a user matching the supplied status.
        // Useful for monitoring user notification delivery activity.
        public async Task<int> CountByUserAndStatusAsync(int userId, string status)
        {
            return await _dbSet.CountAsync(x => x.UserId == userId && x.Status == status);
        }

        // Checks whether a notification is associated with a customer or user.
        // Useful for validating notification ownership before customer or user access.
        public async Task<bool> BelongsToCustomerOrUserAsync(int notificationId, int? customerId, int? userId)
        {
            return await _dbSet.AnyAsync(x =>
                x.NotificationId == notificationId &&
                ((customerId.HasValue && x.CustomerId == customerId) ||
                 (userId.HasValue && x.UserId == userId)));
        }
    }
}
