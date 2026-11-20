using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentGatewayLogDtos;

/// <summary>Input for updating a PaymentGatewayLog. PaymentGatewayLog records are append-only, so only the key is accepted.</summary>
public class UpdatePaymentGatewayLogDto
{
    [Range(1, int.MaxValue)]
    public int PaymentGatewayLogId { get; set; }
}
