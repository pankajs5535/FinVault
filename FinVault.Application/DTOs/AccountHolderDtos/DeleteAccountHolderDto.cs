using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountHolderDtos;

/// <summary>Input for deleting an AccountHolder.</summary>
public class DeleteAccountHolderDto
{
    [Range(1, int.MaxValue)]
    public int AccountHolderId { get; set; }
}
