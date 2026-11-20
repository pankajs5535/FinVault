using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FeeDtos;

/// <summary>Input for deleting a Fee.</summary>
public class DeleteFeeDto
{
    [Range(1, int.MaxValue)]
    public int FeeId { get; set; }
}
