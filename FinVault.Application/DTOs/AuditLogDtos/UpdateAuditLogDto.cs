using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuditLogDtos;

/// <summary>Input for updating an AuditLog. AuditLog records are append-only, so only the key is accepted.</summary>
public class UpdateAuditLogDto
{
    [Range(1, int.MaxValue)]
    public int AuditLogId { get; set; }
}
