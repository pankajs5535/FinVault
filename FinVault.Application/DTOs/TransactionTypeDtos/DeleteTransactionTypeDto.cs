using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.TransactionTypeDtos;

/// <summary>Input for deleting a TransactionType.</summary>
public class DeleteTransactionTypeDto
{
    [Range(1, int.MaxValue)]
    public int TransactionTypeId { get; set; }
}
