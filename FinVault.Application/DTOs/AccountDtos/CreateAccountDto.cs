using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountDtos;

/// <summary>Input for creating an Account.</summary>
public class CreateAccountDto
{
    [Required]
    [StringLength(18)]
    [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Account number must be 9 to 18 digits.")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Pending|Active|Dormant|Inactive|Frozen|Closed)$", ErrorMessage = "Status must be one of: Pending, Active, Dormant, Inactive, Frozen, Closed.")]
    public string Status { get; set; } = "Pending";

    [StringLength(250)]
    public string? StatusReason { get; set; }

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^(Single|Joint|EitherOrSurvivor|FormerOrSurvivor)$", ErrorMessage = "ModeOfOperation must be one of: Single, Joint, EitherOrSurvivor, FormerOrSurvivor.")]
    public string ModeOfOperation { get; set; } = "Single";

    [Range(0, double.MaxValue)]
    public decimal OverdraftLimit { get; set; }

    public DateTime OpenedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    [StringLength(150)]
    public string? NomineeName { get; set; }

    [StringLength(50)]
    public string? NomineeRelationship { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? PrincipalAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaturityAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ContractedInterestRate { get; set; }

    [Range(0, int.MaxValue)]
    public int? TenureMonths { get; set; }

    public DateTime? MaturityDate { get; set; }

    public bool AutoRenew { get; set; }

    [Range(1, int.MaxValue)]
    public int AccountTypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int BranchId { get; set; }

    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }

    [Range(1, int.MaxValue)]
    public int? PayoutAccountId { get; set; }
}
