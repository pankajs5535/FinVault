using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.NotificationTemplateDtos;

/// <summary>Returned to clients for Get All and Get By Id of NotificationTemplate.</summary>
public class NotificationTemplateDto
{
    public int NotificationTemplateId { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Channel { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public string BodyTemplate { get; set; } = string.Empty;

    public string? DltTemplateId { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
