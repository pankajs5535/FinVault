using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentGatewayLogDtos;

/// <summary>Input for creating a PaymentGatewayLog.</summary>
public class CreatePaymentGatewayLogDto
{
    [Required]
    [StringLength(50)]
    public string GatewayName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^(Initiate|StatusCheck|Callback|Reversal|Refund)$", ErrorMessage = "Operation must be one of: Initiate, StatusCheck, Callback, Reversal, Refund.")]
    public string Operation { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string CorrelationId { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int AttemptNumber { get; set; } = 1;

    public string? RequestPayload { get; set; }

    public string? ResponsePayload { get; set; }

    public bool IsSuccess { get; set; }

    [Range(100, 599)]
    public int? HttpStatusCode { get; set; }

    [StringLength(30)]
    public string? ResponseCode { get; set; }

    [StringLength(500)]
    public string? ResponseMessage { get; set; }

    [Range(0, int.MaxValue)]
    public int? DurationMs { get; set; }

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    [Range(1, int.MaxValue)]
    public int PaymentRequestId { get; set; }
}
