using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PermissionDtos;

/// <summary>Returned to clients for Get All and Get By Id of Permission.</summary>
public class PermissionDto
{
    public int PermissionId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ModuleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
