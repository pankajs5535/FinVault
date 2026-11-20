using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReconciliationRecordDtos;

/// <summary>Input for deleting a ReconciliationRecord.</summary>
public class DeleteReconciliationRecordDto
{
    [Range(1, int.MaxValue)]
    public int ReconciliationRecordId { get; set; }
}
