using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.TransactionDtos;

/// <summary>Returned to clients for Get All and Get By Id of Transaction.</summary>
public class TransactionDto
{
    public int TransactionId { get; set; }

    public string TransactionReference { get; set; } = string.Empty;

    public string? IdempotencyKey { get; set; }

    public string? ExternalReference { get; set; }

    public decimal Amount { get; set; }

    public decimal FeeAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal? SourceBalanceAfter { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Channel { get; set; } = string.Empty;

    public DateTime TransactionAt { get; set; }

    public DateTime ValueDate { get; set; }

    public DateTime? PostedAt { get; set; }

    public string? FailureReason { get; set; }

    public string? Description { get; set; }

    public string? Remarks { get; set; }

    public int TransactionTypeId { get; set; }

    public int? SourceAccountId { get; set; }

    public int? DestinationAccountId { get; set; }

    public int CurrencyId { get; set; }

    public int? FeeId { get; set; }

    public int? InitiatedByUserId { get; set; }

    public int? ApprovedByUserId { get; set; }

    public int? ReversalOfTransactionId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
