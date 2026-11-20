using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RoleDtos;

/// <summary>Returned to clients for Get All and Get By Id of Role.</summary>
public class RoleDto
{
    public int RoleId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsSystemRole { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
