using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.TransactionDtos;

/// <summary>Input for deleting a Transaction.</summary>
public class DeleteTransactionDto
{
    [Range(1, int.MaxValue)]
    public int TransactionId { get; set; }
}
