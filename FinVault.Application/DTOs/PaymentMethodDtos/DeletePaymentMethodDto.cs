using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentMethodDtos;

/// <summary>Input for deleting a PaymentMethod.</summary>
public class DeletePaymentMethodDto
{
    [Range(1, int.MaxValue)]
    public int PaymentMethodId { get; set; }
}
