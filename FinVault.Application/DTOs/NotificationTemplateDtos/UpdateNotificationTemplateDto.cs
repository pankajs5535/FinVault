using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.NotificationTemplateDtos;

/// <summary>Input for updating a NotificationTemplate.</summary>
public class UpdateNotificationTemplateDto
{
    [Range(1, int.MaxValue)]
    public int NotificationTemplateId { get; set; }

    [Required]
    [StringLength(50)]
    public string TemplateCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Sms|Email|Push|InApp|WhatsApp)$", ErrorMessage = "Channel must be one of: Sms, Email, Push, InApp, WhatsApp.")]
    public string Channel { get; set; } = "Sms";

    [Required]
    [StringLength(10)]
    public string Language { get; set; } = "en";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Transactional|Security|Promotional|Regulatory|Reminder)$", ErrorMessage = "Category must be one of: Transactional, Security, Promotional, Regulatory, Reminder.")]
    public string Category { get; set; } = "Transactional";

    [StringLength(200)]
    public string? Subject { get; set; }

    [Required]
    public string BodyTemplate { get; set; } = string.Empty;

    [StringLength(30)]
    public string? DltTemplateId { get; set; }

    [Range(0, int.MaxValue)]
    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}
