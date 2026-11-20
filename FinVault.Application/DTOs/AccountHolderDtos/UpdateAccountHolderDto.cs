using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountHolderDtos;

/// <summary>Input for updating an AccountHolder.</summary>
public class UpdateAccountHolderDto
{
    [Range(1, int.MaxValue)]
    public int AccountHolderId { get; set; }

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^(Primary|Joint|Guardian|AuthorizedSignatory)$", ErrorMessage = "HolderType must be one of: Primary, Joint, Guardian, AuthorizedSignatory.")]
    public string HolderType { get; set; } = "Primary";

    public bool CanOperate { get; set; } = true;

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int AccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }
}
