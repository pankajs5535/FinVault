using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos;

/// <summary>Returned to clients for Get All and Get By Id of PaymentRequest.</summary>
public class PaymentRequestDto
{
    public int PaymentRequestId { get; set; }

    public string RequestReference { get; set; } = string.Empty;

    public string IdempotencyKey { get; set; } = string.Empty;

    public string? UtrNumber { get; set; }

    public string? PayeeName { get; set; }

    public string? PayeeAccountNumber { get; set; }

    public string? PayeeIfsc { get; set; }

    public string? PayeeVpa { get; set; }

    public decimal Amount { get; set; }

    public string? Purpose { get; set; }

    public string? Remarks { get; set; }

    public string Channel { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? FailureCode { get; set; }

    public string? FailureReason { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? AuthorizedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? ScheduledFor { get; set; }

    public int PaymentMethodId { get; set; }

    public int CustomerId { get; set; }

    public int SourceAccountId { get; set; }

    public int? BeneficiaryId { get; set; }

    public int CurrencyId { get; set; }

    public int? TransactionId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
