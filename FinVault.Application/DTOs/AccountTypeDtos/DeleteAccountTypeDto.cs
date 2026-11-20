using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountTypeDtos;

/// <summary>Input for deleting an AccountType.</summary>
public class DeleteAccountTypeDto
{
    [Range(1, int.MaxValue)]
    public int AccountTypeId { get; set; }
}
