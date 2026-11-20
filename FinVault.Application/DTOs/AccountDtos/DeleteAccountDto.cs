using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountDtos;

/// <summary>Input for deleting an Account.</summary>
public class DeleteAccountDto
{
    [Range(1, int.MaxValue)]
    public int AccountId { get; set; }
}
