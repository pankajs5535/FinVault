using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuditLogDtos;

/// <summary>Input for deleting an AuditLog.</summary>
public class DeleteAuditLogDto
{
    [Range(1, int.MaxValue)]
    public int AuditLogId { get; set; }
}
