using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReconciliationRecordDtos;

/// <summary>Input for updating a ReconciliationRecord.</summary>
public class UpdateReconciliationRecordDto
{
    [Range(1, int.MaxValue)]
    public int ReconciliationRecordId { get; set; }

    [Required]
    [StringLength(50)]
    public string ExternalReference { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal ExternalAmount { get; set; }

    public DateTime ExternalDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? InternalAmount { get; set; }

    public decimal DifferenceAmount { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Matched|Unmatched|AmountMismatch|MissingInBank|MissingInExternal|Manual)$", ErrorMessage = "MatchStatus must be one of: Matched, Unmatched, AmountMismatch, MissingInBank, MissingInExternal, Manual.")]
    public string MatchStatus { get; set; } = "Unmatched";

    [StringLength(250)]
    public string? ExceptionReason { get; set; }

    public DateTime? ResolvedAt { get; set; }

    [StringLength(500)]
    public string? ResolutionNotes { get; set; }

    [Range(1, int.MaxValue)]
    public int ReconciliationBatchId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ResolvedByUserId { get; set; }
}
