using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReconciliationBatchDtos;

/// <summary>Returned to clients for Get All and Get By Id of ReconciliationBatch.</summary>
public class ReconciliationBatchDto
{
    public int ReconciliationBatchId { get; set; }

    public string BatchNumber { get; set; } = string.Empty;

    public string ReconciliationType { get; set; } = string.Empty;

    public DateTime BusinessDate { get; set; }

    public string SourceName { get; set; } = string.Empty;

    public string? FileName { get; set; }

    public string? FileHash { get; set; }

    public int TotalRecords { get; set; }

    public int MatchedRecords { get; set; }

    public int UnmatchedRecords { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal MatchedAmount { get; set; }

    public decimal DifferenceAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? InitiatedByUserId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
