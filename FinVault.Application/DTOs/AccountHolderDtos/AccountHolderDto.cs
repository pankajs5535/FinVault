using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountHolderDtos;

/// <summary>Returned to clients for Get All and Get By Id of AccountHolder.</summary>
public class AccountHolderDto
{
    public int AccountHolderId { get; set; }

    public string HolderType { get; set; } = string.Empty;

    public bool CanOperate { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public int AccountId { get; set; }

    public int CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
