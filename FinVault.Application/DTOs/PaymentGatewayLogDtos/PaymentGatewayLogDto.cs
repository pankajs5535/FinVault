using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentGatewayLogDtos;

/// <summary>Returned to clients for Get All and Get By Id of PaymentGatewayLog.</summary>
public class PaymentGatewayLogDto
{
    public int PaymentGatewayLogId { get; set; }

    public string GatewayName { get; set; } = string.Empty;

    public string Operation { get; set; } = string.Empty;

    public string CorrelationId { get; set; } = string.Empty;

    public int AttemptNumber { get; set; }

    public string? RequestPayload { get; set; }

    public string? ResponsePayload { get; set; }

    public bool IsSuccess { get; set; }

    public int? HttpStatusCode { get; set; }

    public string? ResponseCode { get; set; }

    public string? ResponseMessage { get; set; }

    public int? DurationMs { get; set; }

    public string? ErrorMessage { get; set; }

    public int PaymentRequestId { get; set; }

    public DateTime CreatedAt { get; set; }
}
