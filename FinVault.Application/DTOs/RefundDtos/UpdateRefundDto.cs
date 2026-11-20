using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RefundDtos;

/// <summary>Input for updating a Refund.</summary>
public class UpdateRefundDto
{
    [Range(1, int.MaxValue)]
    public int RefundId { get; set; }

    [Required]
    [StringLength(40)]
    public string RefundReference { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Full|Partial)$", ErrorMessage = "RefundType must be one of: Full, Partial.")]
    public string RefundType { get; set; } = "Full";

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(250)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Requested|Approved|Rejected|Processed|Failed)$", ErrorMessage = "Status must be one of: Requested, Approved, Rejected, Processed, Failed.")]
    public string Status { get; set; } = "Requested";

    public DateTime RequestedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    [StringLength(250)]
    public string? RejectionReason { get; set; }

    [Range(1, int.MaxValue)]
    public int OriginalTransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int? RefundTransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int? DisputeId { get; set; }

    [Range(1, int.MaxValue)]
    public int? RequestedByUserId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ApprovedByUserId { get; set; }

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
