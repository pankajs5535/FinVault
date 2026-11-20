using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos;

/// <summary>Input for deleting a PaymentRequest.</summary>
public class DeletePaymentRequestDto
{
    [Range(1, int.MaxValue)]
    public int PaymentRequestId { get; set; }
}
