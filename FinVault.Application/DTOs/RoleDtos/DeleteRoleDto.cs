using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RoleDtos;

/// <summary>Input for deleting a Role.</summary>
public class DeleteRoleDto
{
    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }
}
