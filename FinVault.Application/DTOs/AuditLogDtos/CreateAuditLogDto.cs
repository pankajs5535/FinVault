using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuditLogDtos;

/// <summary>Input for creating an AuditLog.</summary>
public class CreateAuditLogDto
{
    [Required]
    [StringLength(50)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityName { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int? EntityId { get; set; }

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Success|Failure)$", ErrorMessage = "Outcome must be one of: Success, Failure.")]
    public string Outcome { get; set; } = "Success";

    public bool IsSecuritySensitive { get; set; }

    public string? OldValuesMasked { get; set; }

    public string? NewValuesMasked { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    [StringLength(64)]
    public string? CorrelationId { get; set; }

    [Range(1, int.MaxValue)]
    public int? UserId { get; set; }
}
