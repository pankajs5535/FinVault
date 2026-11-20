using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReconciliationRecordDtos;

/// <summary>Returned to clients for Get All and Get By Id of ReconciliationRecord.</summary>
public class ReconciliationRecordDto
{
    public int ReconciliationRecordId { get; set; }

    public string ExternalReference { get; set; } = string.Empty;

    public decimal ExternalAmount { get; set; }

    public DateTime ExternalDate { get; set; }

    public decimal? InternalAmount { get; set; }

    public decimal DifferenceAmount { get; set; }

    public string MatchStatus { get; set; } = string.Empty;

    public string? ExceptionReason { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? ResolutionNotes { get; set; }

    public int ReconciliationBatchId { get; set; }

    public int? TransactionId { get; set; }

    public int? ResolvedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
