using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReconciliationBatchDtos;

/// <summary>Input for creating a ReconciliationBatch.</summary>
public class CreateReconciliationBatchDto
{
    [Required]
    [StringLength(30)]
    public string BatchNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Upi|Neft|Rtgs|Imps|CardSettlement|AtmSwitch|GeneralLedger)$", ErrorMessage = "ReconciliationType must be one of: Upi, Neft, Rtgs, Imps, CardSettlement, AtmSwitch, GeneralLedger.")]
    public string ReconciliationType { get; set; } = string.Empty;

    public DateTime BusinessDate { get; set; }

    [Required]
    [StringLength(100)]
    public string SourceName { get; set; } = string.Empty;

    [StringLength(260)]
    public string? FileName { get; set; }

    [StringLength(64)]
    public string? FileHash { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalRecords { get; set; } = 0;

    [Range(0, int.MaxValue)]
    public int MatchedRecords { get; set; } = 0;

    [Range(0, int.MaxValue)]
    public int UnmatchedRecords { get; set; } = 0;

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MatchedAmount { get; set; }

    public decimal DifferenceAmount { get; set; }

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^(Pending|InProgress|Completed|CompletedWithExceptions|Failed)$", ErrorMessage = "Status must be one of: Pending, InProgress, Completed, CompletedWithExceptions, Failed.")]
    public string Status { get; set; } = "Pending";

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [Range(1, int.MaxValue)]
    public int? PaymentMethodId { get; set; }

    [Range(1, int.MaxValue)]
    public int? InitiatedByUserId { get; set; }
}
