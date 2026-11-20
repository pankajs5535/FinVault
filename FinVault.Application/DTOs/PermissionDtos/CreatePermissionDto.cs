using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PermissionDtos;

/// <summary>Input for creating a Permission.</summary>
public class CreatePermissionDto
{
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ModuleName { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
