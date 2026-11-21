
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class NotificationTemplateRepository : GenericRepository<NotificationTemplate>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. TEMPLATE IDENTIFICATION
        // =========================================================

        // Retrieves a template using its unique code, channel and language.
        // Useful for selecting the correct communication template for a banking notification.
        public async Task<NotificationTemplate?> GetByCodeChannelAndLanguageAsync(string templateCode, string channel, string language)
        {
            return await _dbSet.FirstOrDefaultAsync(x =>
                x.TemplateCode == templateCode &&
                x.Channel == channel &&
                x.Language == language);
        }

        // Retrieves a template using its primary key with audit user details.
        // Useful for reviewing who created or last updated a communication template.
        public async Task<NotificationTemplate?> GetTemplateDetailsAsync(int notificationTemplateId)
        {
            return await _dbSet
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.NotificationTemplateId == notificationTemplateId);
        }

        // Checks whether a template code already exists for a channel and language.
        // Useful for preventing duplicate templates that violate the database unique index.
        public async Task<bool> ExistsByCodeChannelAndLanguageAsync(string templateCode, string channel, string language)
        {
            return await _dbSet.AnyAsync(x =>
                x.TemplateCode == templateCode &&
                x.Channel == channel &&
                x.Language == language);
        }

        // Retrieves templates using their template code.
        // Useful for reviewing all channel and language versions of a communication template.
        public async Task<IEnumerable<NotificationTemplate>> GetByTemplateCodeAsync(string templateCode)
        {
            return await _dbSet
                .Where(x => x.TemplateCode == templateCode)
                .OrderBy(x => x.Channel)
                .ThenBy(x => x.Language)
                .ThenByDescending(x => x.Version)
                .ToListAsync();
        }

        // =========================================================
        // 2. CHANNEL AND LANGUAGE
        // =========================================================

        // Retrieves templates configured for a particular communication channel.
        // Useful for managing SMS, email, push, in-app and WhatsApp notifications.
        public async Task<IEnumerable<NotificationTemplate>> GetByChannelAsync(string channel)
        {
            return await _dbSet
                .Where(x => x.Channel == channel)
                .OrderBy(x => x.TemplateCode)
                .ThenBy(x => x.Language)
                .ToListAsync();
        }

        // Retrieves templates configured for a particular language.
        // Useful for supporting multilingual customer communications.
        public async Task<IEnumerable<NotificationTemplate>> GetByLanguageAsync(string language)
        {
            return await _dbSet
                .Where(x => x.Language == language)
                .OrderBy(x => x.TemplateCode)
                .ThenBy(x => x.Channel)
                .ToListAsync();
        }

        // Retrieves templates matching a channel and language.
        // Useful for selecting the correct localized message for a delivery channel.
        public async Task<IEnumerable<NotificationTemplate>> GetByChannelAndLanguageAsync(string channel, string language)
        {
            return await _dbSet
                .Where(x => x.Channel == channel && x.Language == language)
                .OrderBy(x => x.TemplateCode)
                .ThenByDescending(x => x.Version)
                .ToListAsync();
        }

        // Retrieves templates matching a particular business category.
        // Useful for separating transactional, security, promotional, regulatory and reminder messages.
        public async Task<IEnumerable<NotificationTemplate>> GetByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(x => x.Category == category)
                .OrderBy(x => x.TemplateCode)
                .ToListAsync();
        }

        // =========================================================
        // 3. TEMPLATE STATUS AND VERSIONING
        // =========================================================

        // Retrieves all active templates.
        // Useful for ensuring notification processing uses enabled templates.
        public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.TemplateCode)
                .ThenBy(x => x.Channel)
                .ThenBy(x => x.Language)
                .ToListAsync();
        }

        // Retrieves all inactive templates.
        // Useful for reviewing templates that are no longer available for new notifications.
        public async Task<IEnumerable<NotificationTemplate>> GetInactiveTemplatesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderBy(x => x.TemplateCode)
                .ThenByDescending(x => x.Version)
                .ToListAsync();
        }

        // Retrieves active templates for a particular channel and language.
        // Useful for resolving the currently usable localized notification templates.
        public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesByChannelAndLanguageAsync(string channel, string language)
        {
            return await _dbSet
                .Where(x => x.IsActive && x.Channel == channel && x.Language == language)
                .OrderBy(x => x.TemplateCode)
                .ThenByDescending(x => x.Version)
                .ToListAsync();
        }

        // Retrieves templates matching a particular version.
        // Useful for reviewing historical message formats and controlled template releases.
        public async Task<IEnumerable<NotificationTemplate>> GetByVersionAsync(int version)
        {
            return await _dbSet
                .Where(x => x.Version == version)
                .OrderBy(x => x.TemplateCode)
                .ThenBy(x => x.Channel)
                .ThenBy(x => x.Language)
                .ToListAsync();
        }

        // Retrieves the highest version for a template code, channel and language.
        // Useful for determining the latest version before creating a new template revision.
        public async Task<NotificationTemplate?> GetLatestVersionAsync(string templateCode, string channel, string language)
        {
            return await _dbSet
                .Where(x => x.TemplateCode == templateCode &&
                            x.Channel == channel &&
                            x.Language == language)
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync();
        }

        // Retrieves templates created by a particular user.
        // Useful for auditing which staff members created communication templates.
        public async Task<IEnumerable<NotificationTemplate>> GetByCreatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves templates last updated by a particular user.
        // Useful for tracking responsibility for changes to banking communications.
        public async Task<IEnumerable<NotificationTemplate>> GetByUpdatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UpdatedBy == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. SMS DLT AND CONTENT
        // =========================================================

        // Retrieves SMS templates with a registered DLT template ID.
        // Useful for checking SMS templates configured for India's DLT compliance requirements.
        public async Task<IEnumerable<NotificationTemplate>> GetSmsTemplatesWithDltIdAsync()
        {
            return await _dbSet
                .Where(x => x.Channel == "Sms" && x.DltTemplateId != null && x.DltTemplateId != "")
                .OrderBy(x => x.TemplateCode)
                .ToListAsync();
        }

        // Retrieves SMS templates that do not have a DLT template ID.
        // Useful for identifying SMS templates that need DLT registration details.
        public async Task<IEnumerable<NotificationTemplate>> GetSmsTemplatesWithoutDltIdAsync()
        {
            return await _dbSet
                .Where(x => x.Channel == "Sms" && (x.DltTemplateId == null || x.DltTemplateId == ""))
                .OrderBy(x => x.TemplateCode)
                .ToListAsync();
        }

        // Retrieves templates containing a particular body text or placeholder.
        // Useful for locating templates that need content or placeholder updates.
        public async Task<IEnumerable<NotificationTemplate>> SearchTemplatesByBodyAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.BodyTemplate.Contains(searchTerm))
                .OrderBy(x => x.TemplateCode)
                .ToListAsync();
        }

        // =========================================================
        // 5. NOTIFICATION USAGE
        // =========================================================

        // Retrieves templates that have been used by notifications.
        // Useful for reviewing templates currently referenced by notification records.
        public async Task<IEnumerable<NotificationTemplate>> GetTemplatesWithNotificationsAsync()
        {
            return await _dbSet
                .Where(x => x.Notifications.Any())
                .OrderBy(x => x.TemplateCode)
                .ToListAsync();
        }

        // Retrieves templates that have not been used by any notification.
        // Useful for identifying unused templates during template maintenance.
        public async Task<IEnumerable<NotificationTemplate>> GetUnusedTemplatesAsync()
        {
            return await _dbSet
                .Where(x => !x.Notifications.Any())
                .OrderBy(x => x.TemplateCode)
                .ToListAsync();
        }

        // Retrieves a template with its notification history.
        // Useful for investigating the notification records associated with a template.
        public async Task<NotificationTemplate?> GetTemplateWithNotificationsAsync(int notificationTemplateId)
        {
            return await _dbSet
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.NotificationTemplateId == notificationTemplateId);
        }

        // Counts notifications created from a particular template.
        // Useful for measuring the usage of a template in banking communications.
        public async Task<int> CountNotificationsByTemplateAsync(int notificationTemplateId)
        {
            return await _dbSet
                .Where(x => x.NotificationTemplateId == notificationTemplateId)
                .SelectMany(x => x.Notifications)
                .CountAsync();
        }

        // =========================================================
        // 6. SEARCH
        // =========================================================

        // Searches templates by code, name, subject, body, channel or category.
        // Useful for locating templates across a large notification catalogue.
        public async Task<IEnumerable<NotificationTemplate>> SearchTemplatesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.TemplateCode.Contains(searchTerm) ||
                            x.Name.Contains(searchTerm) ||
                            (x.Subject != null && x.Subject.Contains(searchTerm)) ||
                            x.BodyTemplate.Contains(searchTerm) ||
                            x.Channel.Contains(searchTerm) ||
                            x.Category.Contains(searchTerm))
                .OrderBy(x => x.TemplateCode)
                .ThenBy(x => x.Channel)
                .ThenBy(x => x.Language)
                .ToListAsync();
        }
    }
}
