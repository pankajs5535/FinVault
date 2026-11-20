using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.DisputeDtos;

/// <summary>Input for deleting a Dispute.</summary>
public class DeleteDisputeDto
{
    [Range(1, int.MaxValue)]
    public int DisputeId { get; set; }
}
