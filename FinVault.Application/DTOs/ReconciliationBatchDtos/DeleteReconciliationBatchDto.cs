using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReconciliationBatchDtos;

/// <summary>Input for deleting a ReconciliationBatch.</summary>
public class DeleteReconciliationBatchDto
{
    [Range(1, int.MaxValue)]
    public int ReconciliationBatchId { get; set; }
}
