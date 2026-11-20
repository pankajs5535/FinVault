using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos;

/// <summary>Input for updating a PaymentRequest.</summary>
public class UpdatePaymentRequestDto
{
    [Range(1, int.MaxValue)]
    public int PaymentRequestId { get; set; }

    [Required]
    [StringLength(40)]
    public string RequestReference { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string IdempotencyKey { get; set; } = string.Empty;

    [StringLength(30)]
    public string? UtrNumber { get; set; }

    [StringLength(150)]
    public string? PayeeName { get; set; }

    [StringLength(20)]
    public string? PayeeAccountNumber { get; set; }

    [StringLength(11)]
    [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code.")]
    public string? PayeeIfsc { get; set; }

    [StringLength(100)]
    public string? PayeeVpa { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(250)]
    public string? Purpose { get; set; }

    [StringLength(250)]
    public string? Remarks { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Branch|NetBanking|MobileApp|UPI|Api)$", ErrorMessage = "Channel must be one of: Branch, NetBanking, MobileApp, UPI, Api.")]
    public string Channel { get; set; } = "MobileApp";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Created|Authorized|Submitted|Processing|Completed|Failed|Rejected|Expired|Cancelled)$", ErrorMessage = "Status must be one of: Created, Authorized, Submitted, Processing, Completed, Failed, Rejected, Expired, Cancelled.")]
    public string Status { get; set; } = "Created";

    [StringLength(30)]
    public string? FailureCode { get; set; }

    [StringLength(250)]
    public string? FailureReason { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? AuthorizedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? ScheduledFor { get; set; }

    [Range(1, int.MaxValue)]
    public int PaymentMethodId { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int SourceAccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? BeneficiaryId { get; set; }

    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
