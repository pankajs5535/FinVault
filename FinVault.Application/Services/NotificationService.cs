using AutoMapper;
using FinVault.Application.DTOs.NotificationDtos;
using FinVault.Application.DTOs.NotificationTemplateDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class NotificationService : INotificationService
{
    private static readonly string[] ValidChannels =
        { "Sms", "Email", "Push", "InApp", "WhatsApp" };

    private static readonly string[] ValidPriorities =
        { "Low", "Normal", "High", "Urgent" };

    private static readonly string[] ValidNotificationStatuses =
        { "Pending", "Queued", "Sent", "Delivered", "Failed", "Read" };

    private static readonly string[] ValidCategories =
        { "Transactional", "Security", "Promotional", "Regulatory", "Reminder" };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // =========================================================
    // 1. NOTIFICATION CRUD
    // =========================================================

    public async Task<IEnumerable<NotificationDto>> GetAllAsync()
    {
        var notifications = await _unitOfWork.Notification.GetAllAsync();
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<NotificationDto?> GetByIdAsync(int notificationId)
    {
        ValidateId(notificationId, nameof(notificationId));

        var notification = await _unitOfWork.Notification.GetByIdAsync(notificationId);

        return _mapper.Map<NotificationDto?>(notification);
    }

    public async Task<NotificationDto?> GetNotificationDetailsAsync(int notificationId)
    {
        ValidateId(notificationId, nameof(notificationId));

        var notification = await _unitOfWork.Notification
            .GetNotificationDetailsAsync(notificationId);

        return _mapper.Map<NotificationDto?>(notification);
    }

    public async Task<NotificationDto?> GetByProviderMessageIdAsync(string providerMessageId)
    {
        ValidateRequiredString(providerMessageId, nameof(providerMessageId));

        var notification = await _unitOfWork.Notification
            .GetByProviderMessageIdAsync(providerMessageId.Trim());

        return _mapper.Map<NotificationDto?>(notification);
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var channel = NormalizeChannel(dto.Channel);
        var priority = NormalizePriority(dto.Priority);
        var status = NormalizeNotificationStatus(dto.Status);

        ValidateRequiredString(dto.RecipientAddress, nameof(dto.RecipientAddress));
        ValidateRequiredString(dto.Body, nameof(dto.Body));

        if (dto.RecipientAddress.Trim().Length > 256)
            throw new ArgumentException("Recipient address cannot exceed 256 characters.");

        if (dto.Subject?.Length > 200)
            throw new ArgumentException("Subject cannot exceed 200 characters.");

        if (dto.Body.Length > 10000)
            throw new ArgumentException("Notification body cannot exceed 10000 characters.");

        if (status != "Pending")
            throw new InvalidOperationException(
                "A new notification must have Pending status.");

        if (dto.RetryCount != 0)
            throw new InvalidOperationException(
                "A new notification must have zero retries.");

        if (dto.SentAt.HasValue ||
            dto.DeliveredAt.HasValue ||
            dto.ReadAt.HasValue ||
            !string.IsNullOrWhiteSpace(dto.ProviderMessageId) ||
            !string.IsNullOrWhiteSpace(dto.FailureReason))
        {
            throw new InvalidOperationException(
                "Delivery details cannot be supplied when creating a notification.");
        }

        if (dto.CustomerId.HasValue)
            ValidateId(dto.CustomerId.Value, nameof(dto.CustomerId));

        if (dto.UserId.HasValue)
            ValidateId(dto.UserId.Value, nameof(dto.UserId));

        // Enforces CK_Notifications_Recipient.
        if (!dto.CustomerId.HasValue && !dto.UserId.HasValue)
            throw new InvalidOperationException(
                "A notification must belong to at least one customer or user.");

        if (dto.RelatedEntityId.HasValue)
            ValidateId(dto.RelatedEntityId.Value, nameof(dto.RelatedEntityId));

        if (dto.RelatedEntityType?.Length > 50)
            throw new ArgumentException(
                "Related entity type cannot exceed 50 characters.");

        if (dto.RelatedEntityId.HasValue &&
            string.IsNullOrWhiteSpace(dto.RelatedEntityType))
        {
            throw new ArgumentException(
                "Related entity type is required when related entity ID is supplied.");
        }

        if (!dto.RelatedEntityId.HasValue &&
            !string.IsNullOrWhiteSpace(dto.RelatedEntityType))
        {
            throw new ArgumentException(
                "Related entity ID is required when related entity type is supplied.");
        }

        if (dto.CustomerId.HasValue)
        {
            var customer = await _unitOfWork.Customer
                .GetByIdAsync(dto.CustomerId.Value);

            if (customer == null)
                throw new InvalidOperationException(
                    "The specified customer does not exist.");
        }

        if (dto.UserId.HasValue)
        {
            var user = await _unitOfWork.User
                .GetByIdAsync(dto.UserId.Value);

            if (user == null)
                throw new InvalidOperationException(
                    "The specified user does not exist.");
        }

        if (dto.NotificationTemplateId.HasValue)
        {
            ValidateId(
                dto.NotificationTemplateId.Value,
                nameof(dto.NotificationTemplateId));

            var template = await _unitOfWork.NotificationTemplate
                .GetByIdAsync(dto.NotificationTemplateId.Value);

            if (template == null)
                throw new InvalidOperationException(
                    "The specified notification template does not exist.");

            if (!template.IsActive)
                throw new InvalidOperationException(
                    "An inactive template cannot be used for a new notification.");

            if (!string.Equals(
                    template.Channel,
                    channel,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Notification channel must match the selected template channel.");
            }

            if (channel == "Sms" &&
                string.IsNullOrWhiteSpace(template.DltTemplateId))
            {
                throw new InvalidOperationException(
                    "The selected SMS template does not have a DLT template ID.");
            }
        }

        var notification = _mapper.Map<Notification>(dto);

        notification.Channel = channel;
        notification.Priority = priority;
        notification.RecipientAddress = dto.RecipientAddress.Trim();
        notification.Subject = dto.Subject?.Trim();
        notification.Status = "Pending";
        notification.RetryCount = 0;
        notification.ProviderMessageId = null;
        notification.FailureReason = null;
        notification.SentAt = null;
        notification.DeliveredAt = null;
        notification.ReadAt = null;
        notification.RelatedEntityType = dto.RelatedEntityType?.Trim();
        notification.RelatedEntityId = dto.RelatedEntityId;
        notification.CreatedAt = DateTime.UtcNow;
        notification.UpdatedAt = null;

        await _unitOfWork.Notification.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task<NotificationDto?> UpdateAsync(UpdateNotificationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.NotificationId, nameof(dto.NotificationId));

        var channel = NormalizeChannel(dto.Channel);
        var priority = NormalizePriority(dto.Priority);
        var status = NormalizeNotificationStatus(dto.Status);

        ValidateRequiredString(dto.RecipientAddress, nameof(dto.RecipientAddress));
        ValidateRequiredString(dto.Body, nameof(dto.Body));

        if (dto.RecipientAddress.Trim().Length > 256)
            throw new ArgumentException("Recipient address cannot exceed 256 characters.");

        if (dto.Subject?.Length > 200)
            throw new ArgumentException("Subject cannot exceed 200 characters.");

        if (dto.Body.Length > 10000)
            throw new ArgumentException("Notification body cannot exceed 10000 characters.");

        if (dto.RetryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.RetryCount));

        var notification = await _unitOfWork.Notification
            .GetByIdAsync(dto.NotificationId);

        if (notification == null)
            return null;

        // General updates are restricted to Pending notifications.
        if (notification.Status != "Pending")
            throw new InvalidOperationException(
                "Only Pending notifications can be updated through this operation.");

        if (status != notification.Status)
            throw new InvalidOperationException(
                "Delivery status cannot be changed through the general update operation.");

        if (dto.RetryCount != notification.RetryCount)
            throw new InvalidOperationException(
                "Retry count can only be changed by the delivery retry workflow.");

        if (dto.ProviderMessageId != notification.ProviderMessageId ||
            dto.SentAt != notification.SentAt ||
            dto.DeliveredAt != notification.DeliveredAt ||
            dto.ReadAt != notification.ReadAt ||
            dto.FailureReason != notification.FailureReason)
        {
            throw new InvalidOperationException(
                "Provider delivery details cannot be modified through the general update operation.");
        }

        if (dto.CustomerId.HasValue)
            ValidateId(dto.CustomerId.Value, nameof(dto.CustomerId));

        if (dto.UserId.HasValue)
            ValidateId(dto.UserId.Value, nameof(dto.UserId));

        if (!dto.CustomerId.HasValue && !dto.UserId.HasValue)
            throw new InvalidOperationException(
                "A notification must belong to at least one customer or user.");

        if (dto.RelatedEntityId.HasValue)
            ValidateId(dto.RelatedEntityId.Value, nameof(dto.RelatedEntityId));

        if (dto.RelatedEntityType?.Length > 50)
            throw new ArgumentException(
                "Related entity type cannot exceed 50 characters.");

        if (dto.RelatedEntityId.HasValue &&
            string.IsNullOrWhiteSpace(dto.RelatedEntityType))
        {
            throw new ArgumentException(
                "Related entity type is required when related entity ID is supplied.");
        }

        if (!dto.RelatedEntityId.HasValue &&
            !string.IsNullOrWhiteSpace(dto.RelatedEntityType))
        {
            throw new ArgumentException(
                "Related entity ID is required when related entity type is supplied.");
        }

        if (dto.CustomerId.HasValue)
        {
            var customer = await _unitOfWork.Customer
                .GetByIdAsync(dto.CustomerId.Value);

            if (customer == null)
                throw new InvalidOperationException(
                    "The specified customer does not exist.");
        }

        if (dto.UserId.HasValue)
        {
            var user = await _unitOfWork.User
                .GetByIdAsync(dto.UserId.Value);

            if (user == null)
                throw new InvalidOperationException(
                    "The specified user does not exist.");
        }

        if (dto.NotificationTemplateId.HasValue)
        {
            ValidateId(
                dto.NotificationTemplateId.Value,
                nameof(dto.NotificationTemplateId));

            var template = await _unitOfWork.NotificationTemplate
                .GetByIdAsync(dto.NotificationTemplateId.Value);

            if (template == null)
                throw new InvalidOperationException(
                    "The specified notification template does not exist.");

            if (!string.Equals(
                    template.Channel,
                    channel,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Notification channel must match the selected template channel.");
            }
        }

        notification.Channel = channel;
        notification.Priority = priority;
        notification.RecipientAddress = dto.RecipientAddress.Trim();
        notification.Subject = dto.Subject?.Trim();
        notification.Body = dto.Body;
        notification.ScheduledAt = dto.ScheduledAt;
        notification.RelatedEntityType = dto.RelatedEntityType?.Trim();
        notification.RelatedEntityId = dto.RelatedEntityId;
        notification.CustomerId = dto.CustomerId;
        notification.UserId = dto.UserId;
        notification.NotificationTemplateId = dto.NotificationTemplateId;
        notification.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Notification.Update(notification);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task<bool> DeleteAsync(DeleteNotificationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.NotificationId, nameof(dto.NotificationId));

        var notification = await _unitOfWork.Notification
            .GetByIdAsync(dto.NotificationId);

        if (notification == null)
            return false;

        if (notification.Status != "Pending")
            throw new InvalidOperationException(
                "Only Pending notifications can be deleted.");

        _unitOfWork.Notification.Delete(notification);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // 2. CUSTOMER AND USER NOTIFICATIONS
    // =========================================================

    public async Task<IEnumerable<NotificationDto>> GetByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var notifications = await _unitOfWork.Notification
            .GetByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
    {
        ValidateId(userId, nameof(userId));

        var notifications = await _unitOfWork.Notification
            .GetByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>>
        GetCustomerNotificationsWithTemplatesAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var notifications = await _unitOfWork.Notification
            .GetCustomerNotificationsWithTemplatesAsync(customerId);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<bool> BelongsToCustomerOrUserAsync(
        int notificationId,
        int? customerId,
        int? userId)
    {
        ValidateId(notificationId, nameof(notificationId));

        if (!customerId.HasValue && !userId.HasValue)
            throw new ArgumentException(
                "Customer ID or user ID must be supplied.");

        if (customerId.HasValue)
            ValidateId(customerId.Value, nameof(customerId));

        if (userId.HasValue)
            ValidateId(userId.Value, nameof(userId));

        return await _unitOfWork.Notification
            .BelongsToCustomerOrUserAsync(notificationId, customerId, userId);
    }

    // =========================================================
    // 3. CHANNEL AND PRIORITY
    // =========================================================

    public async Task<IEnumerable<NotificationDto>> GetByChannelAsync(string channel)
    {
        var normalizedChannel = NormalizeChannel(channel);

        var notifications = await _unitOfWork.Notification
            .GetByChannelAsync(normalizedChannel);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetByPriorityAsync(string priority)
    {
        var normalizedPriority = NormalizePriority(priority);

        var notifications = await _unitOfWork.Notification
            .GetByPriorityAsync(normalizedPriority);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>>
        GetUndeliveredUrgentNotificationsAsync()
    {
        var notifications = await _unitOfWork.Notification
            .GetUndeliveredUrgentNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    // =========================================================
    // 4. DELIVERY STATUS
    // =========================================================

    public async Task<IEnumerable<NotificationDto>> GetByStatusAsync(string status)
    {
        var normalizedStatus = NormalizeNotificationStatus(status);

        var notifications = await _unitOfWork.Notification
            .GetByStatusAsync(normalizedStatus);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetPendingNotificationsAsync()
    {
        var notifications = await _unitOfWork.Notification
            .GetPendingNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetQueuedNotificationsAsync()
    {
        var notifications = await _unitOfWork.Notification
            .GetQueuedNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetSentNotificationsAsync()
    {
        var notifications = await _unitOfWork.Notification
            .GetSentNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetDeliveredNotificationsAsync()
    {
        var notifications = await _unitOfWork.Notification
            .GetDeliveredNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetFailedNotificationsAsync()
    {
        var notifications = await _unitOfWork.Notification
            .GetFailedNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetReadNotificationsAsync()
    {
        var notifications = await _unitOfWork.Notification
            .GetReadNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    // =========================================================
    // 5. SCHEDULING AND RETRIES
    // =========================================================

    public async Task<IEnumerable<NotificationDto>>
        GetScheduledNotificationsWithinRangeAsync(
            DateTime startDate,
            DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var notifications = await _unitOfWork.Notification
            .GetScheduledNotificationsWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>>
        GetNotificationsDueForDeliveryAsync(DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var notifications = await _unitOfWork.Notification
            .GetNotificationsDueForDeliveryAsync(currentDateTime);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>>
        GetRetryableFailedNotificationsAsync(int maximumRetryCount)
    {
        if (maximumRetryCount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(maximumRetryCount),
                "Maximum retry count must be greater than zero.");

        var notifications = await _unitOfWork.Notification
            .GetRetryableFailedNotificationsAsync(maximumRetryCount);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>>
        GetNotificationsWithHighRetryCountAsync(int minimumRetryCount)
    {
        if (minimumRetryCount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(minimumRetryCount),
                "Minimum retry count must be greater than zero.");

        var notifications = await _unitOfWork.Notification
            .GetNotificationsWithHighRetryCountAsync(minimumRetryCount);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    // =========================================================
    // 6. RELATED BANKING RECORDS AND REPORTING
    // =========================================================

    public async Task<IEnumerable<NotificationDto>> GetByRelatedEntityAsync(
        string entityType,
        int entityId)
    {
        ValidateRequiredString(entityType, nameof(entityType));
        ValidateId(entityId, nameof(entityId));

        var notifications = await _unitOfWork.Notification
            .GetByRelatedEntityAsync(entityType.Trim(), entityId);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetCreatedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var notifications = await _unitOfWork.Notification
            .GetCreatedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetSentWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var notifications = await _unitOfWork.Notification
            .GetSentWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> SearchNotificationsAsync(
        string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        var notifications = await _unitOfWork.Notification
            .SearchNotificationsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<int> CountByCustomerAndStatusAsync(int customerId, string status)
    {
        ValidateId(customerId, nameof(customerId));

        var normalizedStatus = NormalizeNotificationStatus(status);

        return await _unitOfWork.Notification
            .CountByCustomerAndStatusAsync(customerId, normalizedStatus);
    }

    public async Task<int> CountByUserAndStatusAsync(int userId, string status)
    {
        ValidateId(userId, nameof(userId));

        var normalizedStatus = NormalizeNotificationStatus(status);

        return await _unitOfWork.Notification
            .CountByUserAndStatusAsync(userId, normalizedStatus);
    }

    // =========================================================
    // 7. NOTIFICATION TEMPLATE CRUD
    // =========================================================

    public async Task<IEnumerable<NotificationTemplateDto>> GetAllTemplatesAsync()
    {
        var templates = await _unitOfWork.NotificationTemplate.GetAllAsync();
        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<NotificationTemplateDto?> GetTemplateByIdAsync(
        int notificationTemplateId)
    {
        ValidateId(notificationTemplateId, nameof(notificationTemplateId));

        var template = await _unitOfWork.NotificationTemplate
            .GetByIdAsync(notificationTemplateId);

        return _mapper.Map<NotificationTemplateDto?>(template);
    }

    public async Task<NotificationTemplateDto?> GetTemplateDetailsAsync(
        int notificationTemplateId)
    {
        ValidateId(notificationTemplateId, nameof(notificationTemplateId));

        var template = await _unitOfWork.NotificationTemplate
            .GetTemplateDetailsAsync(notificationTemplateId);

        return _mapper.Map<NotificationTemplateDto?>(template);
    }

    public async Task<NotificationTemplateDto?> GetByCodeChannelAndLanguageAsync(
        string templateCode,
        string channel,
        string language)
    {
        ValidateRequiredString(templateCode, nameof(templateCode));

        var normalizedChannel = NormalizeChannel(channel);
        ValidateLanguage(language);

        var template = await _unitOfWork.NotificationTemplate
            .GetByCodeChannelAndLanguageAsync(
                templateCode.Trim(),
                normalizedChannel,
                language.Trim());

        return _mapper.Map<NotificationTemplateDto?>(template);
    }

    public async Task<bool> ExistsByCodeChannelAndLanguageAsync(
        string templateCode,
        string channel,
        string language)
    {
        ValidateRequiredString(templateCode, nameof(templateCode));

        var normalizedChannel = NormalizeChannel(channel);
        ValidateLanguage(language);

        return await _unitOfWork.NotificationTemplate
            .ExistsByCodeChannelAndLanguageAsync(
                templateCode.Trim(),
                normalizedChannel,
                language.Trim());
    }

    public async Task<NotificationTemplateDto> CreateTemplateAsync(
        CreateNotificationTemplateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateTemplateFields(
            dto.TemplateCode,
            dto.Name,
            dto.Channel,
            dto.Language,
            dto.Category,
            dto.BodyTemplate,
            dto.Version,
            dto.Subject,
            dto.DltTemplateId);

        var channel = NormalizeChannel(dto.Channel);
        var category = NormalizeCategory(dto.Category);
        var language = dto.Language.Trim();
        var templateCode = dto.TemplateCode.Trim();

        if (dto.IsActive &&
            channel == "Sms" &&
            string.IsNullOrWhiteSpace(dto.DltTemplateId))
        {
            throw new InvalidOperationException(
                "An active SMS template requires a DLT template ID.");
        }

        var exists = await _unitOfWork.NotificationTemplate
            .ExistsByCodeChannelAndLanguageAsync(
                templateCode,
                channel,
                language);

        if (exists)
            throw new InvalidOperationException(
                "A template with this code, channel and language already exists.");

        var template = _mapper.Map<NotificationTemplate>(dto);

        template.TemplateCode = templateCode;
        template.Name = dto.Name.Trim();
        template.Channel = channel;
        template.Language = language;
        template.Category = category;
        template.Subject = dto.Subject?.Trim();
        template.BodyTemplate = dto.BodyTemplate;
        template.DltTemplateId = dto.DltTemplateId?.Trim();
        template.CreatedAt = DateTime.UtcNow;
        template.UpdatedAt = null;

        await _unitOfWork.NotificationTemplate.AddAsync(template);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationTemplateDto>(template);
    }

    public async Task<NotificationTemplateDto?> UpdateTemplateAsync(
        UpdateNotificationTemplateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(
            dto.NotificationTemplateId,
            nameof(dto.NotificationTemplateId));

        ValidateTemplateFields(
            dto.TemplateCode,
            dto.Name,
            dto.Channel,
            dto.Language,
            dto.Category,
            dto.BodyTemplate,
            dto.Version,
            dto.Subject,
            dto.DltTemplateId);

        var template = await _unitOfWork.NotificationTemplate
            .GetByIdAsync(dto.NotificationTemplateId);

        if (template == null)
            return null;

        var channel = NormalizeChannel(dto.Channel);
        var category = NormalizeCategory(dto.Category);

        if (!string.Equals(
                template.TemplateCode,
                dto.TemplateCode.Trim(),
                StringComparison.Ordinal) ||
            !string.Equals(
                template.Channel,
                channel,
                StringComparison.Ordinal) ||
            !string.Equals(
                template.Language,
                dto.Language.Trim(),
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Template code, channel and language cannot be changed.");
        }

        if (template.Version != dto.Version)
            throw new InvalidOperationException(
                "Template version cannot be changed through the update operation.");

        if (dto.IsActive &&
            channel == "Sms" &&
            string.IsNullOrWhiteSpace(dto.DltTemplateId))
        {
            throw new InvalidOperationException(
                "An active SMS template requires a DLT template ID.");
        }

        template.Name = dto.Name.Trim();
        template.Category = category;
        template.Subject = dto.Subject?.Trim();
        template.BodyTemplate = dto.BodyTemplate;
        template.DltTemplateId = dto.DltTemplateId?.Trim();
        template.IsActive = dto.IsActive;
        template.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.NotificationTemplate.Update(template);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationTemplateDto>(template);
    }

    public async Task<bool> DeleteTemplateAsync(DeleteNotificationTemplateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(
            dto.NotificationTemplateId,
            nameof(dto.NotificationTemplateId));

        var template = await _unitOfWork.NotificationTemplate
            .GetByIdAsync(dto.NotificationTemplateId);

        if (template == null)
            return false;

        var notificationCount = await _unitOfWork.NotificationTemplate
            .CountNotificationsByTemplateAsync(dto.NotificationTemplateId);

        if (notificationCount > 0)
            throw new InvalidOperationException(
                "A template used by notifications cannot be deleted. Deactivate it instead.");

        _unitOfWork.NotificationTemplate.Delete(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // 8. TEMPLATE CHANNEL, LANGUAGE AND CATEGORY
    // =========================================================

    public async Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByCodeAsync(
        string templateCode)
    {
        ValidateRequiredString(templateCode, nameof(templateCode));

        var templates = await _unitOfWork.NotificationTemplate
            .GetByTemplateCodeAsync(templateCode.Trim());

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByChannelAsync(
        string channel)
    {
        var normalizedChannel = NormalizeChannel(channel);

        var templates = await _unitOfWork.NotificationTemplate
            .GetByChannelAsync(normalizedChannel);

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByLanguageAsync(
        string language)
    {
        ValidateLanguage(language);

        var templates = await _unitOfWork.NotificationTemplate
            .GetByLanguageAsync(language.Trim());

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>>
        GetTemplatesByChannelAndLanguageAsync(
            string channel,
            string language)
    {
        var normalizedChannel = NormalizeChannel(channel);
        ValidateLanguage(language);

        var templates = await _unitOfWork.NotificationTemplate
            .GetByChannelAndLanguageAsync(
                normalizedChannel,
                language.Trim());

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByCategoryAsync(
        string category)
    {
        var normalizedCategory = NormalizeCategory(category);

        var templates = await _unitOfWork.NotificationTemplate
            .GetByCategoryAsync(normalizedCategory);

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    // =========================================================
    // 9. TEMPLATE STATUS AND VERSIONING
    // =========================================================

    public async Task<IEnumerable<NotificationTemplateDto>> GetActiveTemplatesAsync()
    {
        var templates = await _unitOfWork.NotificationTemplate
            .GetActiveTemplatesAsync();

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetInactiveTemplatesAsync()
    {
        var templates = await _unitOfWork.NotificationTemplate
            .GetInactiveTemplatesAsync();

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>>
        GetActiveTemplatesByChannelAndLanguageAsync(
            string channel,
            string language)
    {
        var normalizedChannel = NormalizeChannel(channel);
        ValidateLanguage(language);

        var templates = await _unitOfWork.NotificationTemplate
            .GetActiveTemplatesByChannelAndLanguageAsync(
                normalizedChannel,
                language.Trim());

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByVersionAsync(
        int version)
    {
        if (version <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(version),
                "Version must be greater than zero.");

        var templates = await _unitOfWork.NotificationTemplate
            .GetByVersionAsync(version);

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<NotificationTemplateDto?> GetLatestTemplateVersionAsync(
        string templateCode,
        string channel,
        string language)
    {
        ValidateRequiredString(templateCode, nameof(templateCode));

        var normalizedChannel = NormalizeChannel(channel);
        ValidateLanguage(language);

        var template = await _unitOfWork.NotificationTemplate
            .GetLatestVersionAsync(
                templateCode.Trim(),
                normalizedChannel,
                language.Trim());

        return _mapper.Map<NotificationTemplateDto?>(template);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByCreatedByAsync(
        int userId)
    {
        ValidateId(userId, nameof(userId));

        var templates = await _unitOfWork.NotificationTemplate
            .GetByCreatedByAsync(userId);

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetTemplatesByUpdatedByAsync(
        int userId)
    {
        ValidateId(userId, nameof(userId));

        var templates = await _unitOfWork.NotificationTemplate
            .GetByUpdatedByAsync(userId);

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    // =========================================================
    // 10. SMS DLT AND TEMPLATE CONTENT
    // =========================================================

    public async Task<IEnumerable<NotificationTemplateDto>> GetSmsTemplatesWithDltIdAsync()
    {
        var templates = await _unitOfWork.NotificationTemplate
            .GetSmsTemplatesWithDltIdAsync();

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetSmsTemplatesWithoutDltIdAsync()
    {
        var templates = await _unitOfWork.NotificationTemplate
            .GetSmsTemplatesWithoutDltIdAsync();

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> SearchTemplatesByBodyAsync(
        string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        var templates = await _unitOfWork.NotificationTemplate
            .SearchTemplatesByBodyAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    // =========================================================
    // 11. TEMPLATE USAGE AND SEARCH
    // =========================================================

    public async Task<IEnumerable<NotificationTemplateDto>>
        GetTemplatesWithNotificationsAsync()
    {
        var templates = await _unitOfWork.NotificationTemplate
            .GetTemplatesWithNotificationsAsync();

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetUnusedTemplatesAsync()
    {
        var templates = await _unitOfWork.NotificationTemplate
            .GetUnusedTemplatesAsync();

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    public async Task<NotificationTemplateDto?> GetTemplateWithNotificationsAsync(
        int notificationTemplateId)
    {
        ValidateId(notificationTemplateId, nameof(notificationTemplateId));

        var template = await _unitOfWork.NotificationTemplate
            .GetTemplateWithNotificationsAsync(notificationTemplateId);

        return _mapper.Map<NotificationTemplateDto?>(template);
    }

    public async Task<int> CountNotificationsByTemplateAsync(int notificationTemplateId)
    {
        ValidateId(notificationTemplateId, nameof(notificationTemplateId));

        return await _unitOfWork.NotificationTemplate
            .CountNotificationsByTemplateAsync(notificationTemplateId);
    }

    public async Task<IEnumerable<NotificationTemplateDto>> SearchTemplatesAsync(
        string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        var templates = await _unitOfWork.NotificationTemplate
            .SearchTemplatesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }

    // =========================================================
    // PRIVATE VALIDATION METHODS
    // =========================================================

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }

    private static void ValidateRequiredString(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static string NormalizeChannel(string? channel)
    {
        ValidateRequiredString(channel, nameof(channel));

        return ValidChannels.FirstOrDefault(x =>
                   string.Equals(
                       x,
                       channel.Trim(),
                       StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException(
                   $"Invalid channel. Allowed values: {string.Join(", ", ValidChannels)}.");
    }

    private static string NormalizePriority(string? priority)
    {
        ValidateRequiredString(priority, nameof(priority));

        return ValidPriorities.FirstOrDefault(x =>
                   string.Equals(
                       x,
                       priority.Trim(),
                       StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException(
                   $"Invalid priority. Allowed values: {string.Join(", ", ValidPriorities)}.");
    }

    private static string NormalizeNotificationStatus(string? status)
    {
        ValidateRequiredString(status, nameof(status));

        return ValidNotificationStatuses.FirstOrDefault(x =>
                   string.Equals(
                       x,
                       status.Trim(),
                       StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException(
                   $"Invalid notification status. Allowed values: {string.Join(", ", ValidNotificationStatuses)}.");
    }

    private static string NormalizeCategory(string? category)
    {
        ValidateRequiredString(category, nameof(category));

        return ValidCategories.FirstOrDefault(x =>
                   string.Equals(
                       x,
                       category.Trim(),
                       StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException(
                   $"Invalid category. Allowed values: {string.Join(", ", ValidCategories)}.");
    }

    private static void ValidateLanguage(string? language)
    {
        ValidateRequiredString(language, nameof(language));

        if (language.Trim().Length > 10)
            throw new ArgumentException(
                "Language cannot exceed 10 characters.");
    }

    private static void ValidateTemplateFields(
        string? templateCode,
        string? name,
        string? channel,
        string? language,
        string? category,
        string? bodyTemplate,
        int version,
        string? subject,
        string? dltTemplateId)
    {
        ValidateRequiredString(templateCode, nameof(templateCode));
        ValidateRequiredString(name, nameof(name));
        NormalizeChannel(channel);
        ValidateLanguage(language);
        NormalizeCategory(category);
        ValidateRequiredString(bodyTemplate, nameof(bodyTemplate));

        if (templateCode!.Trim().Length > 50)
            throw new ArgumentException(
                "Template code cannot exceed 50 characters.");

        if (name!.Trim().Length > 100)
            throw new ArgumentException(
                "Template name cannot exceed 100 characters.");

        if (bodyTemplate!.Length > 10000)
            throw new ArgumentException(
                "Template body cannot exceed 10000 characters.");

        if (subject?.Length > 200)
            throw new ArgumentException(
                "Template subject cannot exceed 200 characters.");

        if (dltTemplateId?.Length > 30)
            throw new ArgumentException(
                "DLT template ID cannot exceed 30 characters.");

        if (version <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(version),
                "Template version must be greater than zero.");
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }
}