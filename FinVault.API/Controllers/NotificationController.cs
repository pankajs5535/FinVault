using FinVault.Application.DTOs.NotificationDtos;
using FinVault.Application.DTOs.NotificationTemplateDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    // ==================== NOTIFICATION CRUD ====================

    [HttpGet("notifications")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/{notificationId:int}")]
    public async Task<IActionResult> GetById(int notificationId) =>
    NotificationResult(await _service.GetByIdAsync(notificationId));

    [HttpGet("notifications/{notificationId:int}/details")]
    public async Task<IActionResult> GetNotificationDetails(int notificationId) =>
    NotificationResult(await _service.GetNotificationDetailsAsync(notificationId));

    [HttpGet("notifications/provider-message/{providerMessageId}")]
    public async Task<IActionResult> GetByProviderMessageId(string providerMessageId) =>
    NotificationResult(await _service.GetByProviderMessageIdAsync(providerMessageId));

    [HttpPost("notifications")]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto request)
    {
        try
        {
            var data = await _service.CreateAsync(request);
            return CreatedAtAction(
            nameof(GetById),
            new { notificationId = data.NotificationId },
            ApiResponse<NotificationDto>.SuccessResponse(
            data, "Notification created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut("notifications")]
    public async Task<IActionResult> Update([FromBody] UpdateNotificationDto request)
    {
        try
        {
            var data = await _service.UpdateAsync(request);
            return NotificationResult(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete("notifications")]
    public async Task<IActionResult> Delete([FromBody] DeleteNotificationDto request)
    {
        try
        {
            var result = await _service.DeleteAsync(request);
            return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Notification deleted successfully."))
            : NotFound(ApiResponse<object>.FailureResponse("Notification not found or could not be deleted.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    // ==================== CUSTOMER AND USER NOTIFICATIONS ====================

    [HttpGet("customers/{customerId:int}/notifications")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var data = await _service.GetByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("users/{userId:int}/notifications")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var data = await _service.GetByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("customers/{customerId:int}/notifications/with-templates")]
    public async Task<IActionResult> GetCustomerNotificationsWithTemplates(int customerId)
    {
        var data = await _service.GetCustomerNotificationsWithTemplatesAsync(customerId);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    // This helper is intended for ownership checks. Do not trust caller-supplied
    // customerId/userId as proof of identity; secure this endpoint using claims
    // or an authorization handler before exposing it to clients.
    [HttpGet("notifications/{notificationId:int}/ownership-check")]
    public async Task<IActionResult> BelongsToCustomerOrUser(
    int notificationId,
    [FromQuery] int? customerId,
    [FromQuery] int? userId)
    {
        var result = await _service.BelongsToCustomerOrUserAsync(
        notificationId, customerId, userId);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== CHANNEL AND PRIORITY ====================

    [HttpGet("notifications/channel/{channel}")]
    public async Task<IActionResult> GetByChannel(string channel)
    {
        var data = await _service.GetByChannelAsync(channel);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/priority/{priority}")]
    public async Task<IActionResult> GetByPriority(string priority)
    {
        var data = await _service.GetByPriorityAsync(priority);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/urgent/undelivered")]
    public async Task<IActionResult> GetUndeliveredUrgentNotifications()
    {
        var data = await _service.GetUndeliveredUrgentNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    // ==================== DELIVERY STATUS ====================

    [HttpGet("notifications/status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _service.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/pending")]
    public async Task<IActionResult> GetPendingNotifications()
    {
        var data = await _service.GetPendingNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/queued")]
    public async Task<IActionResult> GetQueuedNotifications()
    {
        var data = await _service.GetQueuedNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/sent")]
    public async Task<IActionResult> GetSentNotifications()
    {
        var data = await _service.GetSentNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/delivered")]
    public async Task<IActionResult> GetDeliveredNotifications()
    {
        var data = await _service.GetDeliveredNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/failed")]
    public async Task<IActionResult> GetFailedNotifications()
    {
        var data = await _service.GetFailedNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/read")]
    public async Task<IActionResult> GetReadNotifications()
    {
        var data = await _service.GetReadNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    // ==================== SCHEDULING AND RETRIES ====================

    [HttpGet("notifications/scheduled/date-range")]
    public async Task<IActionResult> GetScheduledNotificationsWithinRange(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        var data = await _service.GetScheduledNotificationsWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/due-for-delivery")]
    public async Task<IActionResult> GetNotificationsDueForDelivery(
    [FromQuery] DateTime currentDateTime)
    {
        var data = await _service.GetNotificationsDueForDeliveryAsync(currentDateTime);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/retryable-failed")]
    public async Task<IActionResult> GetRetryableFailedNotifications(
    [FromQuery] int maximumRetryCount)
    {
        var data = await _service.GetRetryableFailedNotificationsAsync(maximumRetryCount);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/high-retry-count")]
    public async Task<IActionResult> GetNotificationsWithHighRetryCount(
    [FromQuery] int minimumRetryCount)
    {
        var data = await _service.GetNotificationsWithHighRetryCountAsync(minimumRetryCount);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    // ==================== RELATED BANKING RECORDS AND REPORTING ====================

    [HttpGet("notifications/related-entity/{entityType}/{entityId:int}")]
    public async Task<IActionResult> GetByRelatedEntity(string entityType, int entityId)
    {
        var data = await _service.GetByRelatedEntityAsync(entityType, entityId);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/created/date-range")]
    public async Task<IActionResult> GetCreatedWithinRange(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        var data = await _service.GetCreatedWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/sent/date-range")]
    public async Task<IActionResult> GetSentWithinRange(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        var data = await _service.GetSentWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/search")]
    public async Task<IActionResult> SearchNotifications([FromQuery] string searchTerm)
    {
        var data = await _service.SearchNotificationsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(data));
    }

    [HttpGet("notifications/customers/{customerId:int}/count")]
    public async Task<IActionResult> CountByCustomerAndStatus(
    int customerId,
    [FromQuery] string status)
    {
        var count = await _service.CountByCustomerAndStatusAsync(customerId, status);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("notifications/users/{userId:int}/count")]
    public async Task<IActionResult> CountByUserAndStatus(
    int userId,
    [FromQuery] string status)
    {
        var count = await _service.CountByUserAndStatusAsync(userId, status);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== NOTIFICATION TEMPLATE CRUD ====================

    [HttpGet("templates")]
    public async Task<IActionResult> GetAllTemplates()
    {
        var data = await _service.GetAllTemplatesAsync();
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/{notificationTemplateId:int}")]
    public async Task<IActionResult> GetTemplateById(int notificationTemplateId) =>
    TemplateResult(await _service.GetTemplateByIdAsync(notificationTemplateId));

    [HttpGet("templates/{notificationTemplateId:int}/details")]
    public async Task<IActionResult> GetTemplateDetails(int notificationTemplateId) =>
    TemplateResult(await _service.GetTemplateDetailsAsync(notificationTemplateId));

    [HttpGet("templates/by-code-channel-language")]
    public async Task<IActionResult> GetByCodeChannelAndLanguage(
    [FromQuery] string templateCode,
    [FromQuery] string channel,
    [FromQuery] string language) =>
    TemplateResult(await _service.GetByCodeChannelAndLanguageAsync(
    templateCode, channel, language));

    [HttpGet("templates/exists/by-code-channel-language")]
    public async Task<IActionResult> ExistsByCodeChannelAndLanguage(
    [FromQuery] string templateCode,
    [FromQuery] string channel,
    [FromQuery] string language)
    {
        var result = await _service.ExistsByCodeChannelAndLanguageAsync(
        templateCode, channel, language);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpPost("templates")]
    public async Task<IActionResult> CreateTemplate(
    [FromBody] CreateNotificationTemplateDto request)
    {
        try
        {
            var data = await _service.CreateTemplateAsync(request);

            return CreatedAtAction(
            nameof(GetTemplateById),
            new { notificationTemplateId = data.NotificationTemplateId },
            ApiResponse<NotificationTemplateDto>.SuccessResponse(
            data, "Notification template created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut("templates")]
    public async Task<IActionResult> UpdateTemplate(
    [FromBody] UpdateNotificationTemplateDto request)
    {
        try
        {
            var data = await _service.UpdateTemplateAsync(request);
            return TemplateResult(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete("templates")]
    public async Task<IActionResult> DeleteTemplate(
    [FromBody] DeleteNotificationTemplateDto request)
    {
        try
        {
            var result = await _service.DeleteTemplateAsync(request);

            return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Notification template deleted successfully."))
            : NotFound(ApiResponse<object>.FailureResponse("Notification template not found or could not be deleted.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    // ==================== TEMPLATE CHANNEL, LANGUAGE AND CATEGORY ====================

    [HttpGet("templates/code/{templateCode}")]
    public async Task<IActionResult> GetTemplatesByCode(string templateCode)
    {
        var data = await _service.GetTemplatesByCodeAsync(templateCode);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/channel/{channel}")]
    public async Task<IActionResult> GetTemplatesByChannel(string channel)
    {
        var data = await _service.GetTemplatesByChannelAsync(channel);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/language/{language}")]
    public async Task<IActionResult> GetTemplatesByLanguage(string language)
    {
        var data = await _service.GetTemplatesByLanguageAsync(language);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/channel/{channel}/language/{language}")]
    public async Task<IActionResult> GetTemplatesByChannelAndLanguage(
    string channel,
    string language)
    {
        var data = await _service.GetTemplatesByChannelAndLanguageAsync(channel, language);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/category/{category}")]
    public async Task<IActionResult> GetTemplatesByCategory(string category)
    {
        var data = await _service.GetTemplatesByCategoryAsync(category);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    // ==================== TEMPLATE STATUS AND VERSIONING ====================

    [HttpGet("templates/active")]
    public async Task<IActionResult> GetActiveTemplates()
    {
        var data = await _service.GetActiveTemplatesAsync();
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/inactive")]
    public async Task<IActionResult> GetInactiveTemplates()
    {
        var data = await _service.GetInactiveTemplatesAsync();
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/active/channel/{channel}/language/{language}")]
    public async Task<IActionResult> GetActiveTemplatesByChannelAndLanguage(
    string channel,
    string language)
    {
        var data = await _service.GetActiveTemplatesByChannelAndLanguageAsync(channel, language);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/version/{version:int}")]
    public async Task<IActionResult> GetTemplatesByVersion(int version)
    {
        var data = await _service.GetTemplatesByVersionAsync(version);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/latest-version")]
    public async Task<IActionResult> GetLatestTemplateVersion(
    [FromQuery] string templateCode,
    [FromQuery] string channel,
    [FromQuery] string language) =>
    TemplateResult(await _service.GetLatestTemplateVersionAsync(
    templateCode, channel, language));

    [HttpGet("templates/created-by/{userId:int}")]
    public async Task<IActionResult> GetTemplatesByCreatedBy(int userId)
    {
        var data = await _service.GetTemplatesByCreatedByAsync(userId);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/updated-by/{userId:int}")]
    public async Task<IActionResult> GetTemplatesByUpdatedBy(int userId)
    {
        var data = await _service.GetTemplatesByUpdatedByAsync(userId);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    // ==================== SMS DLT AND TEMPLATE CONTENT ====================

    [HttpGet("templates/sms/with-dlt-id")]
    public async Task<IActionResult> GetSmsTemplatesWithDltId()
    {
        var data = await _service.GetSmsTemplatesWithDltIdAsync();
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/sms/without-dlt-id")]
    public async Task<IActionResult> GetSmsTemplatesWithoutDltId()
    {
        var data = await _service.GetSmsTemplatesWithoutDltIdAsync();
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/search/body")]
    public async Task<IActionResult> SearchTemplatesByBody([FromQuery] string searchTerm)
    {
        var data = await _service.SearchTemplatesByBodyAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    // ==================== TEMPLATE USAGE AND SEARCH ====================

    [HttpGet("templates/with-notifications")]
    public async Task<IActionResult> GetTemplatesWithNotifications()
    {
        var data = await _service.GetTemplatesWithNotificationsAsync();
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/unused")]
    public async Task<IActionResult> GetUnusedTemplates()
    {
        var data = await _service.GetUnusedTemplatesAsync();
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    [HttpGet("templates/{notificationTemplateId:int}/with-notifications")]
    public async Task<IActionResult> GetTemplateWithNotifications(int notificationTemplateId) =>
    TemplateResult(await _service.GetTemplateWithNotificationsAsync(notificationTemplateId));

    [HttpGet("templates/{notificationTemplateId:int}/notifications/count")]
    public async Task<IActionResult> CountNotificationsByTemplate(int notificationTemplateId)
    {
        var count = await _service.CountNotificationsByTemplateAsync(notificationTemplateId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("templates/search")]
    public async Task<IActionResult> SearchTemplates([FromQuery] string searchTerm)
    {
        var data = await _service.SearchTemplatesAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<NotificationTemplateDto>>.SuccessResponse(data));
    }

    // ==================== RESPONSE HELPERS ====================

    private IActionResult NotificationResult(NotificationDto? data) =>
    data is null
    ? NotFound(ApiResponse<NotificationDto>.FailureResponse("Notification not found.", 404))
    : Ok(ApiResponse<NotificationDto>.SuccessResponse(data));

    private IActionResult TemplateResult(NotificationTemplateDto? data) =>
    data is null
    ? NotFound(ApiResponse<NotificationTemplateDto>.FailureResponse("Notification template not found.", 404))
    : Ok(ApiResponse<NotificationTemplateDto>.SuccessResponse(data));

    private static bool ValidRange(DateTime startDate, DateTime endDate) =>
    startDate != default &&
    endDate != default &&
    endDate >= startDate;

    private IActionResult InvalidRange() =>
    BadRequest(ApiResponse<object>.FailureResponse("A valid date range is required.", 400));
}