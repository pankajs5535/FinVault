using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuditLogDtos;

/// <summary>Returned to clients for Get All and Get By Id of AuditLog.</summary>
public class AuditLogDto
{
    public int AuditLogId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public int? EntityId { get; set; }

    public string Outcome { get; set; } = string.Empty;

    public bool IsSecuritySensitive { get; set; }

    public string? OldValuesMasked { get; set; }

    public string? NewValuesMasked { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? CorrelationId { get; set; }

    public int? UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
