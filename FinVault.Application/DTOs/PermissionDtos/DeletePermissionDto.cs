using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PermissionDtos;

/// <summary>Input for deleting a Permission.</summary>
public class DeletePermissionDto
{
    [Range(1, int.MaxValue)]
    public int PermissionId { get; set; }
}
