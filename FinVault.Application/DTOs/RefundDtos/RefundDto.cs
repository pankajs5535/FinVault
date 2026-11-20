using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RefundDtos;

/// <summary>Returned to clients for Get All and Get By Id of Refund.</summary>
public class RefundDto
{
    public int RefundId { get; set; }

    public string RefundReference { get; set; } = string.Empty;

    public string RefundType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string? RejectionReason { get; set; }

    public int OriginalTransactionId { get; set; }

    public int? RefundTransactionId { get; set; }

    public int? DisputeId { get; set; }

    public int? RequestedByUserId { get; set; }

    public int? ApprovedByUserId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
