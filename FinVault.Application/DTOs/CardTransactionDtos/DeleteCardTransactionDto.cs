using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CardTransactionDtos;

/// <summary>Input for deleting a CardTransaction.</summary>
public class DeleteCardTransactionDto
{
    [Range(1, int.MaxValue)]
    public int CardTransactionId { get; set; }
}
