using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RoleDtos;

/// <summary>Input for updating a Role.</summary>
public class UpdateRoleDto
{
    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    public bool IsSystemRole { get; set; }

    public bool IsActive { get; set; } = true;
}
