
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface INotificationTemplateRepository : IGenericRepository<NotificationTemplate>
    {
        // =========================================================
        // 1. TEMPLATE IDENTIFICATION
        // =========================================================

        // Retrieves a template using its unique code, channel and language.
        // Useful for selecting the correct communication template for a banking notification.
        Task<NotificationTemplate?> GetByCodeChannelAndLanguageAsync(string templateCode, string channel, string language);

        // Retrieves a template using its primary key with audit user details.
        // Useful for reviewing who created or last updated a communication template.
        Task<NotificationTemplate?> GetTemplateDetailsAsync(int notificationTemplateId);

        // Checks whether a template code already exists for a channel and language.
        // Useful for preventing duplicate templates that violate the database unique index.
        Task<bool> ExistsByCodeChannelAndLanguageAsync(string templateCode, string channel, string language);

        // Retrieves templates using their template code.
        // Useful for reviewing all channel and language versions of a communication template.
        Task<IEnumerable<NotificationTemplate>> GetByTemplateCodeAsync(string templateCode);

        // =========================================================
        // 2. CHANNEL AND LANGUAGE
        // =========================================================

        // Retrieves templates configured for a particular communication channel.
        // Useful for managing SMS, email, push, in-app and WhatsApp notifications.
        Task<IEnumerable<NotificationTemplate>> GetByChannelAsync(string channel);

        // Retrieves templates configured for a particular language.
        // Useful for supporting multilingual customer communications.
        Task<IEnumerable<NotificationTemplate>> GetByLanguageAsync(string language);

        // Retrieves templates matching a channel and language.
        // Useful for selecting the correct localized message for a delivery channel.
        Task<IEnumerable<NotificationTemplate>> GetByChannelAndLanguageAsync(string channel, string language);

        // Retrieves templates matching a particular business category.
        // Useful for separating transactional, security, promotional, regulatory and reminder messages.
        Task<IEnumerable<NotificationTemplate>> GetByCategoryAsync(string category);

        // =========================================================
        // 3. TEMPLATE STATUS AND VERSIONING
        // =========================================================

        // Retrieves all active templates.
        // Useful for ensuring notification processing uses enabled templates.
        Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync();

        // Retrieves all inactive templates.
        // Useful for reviewing templates that are no longer available for new notifications.
        Task<IEnumerable<NotificationTemplate>> GetInactiveTemplatesAsync();

        // Retrieves active templates for a particular channel and language.
        // Useful for resolving the currently usable localized notification templates.
        Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesByChannelAndLanguageAsync(string channel, string language);

        // Retrieves templates matching a particular version.
        // Useful for reviewing historical message formats and controlled template releases.
        Task<IEnumerable<NotificationTemplate>> GetByVersionAsync(int version);

        // Retrieves the highest version for a template code, channel and language.
        // Useful for determining the latest version before creating a new template revision.
        Task<NotificationTemplate?> GetLatestVersionAsync(string templateCode, string channel, string language);

        // Retrieves templates created by a particular user.
        // Useful for auditing which staff members created communication templates.
        Task<IEnumerable<NotificationTemplate>> GetByCreatedByAsync(int userId);

        // Retrieves templates last updated by a particular user.
        // Useful for tracking responsibility for changes to banking communications.
        Task<IEnumerable<NotificationTemplate>> GetByUpdatedByAsync(int userId);

        // =========================================================
        // 4. SMS DLT AND CONTENT
        // =========================================================

        // Retrieves SMS templates with a registered DLT template ID.
        // Useful for checking SMS templates configured for India's DLT compliance requirements.
        Task<IEnumerable<NotificationTemplate>> GetSmsTemplatesWithDltIdAsync();

        // Retrieves SMS templates that do not have a DLT template ID.
        // Useful for identifying SMS templates that need DLT registration details.
        Task<IEnumerable<NotificationTemplate>> GetSmsTemplatesWithoutDltIdAsync();

        // Retrieves templates containing a particular body text or placeholder.
        // Useful for locating templates that need content or placeholder updates.
        Task<IEnumerable<NotificationTemplate>> SearchTemplatesByBodyAsync(string searchTerm);

        // =========================================================
        // 5. NOTIFICATION USAGE
        // =========================================================

        // Retrieves templates that have been used by notifications.
        // Useful for reviewing templates currently referenced by notification records.
        Task<IEnumerable<NotificationTemplate>> GetTemplatesWithNotificationsAsync();

        // Retrieves templates that have not been used by any notification.
        // Useful for identifying unused templates during template maintenance.
        Task<IEnumerable<NotificationTemplate>> GetUnusedTemplatesAsync();

        // Retrieves templates with their notification usage records.
        // Useful for investigating the notification history associated with a template.
        Task<NotificationTemplate?> GetTemplateWithNotificationsAsync(int notificationTemplateId);

        // Counts notifications created from a particular template.
        // Useful for measuring the usage of a template in banking communications.
        Task<int> CountNotificationsByTemplateAsync(int notificationTemplateId);

        // =========================================================
        // 6. SEARCH
        // =========================================================

        // Searches templates by code, name, subject, body, channel or category.
        // Useful for locating templates across a large notification catalogue.
        Task<IEnumerable<NotificationTemplate>> SearchTemplatesAsync(string searchTerm);
    }
}
