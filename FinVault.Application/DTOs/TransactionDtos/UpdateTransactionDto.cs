using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.TransactionDtos;

/// <summary>Input for updating a Transaction.</summary>
public class UpdateTransactionDto
{
    [Range(1, int.MaxValue)]
    public int TransactionId { get; set; }

    [Required]
    [StringLength(40)]
    public string TransactionReference { get; set; } = string.Empty;

    [StringLength(64)]
    public string? IdempotencyKey { get; set; }

    [StringLength(50)]
    public string? ExternalReference { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal FeeAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TaxAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SourceBalanceAfter { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Initiated|Pending|Processing|Posted|Failed|Reversed|Cancelled|OnHold)$", ErrorMessage = "Status must be one of: Initiated, Pending, Processing, Posted, Failed, Reversed, Cancelled, OnHold.")]
    public string Status { get; set; } = "Initiated";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Branch|ATM|NetBanking|MobileApp|UPI|POS|Api|Batch)$", ErrorMessage = "Channel must be one of: Branch, ATM, NetBanking, MobileApp, UPI, POS, Api, Batch.")]
    public string Channel { get; set; } = "Api";

    public DateTime TransactionAt { get; set; }

    public DateTime ValueDate { get; set; }

    public DateTime? PostedAt { get; set; }

    [StringLength(250)]
    public string? FailureReason { get; set; }

    [StringLength(250)]
    public string? Description { get; set; }

    [StringLength(250)]
    public string? Remarks { get; set; }

    [Range(1, int.MaxValue)]
    public int TransactionTypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int? SourceAccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? DestinationAccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }

    [Range(1, int.MaxValue)]
    public int? FeeId { get; set; }

    [Range(1, int.MaxValue)]
    public int? InitiatedByUserId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ApprovedByUserId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ReversalOfTransactionId { get; set; }

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
