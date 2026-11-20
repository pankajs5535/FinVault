using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentGatewayLogDtos;

/// <summary>Input for deleting a PaymentGatewayLog.</summary>
public class DeletePaymentGatewayLogDto
{
    [Range(1, int.MaxValue)]
    public int PaymentGatewayLogId { get; set; }
}
