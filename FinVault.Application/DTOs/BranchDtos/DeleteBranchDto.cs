using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.BranchDtos;

/// <summary>Input for deleting a Branch.</summary>
public class DeleteBranchDto
{
    [Range(1, int.MaxValue)]
    public int BranchId { get; set; }
}
