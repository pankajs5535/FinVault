using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RefundDtos;

/// <summary>Input for deleting a Refund.</summary>
public class DeleteRefundDto
{
    [Range(1, int.MaxValue)]
    public int RefundId { get; set; }
}
